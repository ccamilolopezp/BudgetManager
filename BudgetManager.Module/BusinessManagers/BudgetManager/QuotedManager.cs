using BudgetManager.Module.BusinessObjects.NonPersistent;
using Finac.Sql.Utils.Extensions;
using System;
using System.Globalization;
using System.Linq;

namespace BudgetManager.Module.BusinessManagers
{
    public class QuotedManager
    {
        private Infrastructure.FetchBudgetItemsManager FetchManager = new Infrastructure.FetchBudgetItemsManager();
        private Infrastructure.CreateBudgetItemsManager CreateManager = new Infrastructure.CreateBudgetItemsManager();
        private ValidationManager ValidationManager = new ValidationManager();

        public void LoadQuoted(Budget.Model.Sql.Company company,
                               LoadBudgetValuesParameters parameters)
        {
            var dbContext = new BusinessObjects.BudgetManagerDbContext();
            var cultureInfo = new CultureInfo(parameters.LoadParameters.CultureInfo);


            var dates = (new TimeManager()).GetDatesToProcess(parameters.InitialDate,
                                                              parameters.FinalDate,
                                                              company.BudgetPeriodicity.PeriodInMonths);

            if (dates.Count() != parameters.LoadParameters.TotalValueColumns) throw new Exception($"Error en los parametros de carga, ha seleccionado un rango de {dates.Count()} fechas," +
                                                                                                  $"pero en los parametros de carga \"{parameters.LoadParameters.Name}\" hay {parameters.LoadParameters.TotalValueColumns}");
            ValidationManager.ValidateStoredBudgetUnit(dbContext, dates, company);

            var nodes1 = dbContext.Set<Budget.XAF.Model.Sql.Node>()
                                 .Where(node => node.Tree.Name == parameters.LoadParameters.TreeName.Tree.Name)
                                 .Select(a => new { a.Name, a.Label })
                                 .GroupBy(a => a.Name)
                                 .Where(a => a.Count() > 1)
                                 .ToList();

            var nodes = dbContext.Set<Budget.XAF.Model.Sql.Node>()
                                 .Where(node => node.Tree.Name == parameters.LoadParameters.TreeName.Tree.Name)
                                 .ToDictionary(node => node.Name,
                                               node => node);



            var accounts = (new AccountManager()).GetAccounts(parameters.BudgetData,
                                                              parameters.LoadParameters)
                                                  .ToDictionary(account =>
                                                  {
                                                      if (!nodes.ContainsKey(parameters.LoadParameters.TreeName.Name + account.Name))
                                                          throw new Exception($"La linea {account.Line + 1} del archivo contiene la cuenta {account.Name}, " +
                                                                              $"que no ha sido parametrizada previamente en la estructura del arbol {parameters.LoadParameters.TreeName.Name}");
                                                      return nodes[parameters.LoadParameters.TreeName.Name + account.Name];
                                                  },
                                                  account => account);

            var _company = dbContext.Set<Budget.Model.Sql.Company>()
                                    .FirstOrDefault(comp => comp.Id == company.Id);

            var budgetUnits = dates.GroupBy(date => date.Year)
                                   .ToList()
                                   .Select((dateByYear, index) =>
                                   {
                                       var budgetUnit = CreateManager.CreateBudgetUnit(_company,
                                                                                       dateByYear.Key);

                                       return new
                                       {
                                           Index = index,
                                           BudgetUnit = budgetUnit,
                                           Dates = dateByYear.ToList()
                                       };
                                   }).ToList();

            budgetUnits.ForEach(budgetUnit =>
                           accounts.ToList()
                                   .ForEach(account =>
                                   {
                                       var budgetAccount = CreateManager.CreateBudgetAccount(dbContext,
                                                                                             account.Key,
                                                                                             budgetUnit.BudgetUnit,
                                                                                             account.Value.Label);

                                       var periodBudgetValues = FetchManager.GetPeriodBudgetValues(account.Value.Name,
                                                                                                   budgetAccount,
                                                                                                   account.Value.Label,
                                                                                                   budgetUnit.Dates,
                                                                                                   company.BudgetPeriodicity.PeriodInMonths);

                                       budgetAccount.PeriodBudgetValues = periodBudgetValues.ToList();

                                       if (!account.Value.Values.ContainsKey(Budget.Model.TransactionType.Quoted)) ValidationManager.AccountExceptionBuilder(account.Value, "no tiene Valores Válidos");

                                       var skip = budgetUnit.Index * budgetUnit.Dates.Count();
                                       var take = budgetUnit.Dates.Count();

                                       var values = account.Value.Values[Budget.Model.TransactionType.Quoted]
                                                                 .Skip(skip)
                                                                 .Take(take)
                                                                 .ToArray();

                                       var _transactions = FetchManager.GetTransactionBudgetValues(periodBudgetValues,
                                                                                                   values,
                                                                                                   Budget.Model.TransactionType.Quoted).ToList();

                                       budgetUnit.BudgetUnit.BudgetAccounts.Add(budgetAccount);
                                       budgetAccount.PeriodBudgetValues.ForEach(periodBudgetValue => periodBudgetValue.UpdatePeriodAmount(true));
                                       budgetAccount.UpdateComponentBudgetStatus(true);
                                   }));

            dbContext.SaveMany(budgetUnits.Select(a => a.BudgetUnit));
        }

    }
}