using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CmdTools.Core.Helpers
{
    public static class PseudoCronEvaluetor
    {
        public static bool CheckExpresison(string year, string month, string day, string dayOfWeek, DateTime currentDate)
        {
            return CheckYear(year, currentDate)
                        && CheckMonth(month, currentDate)
                        && CheckDay(day, currentDate)
                        && CheckDayOfWeek(dayOfWeek, currentDate);
        }


        private static bool CheckYear(string year, DateTime currentDate)
        {
            return CheckInteger(year.Replace(" ", ""), currentDate.Year);
        }

        private static bool CheckMonth(string month, DateTime currentDate)
        {
            return CheckInteger(month.Replace(" ", ""), currentDate.Month);
        }

        private static bool CheckDay(string day, DateTime currentDate)
        {
            string cleanDay = day.Replace(" ", "");
            if (CheckInteger(cleanDay, currentDate.Day))
            {
                return true;
            }
            else if (cleanDay == "L")
            {
                var lastDay = DateTime.DaysInMonth(currentDate.Year, currentDate.Month);

                if (currentDate.Day == lastDay)
                {
                    return true;
                }
            }

            return false;
        }

        private static bool CheckDayOfWeek(string dayOW, DateTime currentDate)
        {
            return CheckInteger(dayOW.Replace(" ", ""), (int)currentDate.DayOfWeek);
        }

        private static bool CheckInteger(string reference, int comparer)
        {
            int numeroConvertito;
            bool conversioneRiuscita = int.TryParse(reference, out numeroConvertito);

            if (reference == "*")
            {
                return true;
            }
            else if (reference.Contains("-"))
            {
                var splitted = reference.Split('-');
                if (splitted.Length == 2)
                {
                    var ordered = splitted.Select(_ => Convert.ToInt32(_)).OrderBy(x => x).ToList();
                    var min = ordered[0];
                    var max = ordered[1];

                    if (comparer >= min && comparer <= max)
                    {
                        return true;
                    }
                }
                else
                {
                    throw new Exception($"Input string error. Value: {reference}");
                }
            }
            else if (reference.Contains("/"))
            {
                var splitted = reference.Split('/');
                if (splitted.Length == 2)
                {
                    var startValue = Convert.ToInt32(splitted[0]);
                    var periodicity = Convert.ToInt32(splitted[1]);

                    while (comparer >= startValue)
                    {
                        if (comparer == startValue)
                        {
                            return true;
                        }

                        startValue = startValue + periodicity;
                    }
                }
                else
                {
                    throw new Exception($"Input string error. Value: {reference}");
                }
            }
            else if (numeroConvertito == comparer)
            {
                return true;
            }

            return false;
        }
    }
}
