using DevExpress.ExpressApp.EF.Updating;
using DevExpress.Persistent.BaseImpl.EF;
using DevExpress.Persistent.BaseImpl.EF.PermissionPolicy;
using System;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Core.Objects.DataClasses;
using System.Data.Entity.Infrastructure;
using System.Text.RegularExpressions;

namespace BudgetManager.Module.BusinessObjects
{
    public class BudgetManagerDbContext : DbContext
    {
        public BudgetManagerDbContext(String connectionString)
            : base(connectionString)
        {
        }
        public BudgetManagerDbContext(DbConnection connection)
            : base(connection, false)
        {
        }
        public BudgetManagerDbContext()
            : base("name=ConnectionString")
        {
        }
        public DbSet<ModuleInfo> ModulesInfo { get; set; }
        public DbSet<PermissionPolicyRole> Roles { get; set; }
        public DbSet<PermissionPolicyTypePermissionObject> TypePermissionObjects { get; set; }
        public DbSet<PermissionPolicyUser> Users { get; set; }
        public DbSet<FileData> FileData { get; set; }
        public DbSet<DashboardData> DashboardData { get; set; }
        public DbSet<Analysis> Analysis { get; set; }
        public DbSet<ReportDataV2> ReportDataV2 { get; set; }
        public DbSet<ModelDifference> ModelDifferences { get; set; }
        public DbSet<ModelDifferenceAspect> ModelDifferenceAspects { get; set; }

        #region BudgetManager.Model.Sql
        public DbSet<Budget.Model.Sql.BudgetAccount> BudgetAccounts { get; set; }
        public DbSet<Budget.Model.Sql.BudgetPeriod> BudgetPeriodNs { get; set; }
        public DbSet<Budget.Model.Sql.BudgetPeriodicity> BudgetPeriodicityNs { get; set; }
        public DbSet<Budget.Model.Sql.BudgetUnit> BudgetUnits { get; set; }
        public DbSet<Budget.Model.Sql.Company> Companies { get; set; }
        public DbSet<Budget.Model.Sql.ExecutedAccount> ExecutedAccounts { get; set; }
        public DbSet<Budget.Model.Sql.ExecutedBudgetUnit> ExecutedBudgetUnits { get; set; }
        public DbSet<Budget.Model.Sql.LinkExecutedBudget> LinkExecutedBudgets { get; set; }
        public DbSet<Budget.Model.Sql.PeriodBudgetValue> PeriodBudgetValues { get; set; }
        public DbSet<Budget.Model.Sql.PeriodExecutedValue> PeriodExecutedValues { get; set; }
        public DbSet<Budget.Model.Sql.TransactionBudgetValue> TransactionBudgetValues { get; set; }
        public DbSet<Budget.Model.Sql.TransactionExecutedValue> TransactionExecutedValues { get; set; }
        public DbSet<Budget.Model.Sql.LoadParameters> LoadParameters { get; set; }
        #endregion

        #region Budget.XAF.Model.Sql
        public DbSet<Budget.XAF.Model.Sql.Node> Nodes { get; set; }
        public DbSet<Budget.XAF.Model.Sql.Tree> Trees { get; set; }
        public DbSet<Budget.XAF.Model.Sql.TreeName> TreeNames { get; set; }
        #endregion

        public string GetTableName<T>() where T : class
        {
            IObjectContextAdapter iObjectContextAdapter = this as IObjectContextAdapter;
            ObjectContext objectContext = iObjectContextAdapter.ObjectContext;
            var sql = objectContext.CreateObjectSet<T>().ToTraceString();
            Regex regex = new Regex("FROM (?<table>.*) AS");
            Match match = regex.Match(sql);
            return match.Groups["table"].Value;
        }

        public string GetRelatedEnds<T>(T parent) where T : class
        {
            //var entityProxy = DbContext.FooBars.First(fb => fb.Id == someId);
            //var proxyType = entityProxy.GetType();

            var wrapperField = (parent.GetType()).GetField("_entityWrapper");
            var wrapper = wrapperField.GetValue(parent);
            var wrapperType = wrapper.GetType();

            var relManProp = wrapperType.GetProperty("RelationshipManager");
            //Debug.Assert(relManProp != null, nameof(relManProp) + " != null");
            var relMan = (RelationshipManager)relManProp.GetValue(wrapper);

            var allEnds = relMan.GetAllRelatedEnds();
            //foreach (var relatedEnd in allEnds)
            //{
            //    Debug.Print("RELATIONSHIP-NAME:" + relatedEnd.RelationshipName);
            //}
            var test = ((parent.GetType().GetField("_entityWrapper")).GetType());
            var entityWithRelationships = parent as IEntityWithRelationships;
            if (entityWithRelationships == null) return null;
            var relatedEnds = entityWithRelationships.RelationshipManager.GetAllRelatedEnds();
            return "";
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Budget.Model.Sql.BudgetAccount>()
                        .HasMany(budgetAccount => budgetAccount.PeriodBudgetValues)
                        .WithRequired(periodBudgetValue => periodBudgetValue.BudgetAccount)
                        .WillCascadeOnDelete();

            modelBuilder.Entity<Budget.Model.Sql.BudgetPeriodicity>()
                        .HasMany(budgetPeriodicity => budgetPeriodicity.BudgetPeriods)
                        .WithRequired(budgetPeriod => budgetPeriod.BudgetPeriodicity)
                        .WillCascadeOnDelete();

            modelBuilder.Entity<Budget.Model.Sql.BudgetUnit>()
                        .HasMany(budgetUnit => budgetUnit.BudgetAccounts)
                        .WithRequired(budgetAccount => budgetAccount.BudgetUnit)
                        .WillCascadeOnDelete();

            modelBuilder.Entity<Budget.Model.Sql.Company>()
                        .HasMany(company => company.Budgets)
                        .WithRequired(budget => budget.Company)
                        .WillCascadeOnDelete();

            modelBuilder.Entity<Budget.Model.Sql.Company>()
                        .HasMany(company => company.ExecutedBudgets)
                        .WithRequired(executedBudget => executedBudget.Company)
                        .WillCascadeOnDelete();

            modelBuilder.Entity<Budget.Model.Sql.ExecutedAccount>()
                        .HasOptional(executedAccount => executedAccount.LinkExecutedBudget)
                        .WithOptionalDependent(linkExecutedBudget => linkExecutedBudget.ExecutedAccountId)
                        .WillCascadeOnDelete();

            modelBuilder.Entity<Budget.Model.Sql.BudgetAccount>()
                        .HasOptional(budgetAccount => budgetAccount.LinkExecutedBudget)
                        .WithOptionalDependent(linkExecutedBudget => linkExecutedBudget.BudgetAccountId)
                        .WillCascadeOnDelete();

            modelBuilder.Entity<Budget.Model.Sql.ExecutedBudgetUnit>()
                        .HasMany(executedBudgetUnit => executedBudgetUnit.ExecutedAccounts)
                        .WithRequired(executedAccount => executedAccount.ExecutedBudgetUnit)
                        .WillCascadeOnDelete();

            modelBuilder.Entity<Budget.Model.Sql.PeriodBudgetValue>()
                        .HasMany(periodBudgetValue => periodBudgetValue.TransactionValues)
                        .WithRequired(transactionValue => transactionValue.PeriodBudgetValue)
                        .WillCascadeOnDelete();

            modelBuilder.Entity<Budget.Model.Sql.PeriodExecutedValue>()
                        .HasMany(periodBudgetValue => periodBudgetValue.TransactionValues)
                        .WithRequired(transactionValue => transactionValue.PeriodExecutedValue)
                        .WillCascadeOnDelete();

            modelBuilder.Entity<Budget.XAF.Model.Sql.TreeName>()
                        .HasOptional(treeName => treeName.Tree)
                        .WithRequired(tree => tree.TreeName)
                        .WillCascadeOnDelete();

            modelBuilder.Entity<Budget.XAF.Model.Sql.Tree>()
                        .HasMany(tree => tree.Nodes)
                        .WithRequired(node => node.Tree)
                        .WillCascadeOnDelete();
        }
    }
}