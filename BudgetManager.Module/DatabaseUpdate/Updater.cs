using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.Updating;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl.EF;
using DevExpress.Persistent.BaseImpl.EF.PermissionPolicy;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace BudgetManager.Module.DatabaseUpdate
{
    public class Updater : ModuleUpdater
    {
        public Updater(IObjectSpace objectSpace, Version currentDBVersion) :
            base(objectSpace, currentDBVersion)
        {
        }
        public override void UpdateDatabaseAfterUpdateSchema()
        {
            base.UpdateDatabaseAfterUpdateSchema();                       

            PermissionPolicyUser userAdmin = ObjectSpace.FindObject<PermissionPolicyUser>(new BinaryOperator("UserName", "Admin"));
            if (userAdmin == null)
            {
                userAdmin = ObjectSpace.CreateObject<PermissionPolicyUser>();
                userAdmin.UserName = "Admin";
                // Set a password if the standard authentication type is used
                userAdmin.SetPassword("");
            }
            // If a role with the Administrators name doesn't exist in the database, create this role
            PermissionPolicyRole adminRole = ObjectSpace.FindObject<PermissionPolicyRole>(new BinaryOperator("Name", "Administrators"));
            if (adminRole == null)
            {
                adminRole = ObjectSpace.CreateObject<PermissionPolicyRole>();
                adminRole.Name = "Administrators";
            }
            adminRole.IsAdministrative = true;
            userAdmin.Roles.Add(adminRole);
            ObjectSpace.CommitChanges(); //This line persists created object(s).
            SetUpBudgetPeriodicities();
            ObjectSpace.CommitChanges();
            SetUpBudgetPeriods();
            ObjectSpace.CommitChanges();
            SetUpControllersRoles();
            ObjectSpace.CommitChanges();
        }
        public override void UpdateDatabaseBeforeUpdateSchema()
        {
            base.UpdateDatabaseBeforeUpdateSchema();
        }        

        #region BudgetPeriodicities
        private void SetUpBudgetPeriodicities()
        {
            List<Tuple<string, int>> budgetPeriodicity = new List<Tuple<string, int>>()
            {
                new Tuple<string, int>("Mensual",1),
                new Tuple<string, int>("Bimensual",2),
                new Tuple<string, int>("Trimestral",3),
                new Tuple<string, int>("Cuatrimestral",4),
                new Tuple<string, int>("Semestral",6),
                new Tuple<string, int>("Anual",12),
            };

            Hashtable _BudgetPeriodicity = new Hashtable();
            (from _budgetPeriodicity in ObjectSpace.GetObjectsQuery<Budget.Model.Sql.BudgetPeriodicity>()
             select _budgetPeriodicity).ToList().ForEach(_budgetPeriodicity =>
             {
                 if (!_BudgetPeriodicity.ContainsKey(_budgetPeriodicity.Name)) _BudgetPeriodicity.Add(_budgetPeriodicity.Name, _budgetPeriodicity);
             });

            foreach (var _budgetPeiodicity in budgetPeriodicity)
            {
                if (!_BudgetPeriodicity.ContainsKey(_budgetPeiodicity.Item1))
                {
                    Budget.Model.Sql.BudgetPeriodicity BudgetPeriodicity = ObjectSpace.CreateObject<Budget.Model.Sql.BudgetPeriodicity>();
                    BudgetPeriodicity.Name = _budgetPeiodicity.Item1;
                    BudgetPeriodicity.PeriodInMonths = _budgetPeiodicity.Item2;
                }
            }
        }
        #endregion

        #region BudgetPeriods
        private void SetUpBudgetPeriods()
        {
            List<Tuple<string, int, string, int, int>> budgetPeriods = new List<Tuple<string, int, string, int, int>>()
            {
                new Tuple<string, int, string, int, int>("Mensual", 1, "Enero", 1, 1),
                new Tuple<string, int, string, int, int>("Mensual", 2, "Febrero", 2, 2),
                new Tuple<string, int, string, int, int>("Mensual", 3, "Marzo", 3, 3),
                new Tuple<string, int, string, int, int>("Mensual", 4, "Abril", 4, 4),
                new Tuple<string, int, string, int, int>("Mensual", 5, "Mayo", 5, 5),
                new Tuple<string, int, string, int, int>("Mensual", 6, "Junio", 6, 6),
                new Tuple<string, int, string, int, int>("Mensual", 7, "Julio", 7, 7),
                new Tuple<string, int, string, int, int>("Mensual", 8, "Agosto", 8, 8),
                new Tuple<string, int, string, int, int>("Mensual", 9 ,"Septiembre", 9, 9),
                new Tuple<string, int, string, int, int>("Mensual", 10, "Octubre", 10, 10),
                new Tuple<string, int, string, int, int>("Mensual", 11, "Noviembre", 11, 11),
                new Tuple<string, int, string, int, int>("Mensual", 12, "Diciembre", 12, 12),
                new Tuple<string, int, string, int, int>("Bimensual", 1, "Bimestre", 1, 2),
                new Tuple<string, int, string, int, int>("Bimensual", 2, "Bimestre", 3, 4),
                new Tuple<string, int, string, int, int>("Bimensual", 3, "Bimestre", 5, 6),
                new Tuple<string, int, string, int, int>("Bimensual", 4, "Bimestre", 7, 8),
                new Tuple<string, int, string, int, int>("Bimensual", 5, "Bimestre", 9, 10),
                new Tuple<string, int, string, int, int>("Bimensual", 6, "Bimestre", 11, 12),
                new Tuple<string, int, string, int, int>("Trimestral", 1, "Trimestre", 1, 3),
                new Tuple<string, int, string, int, int>("Trimestral", 2, "Trimestre", 4, 6),
                new Tuple<string, int, string, int, int>("Trimestral", 3, "Trimestre", 7, 9),
                new Tuple<string, int, string, int, int>("Trimestral", 4, "Trimestre", 10, 12),
                new Tuple<string, int, string, int, int>("Cuatrimestral", 1, "Cuatrimestre", 1, 4),
                new Tuple<string, int, string, int, int>("Cuatrimestral", 2, "Cuatrimestre", 5, 8),
                new Tuple<string, int, string, int, int>("Cuatrimestral", 3, "Cuatrimestre", 9, 12),
                new Tuple<string, int, string, int, int>("Semestral", 1, "Semestre", 1, 6),
                new Tuple<string, int, string, int, int>("Semestral", 2, "Semestre", 6, 12),
                new Tuple<string, int, string, int, int>("Anual", 1, "Año", 1, 12)
            };
            var budgetPeriodicities = (from budgetPeriodicity in ObjectSpace.GetObjectsQuery<Budget.Model.Sql.BudgetPeriodicity>()
                                       select budgetPeriodicity).ToDictionary(budgetPeriodicity => budgetPeriodicity.Name,
                                                                              budgetPeriodicity => budgetPeriodicity);
            Hashtable BudgetPeriods = new Hashtable();
            (from account in ObjectSpace.GetObjectsQuery<Budget.Model.Sql.BudgetPeriod>()
             select account).ToList().ForEach(budgetPeriod =>
             {
                 if (!BudgetPeriods.ContainsKey(budgetPeriod.Name)) BudgetPeriods.Add(budgetPeriod.Name, budgetPeriod);
             });

            foreach (var _negativeAccount in budgetPeriods)
            {
                var budgetPeriodName = _negativeAccount.Item2.ToString().Length == 1 ?
                    $"0{_negativeAccount.Item2}-{_negativeAccount.Item3}" : $"{_negativeAccount.Item2}-{_negativeAccount.Item3}";
                if (!BudgetPeriods.ContainsKey(budgetPeriodName))
                {
                    Budget.Model.Sql.BudgetPeriod NegativeAccount = ObjectSpace.CreateObject<Budget.Model.Sql.BudgetPeriod>();
                    NegativeAccount.BudgetPeriodicity = budgetPeriodicities[_negativeAccount.Item1];
                    NegativeAccount.Consecutive = _negativeAccount.Item2;
                    NegativeAccount.Name = budgetPeriodName;
                    NegativeAccount.InitialMonth = _negativeAccount.Item4;
                    NegativeAccount.FinalMonth = _negativeAccount.Item5;
                }
            }
        }
        #endregion

        #region Controllers Roles

        private void SetUpControllersRoles()
        {
            Hashtable PermissionPolicyRoles = new Hashtable();
            ObjectSpace.GetObjectsQuery<PermissionPolicyRole>()
                       .ToList()
                       .ForEach(permissionPolicyRole =>
                       {
                           if (!PermissionPolicyRoles.ContainsKey(permissionPolicyRole.Name))
                               PermissionPolicyRoles.Add(permissionPolicyRole.Name, permissionPolicyRole);
                       });

            (new List<string>()
            {
                "Load Budget Tree",
                "Link Budget Values",
                "Delete Values",
                "Load Budget",
                "Load Executed Budget"
            }).ForEach(_permissionPolicyRole =>
            {
                if (PermissionPolicyRoles.ContainsKey(_permissionPolicyRole)) return;

                PermissionPolicyRole PermissionPolicyRole = ObjectSpace.CreateObject<PermissionPolicyRole>();
                PermissionPolicyRole.Name = _permissionPolicyRole;
            });
        }
        #endregion

    }
}
