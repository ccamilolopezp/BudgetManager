using System;
using System.Collections.Generic;

namespace BudgetManager.Module.BusinessManagers.Infrastructure
{
    public class CreateBudgetItemsManager
    {
        public Budget.Model.Sql.ExecutedBudgetUnit CreateExecutedBudgetUnit(Budget.Model.Sql.Company company,
                                                                                  int year)
           => new Budget.Model.Sql.ExecutedBudgetUnit
           {
               Year = year,
               Company = company,
               ExecutedAccounts = new List<Budget.Model.Sql.ExecutedAccount>()
           };

        public Budget.Model.Sql.ExecutedAccount CreateExecutedBudgetAccount(BusinessObjects.BudgetManagerDbContext dbContext,
                                                                                  Budget.XAF.Model.Sql.Node accountingTreeNode,
                                                                                  Budget.Model.Sql.ExecutedBudgetUnit executedBudgetUnit,
                                                                                  string description)
            => new Budget.Model.Sql.ExecutedAccount
            {
                AccountingTreeNode = accountingTreeNode,
                ExecutedBudgetUnit = executedBudgetUnit,
                Description = description,
                PeriodExecutedValues = new List<Budget.Model.Sql.PeriodExecutedValue>(),
            };

        public Budget.Model.Sql.PeriodExecutedValue CreatePeriodExecutedValue(string accountCode,
                                                                                    Budget.Model.Sql.ExecutedAccount executedAccount,
                                                                                    string description,
                                                                                    Tuple<DateTime, DateTime> dates,
                                                                                    double openingBalance,
                                                                                    double closingBalance)
            => new Budget.Model.Sql.PeriodExecutedValue()
            {
                AccountCode = accountCode,
                ExecutedAccount = executedAccount,
                Description = description,
                PeriodDate = dates.Item1,
                EndOfPeriod = dates.Item2,
                OpeningBalance = openingBalance,
                ClosingBalance = closingBalance,
                TransactionValues = new List<Budget.Model.Sql.TransactionExecutedValue>(),

            };

        public Budget.Model.Sql.TransactionExecutedValue CreateTransactionExecutedValue(Budget.Model.Sql.PeriodExecutedValue periodExecutedValue,
                                                                                              double value,
                                                                                              Budget.Model.TransactionType transactionType,
                                                                                              string description = "")
            => new Budget.Model.Sql.TransactionExecutedValue
            {
                Amount = value,
                Description = description != string.Empty ? description : periodExecutedValue.Description,
                FullAccountCode = periodExecutedValue.AccountCode,
                PeriodExecutedValue = periodExecutedValue,
                TransactionType = transactionType
            };

        public Budget.Model.Sql.BudgetUnit CreateBudgetUnit(Budget.Model.Sql.Company company,
                                                                 int year)
           => new Budget.Model.Sql.BudgetUnit
           {
               Year = year,
               Company = company,
               BudgetAccounts = new List<Budget.Model.Sql.BudgetAccount>(),
           };

        public Budget.Model.Sql.BudgetAccount CreateBudgetAccount(BusinessObjects.BudgetManagerDbContext dbContext,
                                                                        Budget.XAF.Model.Sql.Node accountingTreeNode,
                                                                        Budget.Model.Sql.BudgetUnit budgetUnit,
                                                                        string description)
            => new Budget.Model.Sql.BudgetAccount
            {
                AccountingTreeNode = accountingTreeNode,
                BudgetUnit = budgetUnit,
                Description = description,
                PeriodBudgetValues = new List<Budget.Model.Sql.PeriodBudgetValue>(),
            };

        public Budget.Model.Sql.PeriodBudgetValue CreatePeriodBudgetValue(string accountCode,
                                                                                Budget.Model.Sql.BudgetAccount budgetAccount,
                                                                                string description,
                                                                                Tuple<DateTime, DateTime> dates)
            => new Budget.Model.Sql.PeriodBudgetValue()
            {
                AccountCode = accountCode,
                BudgetAccount = budgetAccount,
                Description = description,
                PeriodDate = dates.Item1,
                EndOfPeriod = dates.Item2,
                TransactionValues = new List<Budget.Model.Sql.TransactionBudgetValue>(),
            };

        public Budget.Model.Sql.TransactionBudgetValue CreateTransactionBudgetValue(Budget.Model.Sql.PeriodBudgetValue periodBudgetValue,
                                                                                          double value,
                                                                                          Budget.Model.TransactionType transactionType)
            => new Budget.Model.Sql.TransactionBudgetValue
            {
                Amount = value,
                Description = periodBudgetValue.Description,
                FullAccountCode = periodBudgetValue.AccountCode,
                PeriodBudgetValue = periodBudgetValue,
                TransactionType = transactionType
            };
    }
}
