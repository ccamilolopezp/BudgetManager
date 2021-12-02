using Finac.Sql.Utils.Extensions;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace BudgetManager.Module.BusinessManagers.Infrastructure
{
    public class DeleteManager
    {
        public void DeleteValues(BusinessObjects.NonPersistent.DeleteParameters deleteParameters)
        {
            var dbContext = new BusinessObjects.BudgetManagerDbContext();
            var linksToDelete = new List<long>();

            deleteParameters.ExecutedBudgetUnitsToDelete.SelectMany(executedBudgetUnit =>
                                                                    dbContext.Set<Budget.Model.Sql.ExecutedBudgetUnit>()
                                                                             .Where(_executedBudgetUnit => _executedBudgetUnit.Id == executedBudgetUnit.Id)
                                                                             .ToList()
                                                                             .Where(_executedBudgetUnit => _executedBudgetUnit.ExecutedAccounts != null)
                                                                             .Select(_executedBudgetUnit => new
                                                                             {
                                                                                 ExecutedId = _executedBudgetUnit.Id,

                                                                                 AccountIds = _executedBudgetUnit.ExecutedAccounts.Where(executedAccount => executedAccount != null)
                                                                                                                                  .Select(executedAccount => executedAccount.Id)
                                                                                                                                  .ToList(),

                                                                                 LinkIds = _executedBudgetUnit.ExecutedAccounts.Where(executedAccount => executedAccount != null && executedAccount.LinkExecutedBudget != null)
                                                                                                                               .Select(executedAccount => executedAccount.LinkExecutedBudget.Id)
                                                                                                                               .ToList(),

                                                                                 AssociatedBudgetAccountsIds = _executedBudgetUnit.ExecutedAccounts.Where(executedAccount => executedAccount != null && executedAccount.LinkExecutedBudget != null && executedAccount.LinkExecutedBudget.BudgetAccountId != null)
                                                                                                                               .Select(executedAccount => executedAccount.LinkExecutedBudget.BudgetAccountId.Id)
                                                                                                                               .ToList(),

                                                                                 PeriodValueIds = _executedBudgetUnit.ExecutedAccounts
                                                                                                                     .Where(executedAccount => executedAccount != null && executedAccount.PeriodExecutedValues != null)
                                                                                                                     .SelectMany(executedBudgetAccount =>
                                                                                                                                 executedBudgetAccount.PeriodExecutedValues
                                                                                                                                                      .Select(periodExecutedValue => periodExecutedValue.Id))
                                                                                                                     .ToList(),
                                                                             }))
                                                        .ToList()
                                                        .ForEach(toDelete =>
                                                        {
                                                            toDelete.PeriodValueIds.AsParallel()
                                                                                   .ForAll(periodID =>
                                                                                   {
                                                                                       var internalContext = new BusinessObjects.BudgetManagerDbContext();
                                                                                       DeleteEntitiesSql(internalContext,
                                                                                                         internalContext.GetTableName<Budget.Model.Sql.TransactionExecutedValue>(),
                                                                                                         "PeriodExecutedValue_Id",
                                                                                                         periodID);
                                                                                   });

                                                            toDelete.AccountIds.AsParallel()
                                                                               .ForAll(accountId =>
                                                                               {
                                                                                   var internalContext = new BusinessObjects.BudgetManagerDbContext();
                                                                                   DeleteEntitiesSql(internalContext,
                                                                                                     internalContext.GetTableName<Budget.Model.Sql.PeriodExecutedValue>(),
                                                                                                     "ExecutedAccount_Id",
                                                                                                     accountId);
                                                                               });

                                                            DeleteEntitiesSql(dbContext,
                                                                              dbContext.GetTableName<Budget.Model.Sql.ExecutedAccount>(),
                                                                              "ExecutedBudgetUnit_Id",
                                                                              toDelete.ExecutedId);

                                                            DeleteEntitiesSql(dbContext,
                                                                              dbContext.GetTableName<Budget.Model.Sql.ExecutedBudgetUnit>(),
                                                                              "Id",
                                                                              toDelete.ExecutedId);

                                                            linksToDelete = linksToDelete.Concat(toDelete.LinkIds).ToList();

                                                            UpdateBudgetValues(toDelete.AssociatedBudgetAccountsIds);
                                                        });

            deleteParameters.BudgetUnitsToDelete.SelectMany(budgetUnit =>
                                                            dbContext.Set<Budget.Model.Sql.BudgetUnit>()
                                                                     .Where(_budgetUnit => _budgetUnit.Id == budgetUnit.Id)
                                                                     .ToList()
                                                                     .Where(_budgetUnit => _budgetUnit.BudgetAccounts != null)
                                                                     .Select(_budgetUnit => new
                                                                     {
                                                                         BudgetId = _budgetUnit.Id,

                                                                         AccountIds = _budgetUnit.BudgetAccounts.Where(budgetAccount => budgetAccount != null)
                                                                                                                .Select(budgetAccount => budgetAccount.Id)
                                                                                                                .ToList(),

                                                                         LinkIds = _budgetUnit.BudgetAccounts.Where(budgetAccount => budgetAccount != null && budgetAccount.LinkExecutedBudget != null)
                                                                                                             .Select(budgetAccount => budgetAccount.LinkExecutedBudget.Id)
                                                                                                             .ToList(),

                                                                         PeriodValueIds = _budgetUnit.BudgetAccounts
                                                                                                     .Where(budgetAccount => budgetAccount != null && budgetAccount.PeriodBudgetValues != null)
                                                                                                     .SelectMany(budgetAccount =>
                                                                                                                 budgetAccount.PeriodBudgetValues
                                                                                                                              .Select(periodBudgetValue => periodBudgetValue.Id))
                                                                                                     .ToList(),
                                                                     }))
                                                .ToList()
                                                .ForEach(toDelete =>
                                                {
                                                    toDelete.PeriodValueIds.AsParallel()
                                                                           .ForAll(periodID =>
                                                                           {
                                                                               var internalContext = new BusinessObjects.BudgetManagerDbContext();
                                                                               DeleteEntitiesSql(internalContext,
                                                                                                 internalContext.GetTableName<Budget.Model.Sql.TransactionBudgetValue>(),
                                                                                                 "PeriodBudgetValue_Id",
                                                                                                 periodID);
                                                                           });

                                                    toDelete.AccountIds.AsParallel()
                                                                       .ForAll(accountId =>
                                                                       {
                                                                           var internalContext = new BusinessObjects.BudgetManagerDbContext();
                                                                           DeleteEntitiesSql(internalContext,
                                                                                             internalContext.GetTableName<Budget.Model.Sql.PeriodBudgetValue>(),
                                                                                            "BudgetAccount_Id",
                                                                                            accountId);
                                                                       });

                                                    DeleteEntitiesSql(dbContext,
                                                                      dbContext.GetTableName<Budget.Model.Sql.BudgetAccount>(),
                                                                      "BudgetUnit_Id",
                                                                      toDelete.BudgetId);

                                                    DeleteEntitiesSql(dbContext,
                                                                      dbContext.GetTableName<Budget.Model.Sql.BudgetUnit>(),
                                                                      "Id",
                                                                      toDelete.BudgetId);

                                                    linksToDelete = linksToDelete.Concat(toDelete.LinkIds).ToList();
                                                });


            dbContext = new BusinessObjects.BudgetManagerDbContext();

            dbContext.Set<Budget.Model.Sql.LinkExecutedBudget>()
                     .ToList()
                     .Where(link => linksToDelete.Contains(link.Id) && link.ExecutedAccountId == null && link.BudgetAccountId == null)
                     .Select(link => link.Id)
                     .ToList()
                     .AsParallel()
                     .ForAll(accountId =>
                     {
                         var internalContext = new BusinessObjects.BudgetManagerDbContext();
                         DeleteEntitiesSql(internalContext,
                                           internalContext.GetTableName<Budget.Model.Sql.LinkExecutedBudget>(),
                                           "Id",
                                           accountId);
                     });
        }

        public void UpdateBudgetValues(List<long> AssociatedBudgetAccountsIds)
        {
            var dbContext = new BusinessObjects.BudgetManagerDbContext();

            dbContext.Set<Budget.Model.Sql.BudgetAccount>()
                     .Where(budgetAccount => AssociatedBudgetAccountsIds.Contains(budgetAccount.Id))
                     .ToList()
                     .ForEach(budgetAccount =>
                     {
                         if (budgetAccount.PeriodBudgetValues != null)
                             budgetAccount.PeriodBudgetValues.ForEach(period => period.UpdateExecutedAmount(true));
                         budgetAccount.UpdateComponentBudgetStatus(true);
                     });

            dbContext.BulkSaveChanges();
        }

        public Action<BusinessObjects.BudgetManagerDbContext, string, string, long> DeleteEntitiesSql =
             (dbContext, table, field, id) => dbContext.Database.ExecuteSqlCommand($"DELETE FROM {table} WHERE \"{field}\" = @id ",
                                                                                   new[] { new SqlParameter("id", id) });
    }
}
