using System;
using System.Collections.Generic;
using System.Text;

namespace BilancioTool.Core.Entities
{
    public class Movimento
    {
        public DateTime DataRegistrazione { get; set; }
        public DateTime DataValuta { get; set; }

        public string Descrizione { get; set; }
        public double Importo { get; set; }


        public string Account { get; set; }


        public double Inflow
        {
            get
            {
                double i = 0.0;
                if (Importo > 0.0)
                {
                    i = Importo;
                }
                return i;
            }
        }

        public double Outflow
        {
            get
            {
                double i = 0.0;
                if (Importo < 0.0)
                {
                    i = -Importo;
                }
                return i;
            }
        }
    }
}
