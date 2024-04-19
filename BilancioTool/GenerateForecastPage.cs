using BilancioTool.Core.Entities;
using BilancioTool.Core.Tables;
using CmdTools.Core.CmdMenuAndPages;
using CmdTools.Core.Helpers;
using CmdTools.Core.UserSettings;
using OfficeOpenXml;
using Spectre.Console;

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
                    if (PseudoCronEvaluetor.CheckExpresison(entry.Year, entry.Month, entry.Day, entry.DayOfWeek, currentDate))
                    {
                        TransactionV4 newTran = new TransactionV4()
                        {
                            Id = $"{currentDate.ToString("yyyyMMdd")}_{entry.PartID}",
                            Date = currentDate,
                            Account = entry.Account,
                            IsNew = true,
                            Inflow = entry.Inflow,
                            Outflow = entry.Outflow,
                            Payee = entry.Payee,
                            Tags = entry.Tags,
                            Notes = entry.Notes,
                            Accounts = entry.Account
                        };
                        forecastedData.Add(newTran);

                        if (entry.Autobalance)
                        {
                            string accounts = $"{entry.Account}|{entry.AccountTo}";
                            newTran.Accounts = accounts;
                            TransactionV4 newTran2 = new TransactionV4()
                            {
                                Id = $"{currentDate.ToString("yyyyMMdd")}_{entry.PartID}",
                                Date = currentDate,
                                Account = entry.AccountTo,
                                IsNew = true,
                                Inflow = entry.Outflow,
                                Outflow = entry.Inflow,
                                Payee = entry.Payee,
                                Tags = entry.Tags,
                                Accounts= accounts
                            };
                            forecastedData.Add(newTran2);
                        }
                    } 
                }

                if (currentDate.Day == 5)
                {
                    DateTime pastMonth = currentDate.AddMonths(-1);

                    double total = forecastedData.Where(_ => _.Date.Year == pastMonth.Year
                        && _.Date.Month == pastMonth.Month
                        && _.Account == "Carta Credito").Sum(_ => _.Outflow);

                    TransactionV4 newTran1 = new TransactionV4()
                    {
                        Id = $"Carta-Credito-Cassa",
                        Date = currentDate,
                        Account = "Cassa",
                        IsNew = true,
                        Inflow = 0,
                        Outflow = total,
                        Accounts = "Carta Credito|Cassa"
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
                        Accounts = "Carta Credito|Cassa"
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
    }
}
