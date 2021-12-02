using BudgetManager.Module.BusinessObjects;
using Finac.ExpressApp.Utils.Extensions;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace BudgetManager.Module.BusinessManagers
{
    public class AccountManager
    {
        public IEnumerable<Account> GetAccounts(DevExpress.Persistent.BaseImpl.EF.FileData fileData,
                                                Budget.Model.Sql.LoadParameters loadParameters,
                                                bool executed = false)
        {

            Dictionary<string, Account> createdAccounts = new Dictionary<string, Account>();
            var cultureInfo = Engine.Common.Utilities.CultureInfo.GetCultureInfo(loadParameters.CultureInfo);
            if (!loadParameters.IncludeAccount || !loadParameters.IncludeAccountName) throw new Exception("No es posible procesar un archivo que no incluya cuenta o nombre, verifique los parametros de carga");

            return fileData.ToStringArray()
                           .Select((record, line) =>
                           {
                               var array = record.Split('\t');

                               if (array.Count() != loadParameters.MaxColumns()) throw new Exception($"La linea {line + 1} del archivo contiene errores se esperaban " +
                                                                                                     $"{loadParameters.MaxColumns()} columnas, y se tienen {array.Count()}");

                               var accountCode = GetAccountCode(line,
                                                                array,
                                                                loadParameters);

                               var account = new Account(line + 1,
                                                         accountCode,
                                                         array[loadParameters.AccountNameColumn - 1],
                                                         loadParameters.IncludeValues ? GetValues(line,
                                                                                                  array,
                                                                                                  loadParameters,
                                                                                                  cultureInfo,
                                                                                                  executed) : new Dictionary<Budget.Model.TransactionType, IEnumerable<double>>(),
                                                         loadParameters.IncludeProcessDate ? GetProcessDate(line,
                                                                                                            array[loadParameters.ProcessDateColumn - 1],
                                                                                                            cultureInfo) : new DateTime(),
                                                        loadParameters.IncludeNit ? GetNit(line,
                                                                                           array[loadParameters.NitColumn - 1],
                                                                                           cultureInfo) : string.Empty);
                               return account;
                           });
        }

        public string GetAccountCode(int line,
                                     string[] array,
                                     Budget.Model.Sql.LoadParameters loadParameters)
        {
            string accountCode;

            if (array[loadParameters.AccountColumnStart - 1] == "0") throw new Exception($"Error en la linea {line + 1} No existe cuenta {array[0]}");

            var accountCodeArray = array.Skip(loadParameters.AccountColumnStart - 1)
                                        .Take(loadParameters.LastColumnWithAccount)
                                        .ToList();

            if (accountCodeArray.TrueForAll(_char => _char == string.Empty)) throw new Exception($"La linea {line + 1} del archivo no tiene número de cuenta.");
            if (array[loadParameters.AccountNameColumn - 1] == string.Empty) throw new Exception($"La linea {line + 1} del archivo no tiene nombre de cuenta, verifique el archivo o el parámetro \"Columna del Nombre de la Cuenta\"");
            if (!accountCodeArray.TrueForAll(_char => Regex.Match(_char, "^[0-9]*$").Success)) throw new Exception($"La cuenta en la fila {line + 1}, {string.Join(" ", accountCodeArray)} no es un número de cuenta válido.");
            if (accountCodeArray.FirstOrDefault(_char => _char.Length > 1 && _char.Length % 2 != 0) != null) throw new Exception($"La linea {line + 1} del archivo, con el texto {string.Join(" ", accountCodeArray)} no tiene número de cuenta valido. ");

            if (loadParameters.TotalAccountColumns == 1)
            {
                accountCode = string.Join("", Regex.Split(accountCodeArray[0], @"(\d{2}).*?")
                                            .Reverse()
                                            .Skip(1)
                                            .Reverse()
                                            .Select((_string, _index) =>
                                            {
                                                if (_index == 1) return string.Join("-", _string.Select(_char =>
                                                                                                {
                                                                                                    if (_char == '0')
                                                                                                        throw new Exception($"Error en la linea {line + 1} No existe cuenta {accountCodeArray[_index]}");
                                                                                                    return _char;
                                                                                                }));
                                                if (_string == string.Empty) return "-";
                                                if (_string == "00") throw new Exception($"La linea {line + 1}, contiene una cuenta con 00.");
                                                return _string;
                                            }));

            }

            accountCode = $"-{string.Join("-", accountCodeArray.Where(a => !string.IsNullOrEmpty(a)))}";

            if (accountCode == string.Empty) accountCode = $"-{array[loadParameters.AccountColumnStart - 1]}";

            return accountCode;
        }

        public Dictionary<Budget.Model.TransactionType, IEnumerable<double>> GetValues(int line,
                                                                                             string[] array,
                                                                                             Budget.Model.Sql.LoadParameters loadParameters,
                                                                                             CultureInfo cultureInfo,
                                                                                             bool executed)
        {
            return array.Skip(loadParameters.ValueColumnStart - 1)
                        .Take(loadParameters.TotalValueColumns)
                        .Select((number, index) =>
                        {
                            if (number == string.Empty)
                                return new
                                {
                                    Index = index,
                                    Amount = 0.0,
                                    TransactionType = executed ? Budget.Model.TransactionType.Executed : Budget.Model.TransactionType.Quoted
                                };

                            var value = PrimitivesTypes.convertStringToDouble(cultureInfo, number);
                            if (!value.Item1) throw new Exception($"Error en la linea {line + 1} no es posible convertir {number} a un formato numérico válido, en la referencia {cultureInfo.Name} " +
                                                                  $"Verifique los parametros de carga");
                            if (loadParameters.IncludeDebitTransaction && index == loadParameters.DebitTransactionColumn - loadParameters.ValueColumnStart)
                                return new
                                {
                                    Index = index,
                                    Amount = value.Item2,
                                    TransactionType = Budget.Model.TransactionType.Debit
                                };

                            if (loadParameters.IncludeCreditTransaction && index == loadParameters.CreditTransactionColumn - loadParameters.ValueColumnStart)
                                return new
                                {
                                    Index = index,
                                    Amount = value.Item2,
                                    TransactionType = Budget.Model.TransactionType.Credit
                                };

                            return new
                            {
                                Index = index,
                                Amount = value.Item2,
                                TransactionType = executed ? Budget.Model.TransactionType.Executed : Budget.Model.TransactionType.Quoted
                            };
                        })
                        .GroupBy(value => value.TransactionType)
                        .ToDictionary(groupByTransaction => groupByTransaction.Key,
                                      groupByTransaction => groupByTransaction.OrderBy(value => value.Index)
                                                                              .Select(value => value.Amount));
        }

        public DateTime GetProcessDate(int line,
                                       string _date,
                                       CultureInfo cultureInfo)
        {
            var date = PrimitivesTypes.convertStringToDate(cultureInfo, _date);
            if (!date.Item1) throw new Exception($"error en la linea {line + 1} no es posible convertir {_date}, " +
                                                 $"a una fecha válida, verifique si la fecha está incluida en el archivo o si el parametro \"incluir fecha de proceso está activo\"");
            return date.Item2;
        }

        public string GetNit(int line,
                             string _nit,
                             CultureInfo cultureInfo)
        {
            if (string.IsNullOrEmpty(_nit)) return _nit;
            return string.Join("", _nit.Trim()
                                       .Select(_char =>
                                       {
                                           if (!Regex.Match(_char.ToString(), "^[0-9]*$").Success) throw new Exception($" Error en la linea {line + 1} No es posible convertir {_nit}  a una identificación Válida");
                                           return _char;
                                       }));
        }

    }
}