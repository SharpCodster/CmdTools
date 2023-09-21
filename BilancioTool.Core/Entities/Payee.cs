using System;
using System.Collections.Generic;
using System.Text;

namespace BilancioTool.Core.Entities
{
    public class Payee
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int ExcelRow { get; set; }
    }
}
