using System;
using System.Collections.Generic;
using System.Text;

namespace BilancioTool.Core.Entities
{
    public class ForecastDefinition
    {
        public string PartID { get; set; }

        public string Year { get; set; }
        public string Month { get; set; }
        public string Day { get; set; }

        public string DayOfWeek { get; set; }

        public string AccountFrom { get; set; }
        public string AccountTo { get; set; }

        public double Total { get; set; }
    }
}
