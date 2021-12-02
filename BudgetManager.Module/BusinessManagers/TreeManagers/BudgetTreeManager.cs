using BudgetManager.Module.BusinessObjects;
using Finac.Sql.Utils.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BudgetManager.Module.BusinessManagers.TreeManagers
{
    class BudgetTreeManager
    {
        public void LoadBudgetTree(Budget.Model.Sql.Company company,
                           BusinessObjects.NonPersistent.LoadBudgetTreeParameters parameters)
        {
            BudgetManagerDbContext dbContext = new BudgetManagerDbContext();
            var treeName = dbContext.Set<Budget.XAF.Model.Sql.TreeName>()
                                    .FirstOrDefault(_treeName => _treeName.Id == parameters.LoadParameters.TreeName.Id);

            var accounts = (new AccountManager()).GetAccounts(parameters.BudgetData,
                                                              parameters.LoadParameters);

            var budgetTree = GetBudgetTree(dbContext, company, treeName);

            var headNode = NewBudgetTreeNode(treeName.Name,
                                             budgetTree as Budget.XAF.Model.Sql.Tree,
                                             null,
                                             treeName.Name);

            dbContext.Save(headNode);

            CreateBudgetTreeNodes(dbContext,
                                  accounts,
                                  budgetTree,
                                  headNode,
                                  new Dictionary<string, Budget.XAF.Model.Sql.Node>());
        }

        public void CreateBudgetTreeNodes(BudgetManagerDbContext dbContext,
                                          IEnumerable<Account> accounts,
                                          Hierarchy.Model.ITree budgetTree,
                                          Budget.XAF.Model.Sql.Node headNode,
                                          Dictionary<string, Budget.XAF.Model.Sql.Node> storedNodes)
        {
            Dictionary<string, Budget.XAF.Model.Sql.Node> nodeDictionary = new Dictionary<string, Budget.XAF.Model.Sql.Node>();

            var parentsAndAncestors = BudgetManager.Utils.DataLoadFunctions.MappedParentAndAncestors(accounts.Select(o => new Tuple<string, string>(o.Name, o.Label)));

            var toSave = parentsAndAncestors.Select(d =>
                                            {
                                                var valueModelTreeNodeParent = headNode;
                                                if (d.Item3 != "") valueModelTreeNodeParent = nodeDictionary[d.Item3];
                                                var key = string.Join("", budgetTree.Name, d.Item1, "-", d.Item2);
                                                var valueModelTreeNode = storedNodes.ContainsKey(key) ? storedNodes[key] : NewBudgetTreeNode(d.Item2,
                                                                                                                                             budgetTree as Budget.XAF.Model.Sql.Tree,
                                                                                                                                             valueModelTreeNodeParent,
                                                                                                                                             headNode.Name,
                                                                                                                                             d.Item1);
                                                if (!nodeDictionary.ContainsKey(d.Item1)) nodeDictionary.Add(d.Item1, valueModelTreeNode);
                                                return valueModelTreeNode;
                                            })
                                            .Where(node => !storedNodes.ContainsKey(string.Join("-", node.Name, node.Label)));

            dbContext.SaveMany(toSave);

            UpdateHeadAndEndNodes(dbContext, headNode);
        }

        public Budget.XAF.Model.Sql.Node NewBudgetTreeNode(string label,
                                                                 Budget.XAF.Model.Sql.Tree valueModelTree,
                                                                 Budget.XAF.Model.Sql.Node parent,
                                                                 string name,
                                                                 string code = "")
            => new Budget.XAF.Model.Sql.Node
            {
                Label = label,
                Name = code != "" ? name + code : name,
                Tree = valueModelTree,
                Parent = parent,
            };


        public Hierarchy.Model.ITree GetBudgetTree(BudgetManagerDbContext dbContext,
                                                         Budget.Model.Sql.Company company,
                                                         Hierarchy.Model.ITreeName treeName)
        {
            var _company = dbContext.Set<Budget.Model.Sql.Company>().FirstOrDefault(_hierarchy => _hierarchy.Id == company.Id);
            var existentTree = _company.AccountingTrees.FirstOrDefault(_tree => _tree.Name == treeName.Name);
            if (existentTree != null) throw new Exception("Ya existe un árbol con Nombre " + existentTree.Name + " Para crearlo nuevamente. Elimine primero el anterior");
          

            var tree = AppParameters.Fabric.NewTree();
            tree.Name = treeName.Name;
            tree.Code = treeName.Name;
            AppParameters.Fabric.SetTreeName(ref tree, treeName);
            _company.AccountingTrees.Add(tree as Budget.XAF.Model.Sql.Tree);

            dbContext.Save(tree as Budget.XAF.Model.Sql.Tree);

            return tree;
        }

        public void UpdateHeadAndEndNodes(BudgetManagerDbContext dbContext,
                                          Budget.XAF.Model.Sql.Node headNode)
        {
            dbContext = new BudgetManagerDbContext();

            dbContext.Set<Budget.XAF.Model.Sql.Node>()
                     .Where(node => node.Tree.Id == headNode.Tree.Id)
                     .ToList()
                     .ForEach(node =>
                     {
                         node.IsEndNode = node.Children.Count == 0;
                         node.IsHeadNode = node.Parent == null;
                     });

            dbContext.BulkSaveChanges();
        }
        public void UpdateNodes(BudgetManagerDbContext dbContext,
                                BusinessObjects.NonPersistent.LoadExecutedBudgetValuesParameters parameters,
                                IEnumerable<Account> accounts)
        {
            var storedNodes = dbContext.Set<Budget.XAF.Model.Sql.Node>()
                                       .Where(node => node.Tree.Name == parameters.LoadParameters.TreeName.Tree.Name)
                                       .ToDictionary(node => string.Join("-", node.Name, node.Label),
                                                    node => node);

            var head = storedNodes.Values.FirstOrDefault(node => node.Parent == null);

            CreateBudgetTreeNodes(dbContext,
                                  accounts,
                                  head.GetTree(),
                                  head,
                                  storedNodes);
        }
    }
}