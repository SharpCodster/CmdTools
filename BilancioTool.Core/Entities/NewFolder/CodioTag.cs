using System;
using System.Collections.Generic;
using System.Text;

namespace BilancioTool.Core.Entities.NewFolder
{
    public class TagDtoRead : BaseAuditableDto
    {
        public string Name { get; set; } = String.Empty;
    }

    public class TagDtoWrite
    {
        public string Name { get; set; } = String.Empty;
    }
}
