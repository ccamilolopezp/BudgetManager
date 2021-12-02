using Finac.Sql.Utils.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BudgetManager.Module.BusinessManagers
{
    public class LinkManager
    {
        public void LinkExecutedToBudget(Budget.Model.Sql.BudgetUnit budgetUnit,
                                         Budget.Model.Sql.ExecutedBudgetUnit executedBudgetUnit)
        {
            BusinessObjects.BudgetManagerDbContext dbContext = new BusinessObjects.BudgetManagerDbContext();

            var budgetAccounts = dbContext.Set<Budget.Model.Sql.BudgetAccount>()
                                          .Where(budgetAccount => budgetAccount.BudgetUnit.Id == budgetUnit.Id)
                                          .ToList()
                                          .Where(budgetAccount => budgetAccount.PeriodBudgetValues != null && budgetAccount.PeriodBudgetValues.Count() != 0)
                                          .ToList();

            var storedExecutedAccounts = dbContext.Set<Budget.Model.Sql.ExecutedAccount>()
                                                  .Where(executedAccount => executedAccount.ExecutedBudgetUnit.Id == executedBudgetUnit.Id)
                                                  .ToList()
                                                  .Where(executedAccount => executedAccount.PeriodExecutedValues != null && executedAccount.PeriodExecutedValues.Count() != 0)
                                                  .GroupBy(executedAccount => executedAccount.PeriodExecutedValues.First().AccountCode)
                                                  .Where(groupByAccountCode => groupByAccountCode.Count() > 1)
                                                  .Select(groupByAccountCode =>
                                                  {
                                                      var initialMessage = $"La cuenta {groupByAccountCode.Key} esta asociada a los nodos:\n";
                                                      var nodes = groupByAccountCode.Select(b => $"{b.AccountingTreeNode.Name}-{b.AccountingTreeNode.Label}")
                                                                                    .ToList();

                                                      var complementaryMessage = string.Join("\n", nodes);
                                                      return string.Join("\n", initialMessage, complementaryMessage, "\n");
                                                  });

            if (storedExecutedAccounts.Count() > 0)
            {
                var message = $"Error: no es posible asociar 2 cuentas del árbol {executedBudgetUnit.ExecutedAccounts.First().AccountingTreeNode.Tree.Name} a una cuenta del árbol " +
                              $"{budgetUnit.BudgetAccounts.First().AccountingTreeNode.Tree.Name}:\n";
                var finalmessage = string.Join("\n", storedExecutedAccounts);
                throw new Exception($"{message} \n {finalmessage}");
            }

            var executedAccounts = dbContext.Set<Budget.Model.Sql.ExecutedAccount>()
                                            .Where(executedAccount => executedAccount.ExecutedBudgetUnit.Id == executedBudgetUnit.Id)
                                            .ToList()
                                            .Where(executedAccount => executedAccount.PeriodExecutedValues != null && executedAccount.PeriodExecutedValues.Count() != 0)
                                            .ToDictionary(executedAccount => executedAccount.PeriodExecutedValues.First().AccountCode,
                                                          executedAccount => executedAccount);

            var storedLinkExecutedBudgets = dbContext.Set<Budget.Model.Sql.LinkExecutedBudget>()
                                               .Where(linkExecutedBudget => linkExecutedBudget.BudgetAccountId.BudgetUnit.Id == budgetUnit.Id ||
                                                                            linkExecutedBudget.ExecutedAccountId.ExecutedBudgetUnit.Id == executedBudgetUnit.Id);

            var storedLinksByBudget = storedLinkExecutedBudgets.Where(linkExecutedBudget => linkExecutedBudget.BudgetAccountId != null)
                                                               .ToDictionary(linkExecutedBudget => linkExecutedBudget.BudgetAccountId,
                                                                             linkExecutedBudget => linkExecutedBudget); 

            var storedLinksByExecuted = storedLinkExecutedBudgets.Where(linkExecutedBudget => linkExecutedBudget.ExecutedAccountId != null)
                                                                 .ToDictionary(linkExecutedBudget => linkExecutedBudget.ExecutedAccountId,
                                                                               linkExecutedBudget => linkExecutedBudget); 


            budgetAccounts.ForEach(budgetAccount =>
                          {
                              var accountReference = budgetAccount.PeriodBudgetValues.First().AccountCode;
                              if (!executedAccounts.ContainsKey(accountReference)) return;

                              var link = GetLinkExecutedBudget(storedLinksByBudget, storedLinksByExecuted, budgetAccount, executedAccounts[accountReference]);

                              if (budgetAccount.LinkExecutedBudget == null) budgetAccount.LinkExecutedBudget = link;
                              if (executedAccounts[accountReference].LinkExecutedBudget == null) executedAccounts[accountReference].LinkExecutedBudget = link;
                              if (link.ExecutedAccountId == null) link.ExecutedAccountId = executedAccounts[accountReference];
                              if (link.BudgetAccountId == null) link.BudgetAccountId = budgetAccount;
                              budgetAccount.PeriodBudgetValues.ForEach(periodBudgetValue => periodBudgetValue.UpdateExecutedAmount(true));
                              budgetAccount.UpdateComponentBudgetStatus(true);
                          });

            dbContext.BulkSaveChanges();
        }

        public Budget.Model.Sql.LinkExecutedBudget GetLinkExecutedBudget(Dictionary<Budget.Model.Sql.BudgetAccount, Budget.Model.Sql.LinkExecutedBudget> storedLinksByBudget,
                                                                               Dictionary<Budget.Model.Sql.ExecutedAccount, Budget.Model.Sql.LinkExecutedBudget> storedLinksByExecuted,
                                                                               Budget.Model.Sql.BudgetAccount budgetAccount,
                                                                               Budget.Model.Sql.ExecutedAccount executedAccount)
            => storedLinksByExecuted.ContainsKey(executedAccount) ?
               storedLinksByExecuted[executedAccount] :
               storedLinksByBudget.ContainsKey(budgetAccount) ?
               storedLinksByBudget[budgetAccount] :
               CreateLinkExecutedBudget(budgetAccount, executedAccount);

        public Budget.Model.Sql.LinkExecutedBudget CreateLinkExecutedBudget(Budget.Model.Sql.BudgetAccount budgetAccount,
                                                                                  Budget.Model.Sql.ExecutedAccount executedAccount)
            => new Budget.Model.Sql.LinkExecutedBudget
            {
                BudgetAccountId = budgetAccount,
                ExecutedAccountId = executedAccount
            };
    }
}