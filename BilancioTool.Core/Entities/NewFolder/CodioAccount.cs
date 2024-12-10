using System;
using System.Collections.Generic;
using System.Text;

namespace BilancioTool.Core.Entities.NewFolder
{
    public class AccountDtoRead : BaseAuditableDto
    {
        public string Name { get; set; } = String.Empty;
        public string? BalanceSheetType { get; set; }
        public string? CashFlowType { get; set; }
        public bool IsActive { get; set; }

    }

    public class AccountDtoWrite
    {
        public string Name { get; set; } = String.Empty;
        public string? BalanceSheetType { get; set; }
        public string? CashFlowType { get; set; }
        public bool IsActive { get; set; }
    }
}
