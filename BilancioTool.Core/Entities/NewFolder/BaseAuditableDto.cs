using System;
using System.Collections.Generic;
using System.Text;

namespace BilancioTool.Core.Entities.NewFolder
{
    public class BaseAuditableDto : BaseDto
    {
        public bool IsDeleted { get; set; }
        public DateTime CreatedAtUtc { get; set; }
        public string CreatedBy { get; set; }
        public DateTime UpdatedAtUtc { get; set; }
        public string UpdatedBy { get; set; }
    }
}
