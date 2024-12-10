using System;
using System.Collections.Generic;
using System.Text;

namespace BilancioTool.Core.Entities.NewFolder
{
    public class TransactionDtoRead : BaseAuditableDto
    {
        public DateTime ValueDate { get; set; }
        public string? ExcelId { get; set; }

        public PayeeDtoRead? Payee { get; set; }
        public virtual ICollection<RecordDtoRead>? Records { get; set; } = new List<RecordDtoRead>();
        public virtual ICollection<TagDtoRead>? Tags { get; set; } = new List<TagDtoRead>();
    }



    public class TransactionDtoCreate
    {
        public TransactionDtoCreate()
        {
            Tags = new List<string>();
            Records = new List<RecordDtoWrite>();
        }

        public DateTime Date { get; set; }
        public string? Payee { get; set; }
        public string? ExcelId { get; set; }

        public ICollection<string> Tags { get; set; }

        public ICollection<RecordDtoWrite> Records { get; set; }
    }

    public class TransactionDtoWrite
    {
        public TransactionDtoWrite()
        {
            //TagIds = new List<string>();
        }

        public DateTime ValueDate { get; set; } // data valuta
        public string? PayeeId { get; set; }
        public bool IsClosed { get; set; }
        //public ICollection<string>? TagIds { get; set; }
        //public virtual ICollection<SplitDtoWrite>? Splits { get; set; }
    }
}
