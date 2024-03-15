namespace BilancioTool.Core.Entities
{
    public class ForecastDefinition
    {
        public string PartID { get; set; }

        public string Year { get; set; }
        public string Month { get; set; }
        public string Day { get; set; }

        public string DayOfWeek { get; set; }

        public string Account { get; set; }

        public double Inflow { get; set; }
        public double Outflow { get; set; }
    }
}
