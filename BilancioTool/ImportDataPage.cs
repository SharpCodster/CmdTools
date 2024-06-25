using BilancioTool.Core.Entities;
using BilancioTool.Core.Tables;
using CmdTools.Core.CmdMenuAndPages;
using CmdTools.Core.UserSettings;
using OfficeOpenXml;
using Spectre.Console;
using System.Drawing;
using System.Text.RegularExpressions;

namespace BilancioTool
{
    public class ImportDataPage : Page
    {
        public ImportDataPage(ICmdWizard program, IIOWrapper ioWrapper)
            : base("Import data", program, ioWrapper)
        {
        }

        protected override void DisplayContent()
        {
            var account = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("What account do you want to [green]import[/]?")
                    .PageSize(10)
                    .MoreChoicesText("[grey](Move up and down to reveal more)[/]")
                    .AddChoices(new[] {
                        "Unicredit", "Unicredit Sofia", "Carta Credito Marco",
                        "Carta Credito Sofia",
                    }));

            AnsiConsole.Write(String.Format("Account: {0}", account));
            AnsiConsole.WriteLine();

            UserSettingsManager settingManager = new UserSettingsManager();
            var settings = settingManager.ReadFromConfigFile();


            if (!IOWrapper.GetConfirmation($"Confirm import folder at [green]{settings.BilancioSettings.ImportFolder}[/]?"))
            {
                settings.BilancioSettings.ImportFolder = IOWrapper.ReadString("Import folder path:");
            }

            var files = Directory.EnumerateFiles(settings.BilancioSettings.ImportFolder, "*.xlsx", SearchOption.AllDirectories);
            var files2 = Directory.EnumerateFiles(settings.BilancioSettings.ImportFolder, "*.csv", SearchOption.AllDirectories);

            var file = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("What file do you want to [green]import[/]?")
                    .PageSize(10)
                    .MoreChoicesText("[grey](Move up and down to reveal more)[/]")
                    .AddChoices(files2.Concat(files)));

            AnsiConsole.Write(String.Format("File to Import: {0}", file));
            AnsiConsole.WriteLine();

            if (!IOWrapper.GetConfirmation($"Confirm book folder at [green]{settings.BilancioSettings.BooktFolder}[/]?"))
            {
                settings.BilancioSettings.BooktFolder = IOWrapper.ReadString("Import folder path:");
            }

            var mastrinoPath = $"{settings.BilancioSettings.BooktFolder}/LibroMastro.xlsx";

            List<TransactionV4> transactionsV4 = new List<TransactionV4>();
            
            using (ExcelPackage package = new ExcelPackage(new FileInfo(mastrinoPath)))
            {
                TransactionsTable tt = new TransactionsTable();
                transactionsV4 = tt.ReadTable(package.Workbook);
            }

            var match = 0;
            var noMatch = 0;

            List<Movimento> movimenti = new List<Movimento>();

            if (account.StartsWith("Unicredit"))
            {
                if (file.EndsWith(".csv"))
                {
                    using (var reader = new StreamReader(file))
                    {
                        bool isFirstRow = true;
                        while (!reader.EndOfStream)
                        {
                            var line = reader.ReadLine();
                            var values = line.Split(';');

                            if (isFirstRow)
                            {
                                isFirstRow = false;
                                continue;
                            }

                            movimenti.Add(new Movimento()
                            {
                                DataRegistrazione = Convert.ToDateTime(values[0]),
                                DataValuta = Convert.ToDateTime(values[1]),
                                Account = account,
                                Descrizione = FormatBankDescription(values[2]),
                                Importo = Convert.ToDouble(values[3])
                            });
                        }
                    }

                }
                else
                {
                    using (ExcelPackage package = new ExcelPackage(new FileInfo(file)))
                    {
                        MovimentContoTable tt = new MovimentContoTable(account);
                        movimenti = tt.ReadTable(package.Workbook);
                    }
                } 
            }

            if (account.StartsWith("Carta Credito"))
            {
                if (file.EndsWith(".csv"))
                {
                    using (var reader = new StreamReader(file))
                    {
                        bool isFirstRow = true;
                        while (!reader.EndOfStream)
                        {
                            var line = reader.ReadLine();
                            var values = line.Split(';');

                            if (values.Length == 0)
                            {
                                continue;
                            }

                            if (isFirstRow)
                            {
                                isFirstRow = false;
                                continue;
                            }

                            if (values.Length == 6)
                            {
                                movimenti.Add(new Movimento()
                                {
                                    DataRegistrazione = Convert.ToDateTime(values[0]),
                                    DataValuta = Convert.ToDateTime(values[0]),
                                    Account = account,
                                    Descrizione = FormatBankDescription(values[3]),
                                    Importo = Convert.ToDouble(values[4])
                                });
                            }

                            if (values.Length == 5)
                            {
                                movimenti.Add(new Movimento()
                                {
                                    DataRegistrazione = Convert.ToDateTime(values[0]),
                                    DataValuta = Convert.ToDateTime(values[0]),
                                    Account = account,
                                    Descrizione = values[2].Trim(),
                                    Importo = Convert.ToDouble(values[3])
                                });
                            }
                        }
                    }

                }
                else
                {
                    using (ExcelPackage package = new ExcelPackage(new FileInfo(file)))
                    {
                        MovimentoCartaTable tt = new MovimentoCartaTable(account);
                        movimenti = tt.ReadTable(package.Workbook);
                    }
                }
            }

            foreach (var trans in movimenti)
            {
                var rowCount = HasMultiple(trans, movimenti);
                var alredyInserted = ExactMatch(transactionsV4, trans);

                if (alredyInserted.Count == rowCount)
                {
                    match++;
                }
                else
                {
                    List<TransactionV4> possibilities = WideMatch(transactionsV4, trans);

                    if (possibilities.Count == 0)
                    {
                        for (var i = alredyInserted.Count; i < rowCount; i++)
                        {
                            var id = transactionsV4.Where(_ => _.Date == trans.DataValuta).GroupBy(_ => _.Id).Count() + 1;
                            string data = $"{trans.DataValuta.ToString("yyyyMMdd")}_{string.Format("{0:000}", id)}";

                            noMatch++;
                            TransactionV4 newTrans = new TransactionV4()
                            {
                                Id = data,
                                Date = trans.DataValuta,
                                Account = trans.Account,
                                Inflow = trans.Inflow,
                                Outflow = trans.Outflow,
                                Notes = trans.Descrizione.Trim(),
                                IsNew = true,
                            };
                            transactionsV4.Add(newTrans);
                            TransactionV4 newTrans2 = new TransactionV4()
                            {
                                Id = data,
                                Date = trans.DataValuta,
                                Account = "No Match",
                                Inflow = trans.Outflow,
                                Outflow = trans.Inflow,
                                Notes = "",
                                IsNew = true,
                            };
                            transactionsV4.Add(newTrans2);
                        }
                    }
                    else
                    {
                        match++;

                        if (rowCount == 1 && possibilities.Count == 1)
                        {
                            var firstPossible = possibilities.FirstOrDefault();

                            if (firstPossible.Date != trans.DataValuta)
                            {
                                string oldId = firstPossible.Id;
                                int newCount = transactionsV4.Where(_ => _.Date == trans.DataValuta).GroupBy(_ => _.Id).Count() + 1;

                                string newId = $"{trans.DataValuta.ToString("yyyyMMdd")}_{string.Format("{0:000}", newCount)}";

                                List<TransactionV4> toUpdate = transactionsV4.Where(_ => _.Id == oldId).ToList();

                                foreach(var item in toUpdate)
                                {
                                    item.Id = newId;
                                    item.Date = trans.DataValuta;
                                    item.HasChanges = true;
                                }    
                            }
                        }
                        else
                        {
                            int i = 0;
                        }

                        //var firstPossible = possibilities.OrderBy(_ => Math.Abs((_.Date - trans.DataValuta).Ticks)).FirstOrDefault();

                        //if (firstPossible.Date != trans.DataValuta)
                        //{
                        //    firstPossible.Date = trans.DataValuta;
                        //    firstPossible.HasChanges = true;
                        //}

                        //if (String.IsNullOrEmpty(possibilities[0].Notes) || possibilities[0].Notes.Trim() != trans.Descrizione.Trim())
                        //{
                        //    firstPossible.Notes = trans.Descrizione.Trim();
                        //    firstPossible.HasChanges = true;
                        //}

                    }
                }
            }

            IOWrapper.WriteLine($"Matches: {match}");
            IOWrapper.WriteLine($"No Matches: {noMatch}");

            using (ExcelPackage package = new ExcelPackage(new FileInfo(mastrinoPath)))
            {
                TransactionsTable tt = new TransactionsTable();

                tt.UpdateTable(package.Workbook, transactionsV4);
                package.Save();
            }

            IOWrapper.GetConfirmation("Press [[Enter]] to exit");

            Program.NavigateHome();

        }

        private int HasMultiple(Movimento? trans, List<Movimento> transactions)
        {
            return transactions.Where(_ =>
                                _.Inflow == trans.Inflow
                                && _.Outflow == trans.Outflow
                                && _.DataValuta == trans.DataValuta
                                && (_.Descrizione.Trim() == trans.Descrizione.Trim())
                                ).Count();
        }

        private List<TransactionV4> ExactMatch(List<TransactionV4> transactionsV4, Movimento trans)
        {
            return transactionsV4.Where(_ =>
                                _.Account.ToLower().Trim() == trans.Account.ToLower().Trim()
                                && _.Inflow == trans.Inflow
                                && _.Outflow == trans.Outflow
                                && _.Date == trans.DataValuta
                                && (!String.IsNullOrEmpty(_.Notes) && _.Notes.Trim() == trans.Descrizione.Trim())
                                ).ToList();
        }

        private List<TransactionV4> WideMatch(List<TransactionV4> transactionsV4, Movimento? trans)
        {
            return transactionsV4.Where(_ =>
                                _.Account.ToLower().Trim() == trans.Account.ToLower().Trim()
                                && _.Inflow == trans.Inflow
                                && _.Outflow == trans.Outflow
                                && (_.Date <= trans.DataValuta.AddDays(2) && _.Date >= trans.DataValuta.AddDays(-2))
                                && (!String.IsNullOrEmpty(_.Notes) && _.Notes.Trim() == trans.Descrizione.Trim())
                                ).OrderBy(_ => _.Date).ToList();
        }

        public static string FormatBankDescription(string input)
        {
            string result = input;
            if (!String.IsNullOrEmpty(result))
            {
                // Rimuovi i simboli
                result = result.Replace("*", " ").Replace("?", " ").Replace(":", " ").Replace(";", " ");
                // Toglie gli spazi dall'inizio alla fine
                result = result.Trim();
                // Sostituisci due o più spazi con ';'
                result = Regex.Replace(result, @"\s{2,}", " ");
            }
            return result;
        }
    }
}
