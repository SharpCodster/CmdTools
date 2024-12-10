using System;
using System.Collections.Generic;
using System.Text;

namespace BilancioTool.Core.Entities.NewFolder
{
    public class RecordDtoRead : BaseAuditableDto
    {
        public string TransactionId { get; set; } = Guid.Empty.ToString();
        public string AccountId { get; set; } = Guid.Empty.ToString();

        public decimal Inflow { get; set; }
        public decimal Outflow { get; set; }
        public decimal Total { get; set; }
        public string? Notes { get; set; }

    }

    public class RecordDtoWrite
    {
        public string Account { get; set; } = String.Empty;
        public decimal Inflow { get; set; }
        public decimal Outflow { get; set; }
        public string? Notes { get; set; }
    }
}
