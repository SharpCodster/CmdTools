using System;
using System.Collections.Generic;
using System.Text;

namespace BilancioTool.Core.Entities.NewFolder
{
    public class PayeeDtoRead : BaseAuditableDto
    {
        public string Name { get; set; } = String.Empty;
    }

    public class PayeeDtoWrite
    {
        public string Name { get; set; } = String.Empty;
    }
}
