using System;
using System.Collections.Generic;
using System.Text;

namespace BilancioTool.Core.Entities
{
    public class TransactionV4
    {
        public string Id { get; set; }
        public DateTime Date { get; set; }
        public string Account { get; set; }
        public double Inflow { get; set; }
        public double Outflow { get; set; }
        public string Payee { get; set; }
        public string Notes { get; set; }
        public string Tags { get; set; }

        public bool HasChanges { get; set; } = false;
        public bool IsNew { get; set; } = false;
        public int ExcelRow { get; set; }
    }
}
