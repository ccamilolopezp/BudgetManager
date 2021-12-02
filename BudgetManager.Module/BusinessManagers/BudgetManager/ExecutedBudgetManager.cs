using BudgetManager.Module.BusinessObjects;
using BudgetManager.Module.BusinessObjects.NonPersistent;
using Finac.Sql.Utils.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BudgetManager.Module.BusinessManagers
{
    public class ExecutedBudgetManager
    {
        private BudgetManagerDbContext DbContext;
        private Infrastructure.FetchBudgetItemsManager ItemsManager = new Infrastructure.FetchBudgetItemsManager();
        private ValidationManager ValidationManager = new ValidationManager();

        public void LoadPeriodExecuted(Budget.Model.Sql.Company company,
                                       LoadExecutedBudgetValuesParameters parameters,
                                       DateTime date)
        {
            var accountsInFileData = (new AccountManager()).GetAccounts(parameters.BudgetData,
                                                                        parameters.LoadParameters,
                                                                        true).ToList();

            if (parameters.LoadParameters.ForceCreateMissingNodes) (new TreeManagers.BudgetTreeManager()).UpdateNodes(DbContext = new BudgetManagerDbContext(),
                                                                                                                      parameters,
                                                                                                                      accountsInFileData);

            LoadPeriodValues(accountsInFileData,
                             parameters,
                             company,
                             date);
        }

        public void LoadPeriodValues(IEnumerable<Account> accountsInFileData,
                                     LoadExecutedBudgetValuesParameters parameters,
                                     Budget.Model.Sql.Company company,
                                     DateTime date)
        {
            DbContext = new BudgetManagerDbContext();
            bool isNew = true;

            var nodes = DbContext.Set<Budget.XAF.Model.Sql.Node>()
                                 .Where(node => node.Tree.Name == parameters.LoadParameters.TreeName.Tree.Name)
                                 .GroupBy(node => node.Name)
                                 .ToDictionary(groupByName => groupByName.Key,
                                               groupByName => groupByName.ToDictionary(node => string.Join("-", node.Name, node.Label),
                                                                                       node => node));

            var _company = DbContext.Set<Budget.Model.Sql.Company>()
                                    .FirstOrDefault(comp => comp.Id == company.Id);

            var accounts = accountsInFileData.GroupBy(account => account.Name)
                                             .SelectMany(group =>
                                             {
                                                 var primaryKey = string.Join("", parameters.LoadParameters.TreeName.Name, group.Key);
                                                 if (!nodes.ContainsKey(primaryKey)) throw new Exception($"no existe el nodo {primaryKey}, en el árbol {parameters.LoadParameters.TreeName.Tree.Name}");

                                                 var posibleKeys = group.Select(a => string.Join("", parameters.LoadParameters.TreeName.Name, string.Join("-", a.Name, a.Label)));

                                                 var secondaryKey = nodes[primaryKey].Keys.Where(a => posibleKeys.Contains(a));

                                                 if (secondaryKey.Count() != 1) throw new Exception($"Error: la cuenta {group.First().Name}, {group.First().Label}. Se puede asociar a mas de un nodo " +
                                                                                                    $"verifique la estructura del arbol {parameters.LoadParameters.TreeName.Tree.Name}");

                                                 return group.ToDictionary(a => a,
                                                                           a => nodes[primaryKey][secondaryKey.First()]);
                                             });


            var executedBudgetUnit = ItemsManager.GetExecutedBudgetUnit(DbContext,
                                                                        _company,
                                                                        date,
                                                                        ref isNew);

            accounts.ToList()
                    .ForEach(account =>
                    {
                        ValidationManager.ValidateAccountValues(account.Key);
                        var executedAccount = ItemsManager.GetExecutedAccount(DbContext,
                                                                              account.Value,
                                                                              ref executedBudgetUnit,
                                                                              account.Value.Label);


                        var periodExecutedValue = ItemsManager.GetPeriodExecutedValue(account.Key.Name,
                                                                                      executedAccount,
                                                                                      account.Value.Label,
                                                                                      new Tuple<DateTime, DateTime>(date, date.AddMonths(company.BudgetPeriodicity.PeriodInMonths)),
                                                                                      account.Key.Values[Budget.Model.TransactionType.Executed].First(),
                                                                                      account.Key.Values[Budget.Model.TransactionType.Executed].Last());

                        if (!executedAccount.PeriodExecutedValues.Contains(periodExecutedValue)) executedAccount.PeriodExecutedValues.Add(periodExecutedValue);

                        var _transactions = ItemsManager.GetTransactionExecutedValues(periodExecutedValue,
                                                                                      account.Key.Values
                                                                                                 .Where(transaction => transaction.Key == Budget.Model.TransactionType.Credit ||
                                                                                                                                           transaction.Key == Budget.Model.TransactionType.Debit),
                                                                                      account.Key.Label).ToList();

                        periodExecutedValue.TransactionValues = periodExecutedValue.TransactionValues.Concat(_transactions).ToList();

                        periodExecutedValue.UpdatePeriodAmount(true);
                        executedAccount.UpdateAccountingResultByComponentStatus(true);
                    });

            if (isNew) DbContext.Save(executedBudgetUnit);
            else DbContext.SaveChanges();
        }
    }
}
