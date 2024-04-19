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

        public bool Autobalance { get; set; } = false;
        public string AccountTo { get; set; }

        public string Payee { get; set; }
        public string Tags { get; set; }
        public string Notes { get; set; }
    }
}
