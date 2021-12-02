using DevExpress.ExpressApp.DC;
using System;
using System.Collections.Generic;

namespace BudgetManager.Module.BusinessObjects
{
    [DomainComponent]
    public class Account
    {
        public Account(int line,
                       string name,
                       string label,
                       Dictionary<Budget.Model.TransactionType,IEnumerable<double>> values,
                       DateTime processDate,
                       string nit)
        {
            if (string.IsNullOrEmpty(name)) throw new ArgumentException("Se esta Realizando una instancia, entregando un parametro vacío o nulo", "Code");
            if (string.IsNullOrEmpty(label)) throw new ArgumentException("Se esta Realizando una instancia, entregando un parametro vacío o nulo", "Label");
            Line = line;
            Name = name;
            Label = label;
            Values = values;
            ProcessDate = processDate;
            Nit = nit;
        }

        public int Line { get; set; }
        public string Label { get; set; }
        public string Name { get; set; }
        public Dictionary<Budget.Model.TransactionType, IEnumerable<double>> Values { get; set; }
        public DateTime ProcessDate { get; set; }
        public string Nit { get; set; }
    }

}