using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.ReportsV2;
using DevExpress.ExpressApp.Updating;
using Finac.nonSQL.Utils;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace BudgetManager.Module
{
    public sealed partial class BudgetManagerModule : ModuleBase
    {
        static BudgetManagerModule()
        {
            DevExpress.Data.Linq.CriteriaToEFExpressionConverter.SqlFunctionsType = typeof(System.Data.Entity.SqlServer.SqlFunctions);
            DevExpress.Data.Linq.CriteriaToEFExpressionConverter.EntityFunctionsType = typeof(System.Data.Entity.DbFunctions);
            DevExpress.ExpressApp.SystemModule.ResetViewSettingsController.DefaultAllowRecreateView = false;
            // Uncomment this code to delete and recreate the database each time the data model has changed.
            // Do not use this code in a production environment to avoid data loss.
             #if DEBUG
            // Database.SetInitializer(new DropCreateDatabaseIfModelChanges<BudgetManagerDbContext>());
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<BusinessObjects.BudgetManagerDbContext, Configuration>());
             #endif 
        }
        public BudgetManagerModule()
        {
            InitializeComponent();
            DevExpress.ExpressApp.Security.SecurityModule.UsedExportedTypes = DevExpress.Persistent.Base.UsedExportedTypes.Custom;
            AdditionalExportedTypes.Add(typeof(DevExpress.Persistent.BaseImpl.EF.FileData));
            AdditionalExportedTypes.Add(typeof(DevExpress.Persistent.BaseImpl.EF.FileAttachment));
            AdditionalExportedTypes.Add(typeof(DevExpress.Persistent.BaseImpl.EF.Analysis));
            AdditionalExportedTypes.Add(typeof(DevExpress.Persistent.BaseImpl.EF.HCategory));
            List<string> assembliesContentListToInclude = new List<string>()
            {
               "Budget.XAF.Model.Sql",
               "Budget.Model.Sql"
            };
            List<string> assembliesContentListToExclude = new List<string>()
            {
            };
            Dictionary<string, string> prefixes = new Dictionary<string, string>()
            {
                { "Budget.Model.Sql", "Sql" },
                { "Budget.Model.NonSql", "NonSql" }
            };

            UpdateDeclaredExportedTypes(assembliesContentListToInclude, assembliesContentListToExclude, prefixes);
        }
        private void RegisterClassesMap()
        {
           //Register here the persistent Class in MongoDB
        }
        public override IEnumerable<ModuleUpdater> GetModuleUpdaters(IObjectSpace objectSpace, Version versionFromDB)
        {
            ModuleUpdater updater = new DatabaseUpdate.Updater(objectSpace, versionFromDB);
            return new ModuleUpdater[] { updater };
        }
        public override void Setup(XafApplication application)
        {
            base.Setup(application);
            // Manage various aspects of the application UI and behavior at the module level.
        }
        public override void Setup(ApplicationModulesManager moduleManager)
        {
            base.Setup(moduleManager);
            ReportsModuleV2 reportModule = moduleManager.Modules.FindModule<ReportsModuleV2>();
            reportModule.ReportDataType = typeof(DevExpress.Persistent.BaseImpl.EF.ReportDataV2);
        }
        private void UpdateDeclaredExportedTypes(List<string> assembliesContentListToInclude,
                                                 List<string> assembliesContentListToExclude,
                                                 Dictionary<string, string> prefixes)
        {

            System
                .Reflection
                .Assembly
                .GetExecutingAssembly()
                .GetReferencedAssemblies()
                .ToList()
                .ForEach(assemblyName =>
                {
                    bool include = assembliesContentListToInclude.Any(key => assemblyName.FullName.Contains(key));
                    bool exclude = assembliesContentListToExclude.Any(key => assemblyName.FullName.Contains(key));

                    if (include && !exclude)
                    {

                        IEnumerable<Type> entities =
                            ModuleHelper.CollectExportedTypesFromAssembly(
                                    System.Reflection.Assembly.Load(assemblyName),
                                    type => !type.IsAbstract && !type.IsInterface && type.IsPublic
                                    && type.IsClass && !type.IsGenericType
                                    && type.GetConstructor(Type.EmptyTypes) != null
                                    && !typeof(System.ComponentModel.Component).IsAssignableFrom(type)
                                    && !typeof(Attribute).IsAssignableFrom(type));

                        AdditionalExportedTypes
                            .AddRange(entities);

                        if (prefixes.ContainsKey(assemblyName.Name))
                            entities
                                .ToList()
                                .ForEach(entity =>
                                {
                                    DevExpress.ExpressApp.Model.NodeGenerators.ModelNodesGeneratorSettings.SetIdPrefix(
                                        entity,
                                        prefixes[assemblyName.Name] + "_" + entity.Name
                                    );
                                });

                        IEnumerable<Type> domainComponents =
                        ModuleHelper.CollectExportedTypesFromAssembly(
                                    System.Reflection.Assembly.Load(assemblyName),
                                    type => !type.IsAbstract && type.IsPublic
                                    && type.IsInterface && !type.IsGenericType && type.GetConstructor(Type.EmptyTypes) != null
                                    && !typeof(System.ComponentModel.Component).IsAssignableFrom(type)
                                    && !typeof(Attribute).IsAssignableFrom(type)
                                    && type.CustomAttributes.Where(attribute => attribute.AttributeType == typeof(DomainComponentAttribute)).Count() == 1);

                        domainComponents
                            .ToList()
                            .ForEach(entity => XafTypesInfo.Instance.RegisterSharedPart(entity));

                    }

                });

        }
    }
}
