using System;
using System.Collections.Generic;
using System.Text;

namespace BilancioTool.Core.Entities
{
    public class Account
    {
        public int Order { get; set; }
        public string Name { get; set; }

        public string BalanceSheetClass { get; set; }
        public string CashFlowClass { get; set; }

        public int ExcelRow { get; set; }
    }
}
