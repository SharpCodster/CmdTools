using BilancioTool.Core.Entities;
using BilancioTool.Core.Tables;
using CmdTools.Core.CmdMenuAndPages;
using CmdTools.Core.UserSettings;
using OfficeOpenXml;
using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BilancioTool
{
    public class GenerateForecastPage : Page
    {
        public GenerateForecastPage(ICmdWizard program, IIOWrapper ioWrapper)
            : base("Generating forecast", program, ioWrapper)
        {
        }

        protected override void DisplayContent()
        {
            UserSettingsManager settingManager = new UserSettingsManager();
            var settings = settingManager.ReadFromConfigFile();

            if (!IOWrapper.GetConfirmation($"Confirm forecast folder at [green]{settings.BilancioSettings.ForecastFolder}[/]?"))
            {
                settings.BilancioSettings.ForecastFolder = IOWrapper.ReadString("Import folder path:");
            }

            var whatIf = $"{settings.BilancioSettings.ForecastFolder}\\What If";
            var mastro = $"{settings.BilancioSettings.ForecastFolder}\\Mastro Previsionale.xlsx";
            var forecast = $"{settings.BilancioSettings.ForecastFolder}\\Forecast.xlsx";

            var files = Directory.EnumerateFiles(whatIf, "*", SearchOption.AllDirectories);


            var forecastFiles = AnsiConsole.Prompt(
                new MultiSelectionPrompt<string>()
                    .Title("Select forecast files to inclide in your [green]report[/]?")
                    .PageSize(10)
                    .MoreChoicesText("[grey](Move up and down to reveal more files)[/]")
                    .InstructionsText(
                        "[grey](Press [blue]<space>[/] to toggle a file, " +
                        "[green]<enter>[/] to accept)[/]")
                    .AddChoices(files));


            List<ForecastDefinition> definitions = new List<ForecastDefinition>();

            foreach (var file in forecastFiles)
            {
                using (ExcelPackage package = new ExcelPackage(new FileInfo(file)))
                {
                    ForecastDefinitionTable tt = new ForecastDefinitionTable();
                    definitions.AddRange(tt.ReadTable(package.Workbook));
                }
            }

            var yearStart = IOWrapper.ReadInt("Initial year?", DateTime.Now.Year, 2020, 2100);
            var years = IOWrapper.ReadInt("How many years do you want to forecast?", 5, 1, 100);
            var endDate = new DateTime(DateTime.Now.Year + years, 12, 31);
            var currentDate = new DateTime(yearStart, 1, 1);

            List<TransactionV4> forecastedData = new List<TransactionV4>();

            while (currentDate <= endDate)
            {
                foreach (var entry in definitions)
                {
                    if (CheckYear(entry.Year, currentDate)
                        && CheckMonth(entry.Month, currentDate)
                        && CheckDay(entry.Day, currentDate)
                        && CheckDayOfWeek(entry.DayOfWeek, currentDate))
                {
                        TransactionV4 newTran = new TransactionV4()
                        {
                            Id = $"{currentDate.ToString("yyyyMMdd")}_{entry.PartID}",
                            Date = currentDate,
                            Account = entry.Account,
                            IsNew = true,
                            Inflow = entry.Inflow,
                            Outflow = entry.Outflow,
                        };
                        forecastedData.Add(newTran);
                    } 
                }

                if (currentDate.Day == 5)
                {
                    double total = forecastedData.Where(_ => _.Date.Year == currentDate.Year
                        && _.Date.Month == currentDate.Month - 1
                        && _.Account == "Carta Credito").Sum(_ => _.Outflow);

                    TransactionV4 newTran1 = new TransactionV4()
                    {
                        Id = $"Carta-Credito-Cassa",
                        Date = currentDate,
                        Account = "Cassa",
                        IsNew = true,
                        Inflow = 0,
                        Outflow = total,
                    };
                    forecastedData.Add(newTran1);
                    TransactionV4 newTran2 = new TransactionV4()
                    {
                        Id = $"Carta-Credito-Cassa",
                        Date = currentDate,
                        Account = "Carta Credito",
                        IsNew = true,
                        Inflow = total,
                        Outflow = 0,
                    };
                    forecastedData.Add(newTran2);
                }

                currentDate = currentDate.AddDays(1);
            }




            using (ExcelPackage package = new ExcelPackage(new FileInfo(forecast)))
            {
                TransactionsTable tt = new TransactionsTable("ForecastTrans");
                tt.BuildTable(package.Workbook, forecastedData, true);
                package.Save();
            }
        }

        private bool CheckYear(string year, DateTime currentDate)
        {
            if (year == "*")
            {
                return true;
            }
            else if (year.Contains("-"))
            {
                var splitted = year.Split('-');
                if (splitted.Length == 2)
                {
                    var ordered = splitted.Select(_ => Convert.ToInt32(_)).OrderBy(x => x).ToList();
                    var min = ordered[0];
                    var max = ordered[1];

                    if (min <= currentDate.Year && max >= currentDate.Year)
                    {
                        return true;
                    }
                }
            }
            else if (year.Contains("/"))
            {
                var splitted = year.Split('/');
                if (splitted.Length == 2)
                {
                    var startValue = Convert.ToInt32(splitted[0]);
                    var periodicity = Convert.ToInt32(splitted[1]);

                    while (currentDate.Year >= startValue)
                    {
                        if (currentDate.Year == startValue)
                        {
                            return true;
                        }

                        startValue = startValue + periodicity;
                    }
                }
            }
            else if (Convert.ToInt32(year) == currentDate.Year)
            {
                return true;
            }

            return false;
        }

        private bool CheckMonth(string month, DateTime currentDate)
        {
            if (month == "*")
            {
                return true;
            }

            if (month.Contains("-"))
            {
                var splitted = month.Split('-');
                if (splitted.Length == 2)
                {
                    var ordered = splitted.Select(_ => Convert.ToInt32(_)).OrderBy(x => x).ToList();
                    var min = ordered[0];
                    var max = ordered[1];

                    if (min <= currentDate.Month && max >= currentDate.Month)
                    {
                        return true;
                    }
                }
            }
            else if (month.Contains("/"))
            {
                var splitted = month.Split('/');
                if (splitted.Length == 2)
                {
                    var startValue = Convert.ToInt32(splitted[0]);
                    var periodicity = Convert.ToInt32(splitted[1]);

                    while (currentDate.Month >= startValue)
                    {
                        if (currentDate.Month == startValue)
                        {
                            return true;
                        }

                        startValue = startValue + periodicity;
                    }
                }
            }
            else if (Convert.ToInt32(month) == currentDate.Month)
            {
                return true;
            }

            return false;
        }

        private bool CheckDay(string day, DateTime currentDate)
        {
            if (day == "*")
            {
                return true;
            }
            else if (day.Contains("-"))
            {
                var splitted = day.Split('-');
                if (splitted.Length == 2)
                {
                    var ordered = splitted.Select(_ => Convert.ToInt32(_)).OrderBy(x => x).ToList();
                    var min = ordered[0];
                    var max = ordered[1];

                    if (min <= currentDate.Day && max >= currentDate.Day)
                    {
                        return true;
                    }
                }
            }
            else if (day.Contains("/"))
            {
                var splitted = day.Split('/');
                if (splitted.Length == 2)
                {
                    var startValue = Convert.ToInt32(splitted[0]);
                    var periodicity = Convert.ToInt32(splitted[1]);

                    while (currentDate.Day >= startValue)
                    {
                        if (currentDate.Day == startValue)
                        {
                            return true;
                        }

                        startValue = startValue + periodicity;
                    }
                }
            }
            else if (day == "L")
            {

                var lastDay = DateTime.DaysInMonth(currentDate.Year, currentDate.Month);

                if (currentDate.Day == lastDay)
                {
                    return true;
                }
            }
            else if (Convert.ToInt32(day) == currentDate.Day)
            {
                return true;
            }

            return false;
        }

        private bool CheckDayOfWeek(string dayOW, DateTime currentDate)
        {
            if (dayOW == "*")
            {
                return true;
            }
            else if (dayOW.Contains("-"))
            {
                var splitted = dayOW.Split('-');
                if (splitted.Length == 2)
                {
                    var ordered = splitted.Select(_ => Convert.ToInt32(_)).OrderBy(x => x).ToList();
                    var min = ordered[0];
                    var max = ordered[1];

                    if (min <= (int)currentDate.DayOfWeek && max >= (int)currentDate.DayOfWeek)
                    {
                        return true;
                    }
                }
            }
            else if (dayOW.Contains("/"))
            {
                var splitted = dayOW.Split('/');
                if (splitted.Length == 2)
                {
                    var startValue = Convert.ToInt32(splitted[0]);
                    var periodicity = Convert.ToInt32(splitted[1]);

                    while ((int)currentDate.DayOfWeek >= startValue)
                    {
                        if ((int)currentDate.DayOfWeek == startValue)
                        {
                            return true;
                        }

                        startValue = startValue + periodicity;
                    }
                }
            }
            else if (Convert.ToInt32(dayOW) == (int)currentDate.DayOfWeek)
            {
                return true;
            }

            return false;
        }
    }
}
