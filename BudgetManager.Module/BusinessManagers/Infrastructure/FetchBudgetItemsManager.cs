using System;
using System.Collections.Generic;
using System.Linq;

namespace BudgetManager.Module.BusinessManagers.Infrastructure
{
    public class FetchBudgetItemsManager
    {
        private CreateBudgetItemsManager CreateManager = new CreateBudgetItemsManager();

        public Budget.Model.Sql.ExecutedBudgetUnit GetExecutedBudgetUnit(BusinessObjects.BudgetManagerDbContext dbContext,
                                                                               Budget.Model.Sql.Company company,
                                                                               DateTime date,
                                                                               ref bool isNew)
        {
            var executedBudgetUnit = dbContext.Set<Budget.Model.Sql.ExecutedBudgetUnit>()
                                              .FirstOrDefault(_executedBudgetUnit => _executedBudgetUnit.Company.Id == company.Id && _executedBudgetUnit.Year == date.Year);

            if (executedBudgetUnit == null) return CreateManager.CreateExecutedBudgetUnit(company, date.Year);

            isNew = false;
            return executedBudgetUnit;
        }

        public Budget.Model.Sql.ExecutedAccount GetExecutedAccount(BusinessObjects.BudgetManagerDbContext dbContext,
                                                                         Budget.XAF.Model.Sql.Node accountingTreeNode,
                                                                         ref Budget.Model.Sql.ExecutedBudgetUnit executedBudgetUnit,
                                                                         string label)
        {
            var executedBudgetAccount = executedBudgetUnit.ExecutedAccounts
                                                          .FirstOrDefault(_executedBudgetAccount => _executedBudgetAccount.AccountingTreeNode.ID == accountingTreeNode.ID);

            if (executedBudgetAccount == null)
            {
                executedBudgetAccount = CreateManager.CreateExecutedBudgetAccount(dbContext,
                                                                                  accountingTreeNode,
                                                                                  executedBudgetUnit,
                                                                                  label);

                executedBudgetUnit.ExecutedAccounts.Add(executedBudgetAccount);
            }

            return executedBudgetAccount;
        }

        public Budget.Model.Sql.PeriodExecutedValue GetPeriodExecutedValue(string name,
                                                                                 Budget.Model.Sql.ExecutedAccount executedAccount,
                                                                                 string label,
                                                                                 Tuple<DateTime, DateTime> periodDates,
                                                                                 double openingBalance,
                                                                                 double closingBalance,
                                                                                 bool thirdParties = false)
        {
            var periodExecutedValue = executedAccount.PeriodExecutedValues.Where(_periodExecutedValue => _periodExecutedValue.ExecutedAccount.Id == executedAccount.Id &&
                                                                                                         _periodExecutedValue.PeriodDate.Month == periodDates.Item1.Month &&
                                                                                                         _periodExecutedValue.PeriodDate.Year == periodDates.Item2.Year);

            if (!thirdParties && periodExecutedValue.Count() != 0) throw new Exception($"ya existen valores cargados para el periodo {periodDates.Item1.ToShortDateString()} en la cuenta {name}, " +
                                                                                       $"es posible que este cargando información para terceros, verifique los parametros de carga");

            if (thirdParties && periodExecutedValue.Count() > 1) throw new Exception($"ya existen valores cargados para el periodo {periodDates.Item1.ToShortDateString()} en la cuenta {name}");

            if (periodExecutedValue.Count() != 0) return periodExecutedValue.First();

            return CreateManager.CreatePeriodExecutedValue(name,
                                                           executedAccount,
                                                           label,
                                                           periodDates,
                                                           openingBalance,
                                                           closingBalance);
        }

        public IEnumerable<Budget.Model.Sql.TransactionExecutedValue> GetTransactionExecutedValues(Budget.Model.Sql.PeriodExecutedValue periodExecutedValue,
                                                                                                         IEnumerable<KeyValuePair<Budget.Model.TransactionType, IEnumerable<double>>> values,
                                                                                                         string description = "")
            => values.Select(_value => CreateManager.CreateTransactionExecutedValue(periodExecutedValue,
                                                                                    _value.Value.First(),
                                                                                    _value.Key,
                                                                                    description));

        public Budget.Model.Sql.PeriodBudgetValue[] GetPeriodBudgetValues(string accountCode,
                                                                             Budget.Model.Sql.BudgetAccount budgetAccount,
                                                                             string description,
                                                                             IEnumerable<DateTime> dates,
                                                                             int step)
         => dates.Select(date => CreateManager.CreatePeriodBudgetValue(accountCode,
                                                                       budgetAccount,
                                                                       description,
                                                                       new Tuple<DateTime, DateTime>(date, date.AddMonths(step))))
                 .ToArray();

        public IEnumerable<Budget.Model.Sql.TransactionBudgetValue> GetTransactionBudgetValues(Budget.Model.Sql.PeriodBudgetValue[] periodBudgetValues,
                                                                                                     double[] values,
                                                                                                     Budget.Model.TransactionType transactionType)
        {
            for (int index = 0; index < values.Count(); index++)
            {
                var transaction = CreateManager.CreateTransactionBudgetValue(periodBudgetValues[index],
                                                                             values[index],
                                                                             transactionType);
                periodBudgetValues[index].TransactionValues.Add(transaction);
                yield return transaction;
            }
        }
    }
}
