using System;
using System.Collections.Generic;
using System.Text;

namespace BilancioTool.Core.Entities.NewFolder
{
    public class LedgerRecordDto
    {
        public string? ExcelId { get; set; } = null;
        public DateTime Date { get; set; } = DateTime.UtcNow;
        public string AccountName { get; set; } = "Account";

        public decimal Inflow { get; set; } = 0.0M;
        public decimal Outflow { get; set; } = 0.0M;

        public string PayeeName { get; set; } = "Payee";

        public string? Notes { get; set; } = null;

        public IEnumerable<string> Tags { get; set; } = new List<string>();

        public string TransactionId { get; set; } = Guid.NewGuid().ToString();
        public string AccountId { get; set; } = Guid.NewGuid().ToString();
        public string? PayeeId { get; set; } = null;
    }
}
