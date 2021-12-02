using System.Data.Entity.Migrations;

internal sealed class Configuration : DbMigrationsConfiguration<BudgetManager.Module.BusinessObjects.BudgetManagerDbContext>
{
    public Configuration()
    {
        AutomaticMigrationsEnabled = true;
        AutomaticMigrationDataLossAllowed = true;
        ContextKey = "BudgetManager.Module.BusinessObjects.BudgetManagerDbContext";
    }

    protected override void Seed(BudgetManager.Module.BusinessObjects.BudgetManagerDbContext context)
    {

    }
}
