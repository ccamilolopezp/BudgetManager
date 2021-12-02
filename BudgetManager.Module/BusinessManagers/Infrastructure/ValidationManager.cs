using BudgetManager.Module.BusinessObjects;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BudgetManager.Module.BusinessManagers
{
    public class ValidationManager
    {
        public void ValidateLoadBudgetTreeParameters(Budget.Model.Sql.Company company,
                                                     BusinessObjects.NonPersistent.LoadBudgetTreeParameters parameters)
        {
            if (company == null) throw new Exception("Debe seleccionar una compañia válida.");
            if (company.Name == null) throw new Exception("La compañia seleccionada no posee un Nombre");

            if (parameters.LoadParameters == null) throw new Exception("Debe seleccionar un esquema de parametros de carga.");
            if (parameters.LoadParameters.LastColumnWithAccount < 1) throw new Exception("No Es posible asociar un número menor a 1 en el campo última columna con número de cuenta, de los parametros de carga");
            if (parameters.LoadParameters.TreeName == null) throw new Exception("Error en los parametros de carga, Debe seleccionar un Nombre Del árbol que desea procesar");
            if (string.IsNullOrEmpty(parameters.LoadParameters.TreeName.Name)) throw new Exception("El nombre del árbol que desea procesar está vacío");

            if (parameters.BudgetData == null) throw new Exception($"Debe seleccionar un archivo \".txt\" válido para la Estructura del Árbol {parameters.LoadParameters.TreeName.Name}");
            if (parameters.BudgetData.Content.Count() == 0) throw new Exception("El archivo seleccionado para la Estructura del Árbol está vacio");
            if (string.IsNullOrEmpty(parameters.LoadParameters.CultureInfo)) throw new Exception("La referencia cultural, no puede estar vacía, revise los parametros de carga.");
        }

        public void ValidateLoadQuotedParameters(Budget.Model.Sql.Company company,
                                                 BusinessObjects.NonPersistent.LoadBudgetValuesParameters parameters)
        {
            if (company == null) throw new Exception("Debe seleccionar una compañia válida.");
            if (company.Name == null) throw new Exception("La compañia seleccionada no posee un Nombre");
            if (company.AccountingTrees == null) throw new Exception("No hay árboles asociados a la unidad de jerarquia de la compañia.");
            if (company.AccountingTrees.Count() == 0) throw new Exception("No hay árboles asociados a la unidad de jerarquia de la compañia.");

            if (parameters.LoadParameters == null) throw new Exception("Debe seleccionar un esquema de parametros de carga.");
            if (parameters.LoadParameters.TreeName == null) throw new Exception("Error en los parametros de carga, Debe seleccionar un Nombre Del árbol que desea procesar");
            if (string.IsNullOrEmpty(parameters.LoadParameters.TreeName.Name)) throw new Exception("El nombre del árbol que desea procesar está vacío");
            if (parameters.LoadParameters.TreeName.Tree == null) throw new Exception("No existe estructura para el arbol que intenta procesar");
            if (company.AccountingTrees.FirstOrDefault(tree => tree.Id == parameters.LoadParameters.TreeName.Tree.Id) == null) throw new Exception("El arbol seleccionado no esta asociado a la unidad de Jerarquia de la compañia");

            if (parameters.BudgetData == null) throw new Exception($"Debe seleccionar un archivo \".txt\" válido, para cargar valores al Árbol {parameters.LoadParameters.TreeName.Name}");
            if (parameters.BudgetData.Content.Count() == 0) throw new Exception("El archivo seleccionado para la Estructura del Árbol está vacio");

            if (parameters.LoadParameters.LastColumnWithAccount < 1) throw new Exception("No Es posible asociar un número menor a 1 en el campo última columna con número de cuenta, de los parametros de carga");

            if (parameters.InitialDate == DateTime.MinValue || parameters.FinalDate == DateTime.MinValue) throw new Exception("Debe seleccionar una fecha válida");
            if (parameters.InitialDate > parameters.FinalDate) throw new Exception("La Fecha inicial no puede ser mayor a la fecha final");
            if (company.BudgetPeriodicity == null) throw new Exception("la compañia seleccionada no tiene periodicidad definida");
            if (company.BudgetPeriodicity.BudgetPeriods == null) throw new Exception("Error interno contacte al administrador");
            if (company.BudgetPeriodicity.BudgetPeriods.Count() == 0) throw new Exception("Error interno contacte al administrador");

            var periods = company.BudgetPeriodicity.BudgetPeriods.Select(a => a.InitialMonth);

            if (!periods.Contains(parameters.InitialDate.Month)) throw new Exception("la fecha inicial seleccionada no se puede asociar a la periodicidad definida para la compañia");
            if (!periods.Contains(parameters.FinalDate.Month)) throw new Exception("la fecha final seleccionada no se puede asociar a la periodicidad definida para la compañia");

        }

        public void ValidateLoadExecutedParameters(Budget.Model.Sql.Company company,
                                                   BusinessObjects.NonPersistent.LoadExecutedBudgetValuesParameters parameters)
        {
            if (company == null) throw new Exception("Debe seleccionar una compañia válida.");
            if (company.Name == null) throw new Exception("La compañia seleccionada no posee un Nombre");
            if (company.AccountingTrees == null) throw new Exception("No hay árboles asociados a la unidad de jerarquia de la compañia.");
            if (company.AccountingTrees.Count() == 0) throw new Exception("No hay árboles asociados a la unidad de jerarquia de la compañia.");

            if (parameters.LoadParameters == null) throw new Exception("Debe seleccionar un esquema de parametros de carga.");
            if (parameters.LoadParameters.TreeName == null) throw new Exception("Error en los parametros de carga, Debe seleccionar un Nombre Del árbol que desea procesar");

            if (string.IsNullOrEmpty(parameters.LoadParameters.TreeName.Name)) throw new Exception("El nombre del árbol que desea procesar está vacío");
            if (parameters.LoadParameters.TreeName.Tree == null) throw new Exception("No existe estructura para el arbol que intenta procesar");
            if (company.AccountingTrees.FirstOrDefault(tree => tree.Id == parameters.LoadParameters.TreeName.Tree.Id) == null) throw new Exception("El arbol seleccionado no esta asociado a la unidad de Jerarquia de la compañia");

            if (parameters.BudgetData == null) throw new Exception($"Debe seleccionar un archivo \".txt\" válido, para cargar valores al Árbol {parameters.LoadParameters.TreeName.Name}");
            if (parameters.BudgetData.Content.Count() == 0) throw new Exception("El archivo seleccionado para la Estructura del Árbol está vacio");

            if (parameters.LoadParameters.LastColumnWithAccount < 1) throw new Exception("No Es posible asociar un número menor a 1 en el campo última columna con número de cuenta, de los parametros de carga");

            if (parameters.ProcessDate == DateTime.MinValue) throw new Exception("Debe seleccionar una fecha válida");
            if (company.BudgetPeriodicity == null) throw new Exception("la compañia seleccionada no tiene periodicidad definida");
            if (company.BudgetPeriodicity.BudgetPeriods == null) throw new Exception("Error interno contacte al administrador");
            if (company.BudgetPeriodicity.BudgetPeriods.Count() == 0) throw new Exception("Error interno contacte al administrador");

            var periods = company.BudgetPeriodicity.BudgetPeriods.Select(a => a.InitialMonth);

            if (!periods.Contains(parameters.ProcessDate.Month)) throw new Exception("la fecha de proceso seleccionada no se puede asociar a la periodicidad definida para la compañia");
        }

        public void ValidateStoredBudgetUnit(BusinessObjects.BudgetManagerDbContext dbContext,
                                             IEnumerable<DateTime> dates,
                                             Budget.Model.Sql.Company company)
        {
            var years = dates.GroupBy(date => date.Year)
                             .Select(dateByYear => dateByYear.Key)
                             .ToList();

            var budgetUnitStored = dbContext.Set<Budget.Model.Sql.BudgetUnit>()
                                            .FirstOrDefault(budgetUnit => budgetUnit.Company.Id == company.Id && years.Contains(budgetUnit.Year));

            if (budgetUnitStored != null) throw new Exception($"ya existe un presupuesto cargado para la compañia {company.Name}, para el año {budgetUnitStored.Year}");
        }

        public void ValidateLinkExecutedBudgetParameters(BusinessObjects.NonPersistent.LinkValuesParameters parameters)
        {
            if (parameters.BudgetUnit == null) throw new Exception("Debe seleccionar una Unidad de Presupuesto válida");

            if (parameters.ExecutedBudgetUnit == null) throw new Exception("Debe seleccionar una Unidad de Presupuesto Ejecutado válida");

            if (parameters.BudgetUnit.Year != parameters.ExecutedBudgetUnit.Year) throw new Exception($"El año de la Unidad de Presupuesto seleccionada: {parameters.BudgetUnit.Year} y el año de la" +
                                                                                                      $" Unidad de Presupuesto Ejecutado: {parameters.ExecutedBudgetUnit.Year} no coinciden");
        }

        public void ValidateDeleteParameters(BusinessObjects.NonPersistent.DeleteParameters parameters)
        {
            if (parameters.BudgetUnitsToDelete == null && parameters.ExecutedBudgetUnitsToDelete == null) throw new Exception("Debe seleccionar al menos una Unidad de Presupuesto válida");
            if (parameters.BudgetUnitsToDelete.Count() == 0 && parameters.ExecutedBudgetUnitsToDelete.Count() == 0) throw new Exception("Debe seleccionar al menos una Unidad de Presupuesto válida");
        }

        public void ValidateAccountValues(Account account)
        {
            if (!account.Values.ContainsKey(Budget.Model.TransactionType.Executed)) throw AccountExceptionBuilder(account, "no tiene Valores Válidos");
            if (account.Values[Budget.Model.TransactionType.Executed].Count() != 2) throw AccountExceptionBuilder(account, "no tiene valores suficientes para asociar el saldo inicial y el final");
            if (!account.Values.ContainsKey(Budget.Model.TransactionType.Credit)) throw AccountExceptionBuilder(account, "no tiene valores para el crédito de la cuenta,");
            if (account.Values[Budget.Model.TransactionType.Credit].Count() != 1) throw AccountExceptionBuilder(account, $"solo puede haber 1 valor para el crédito de la cuenta, y existen {account.Values[Budget.Model.TransactionType.Credit].Count()}");
            if (!account.Values.ContainsKey(Budget.Model.TransactionType.Debit)) throw AccountExceptionBuilder(account, "no tiene valores para el débito de la cuenta");
            if (account.Values[Budget.Model.TransactionType.Debit].Count() != 1) throw AccountExceptionBuilder(account, $"solo puede haber 1 valor para el crédito de la cuenta, y existen {account.Values[Budget.Model.TransactionType.Debit].Count()}");
        }

        public Func<Account, string, Exception> AccountExceptionBuilder =
            (account, message) => new Exception($"Error en la linea {account.Line} la cuenta {account.Name} - {account.Label} - {account.ProcessDate.ToShortDateString()}," +
                                                $" {message}, verifique su archivo \".txt\"");
    }

}