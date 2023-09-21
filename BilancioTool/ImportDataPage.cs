using BilancioTool.Core.Entities;
using BilancioTool.Core.Tables;
using CmdTools.Core.CmdMenuAndPages;
using CmdTools.Core.UserSettings;
using OfficeOpenXml;
using Spectre.Console;

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


            UserSettingsManager settingManager = new UserSettingsManager();
            var settings = settingManager.ReadFromConfigFile();


            if (!IOWrapper.GetConfirmation($"Confirm import folder at [green]{settings.BilancioSettings.ImportFolder}[/]?"))
            {
                settings.BilancioSettings.ImportFolder = IOWrapper.ReadString("Import folder path:");
            }

            var files = Directory.EnumerateFiles(settings.BilancioSettings.ImportFolder, "*", SearchOption.AllDirectories);
            
            var file = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("What file do you want to [green]import[/]?")
                    .PageSize(10)
                    .MoreChoicesText("[grey](Move up and down to reveal more)[/]")
                    .AddChoices(files));


            if (!IOWrapper.GetConfirmation($"Confirm book folder at [green]{settings.BilancioSettings.BooktFolder}[/]?"))
            {
                settings.BilancioSettings.BooktFolder = IOWrapper.ReadString("Import folder path:");
            }

            var year = IOWrapper.ReadInt("Wicth year do you want to import?", DateTime.Now.Year, 1900, DateTime.Now.Year);
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
                using (ExcelPackage package = new ExcelPackage(new FileInfo(file)))
                {
                    MovimentContoTable tt = new MovimentContoTable(account);
                    movimenti = tt.ReadTable(package.Workbook);
                }
            }

            if (account.StartsWith("Carta Credito"))
            {
                using (ExcelPackage package = new ExcelPackage(new FileInfo(file)))
                {
                    MovimentoCartaTable tt = new MovimentoCartaTable(account);
                    movimenti = tt.ReadTable(package.Workbook);
                }
            }

            foreach (var trans in movimenti.Where(_ => _.DataValuta.Year == year))
            {
                var alredyInserted = ExactMAtch(transactionsV4, trans);
                
                if (alredyInserted != null)
                {
                    match++;
                }
                else
                {
                    List<TransactionV4> possibilities = WideMatch(transactionsV4, trans);

                    if (possibilities.Count == 0)
                    {
                        noMatch++;
                        TransactionV4 newTrans = new TransactionV4()
                        {
                            Id = $"{trans.DataValuta.ToString("yyyyMMdd")}_NM{noMatch}",
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
                            Id = $"{trans.DataValuta.ToString("yyyyMMdd")}_NM{noMatch}",
                            Date = trans.DataValuta,
                            Account = "No Match",
                            Inflow = trans.Outflow,
                            Outflow = trans.Inflow,
                            Notes = "",
                            IsNew = true,
                        };
                        transactionsV4.Add(newTrans2);
                    }
                    else
                    {
                        match++;

                        var firstPossible = possibilities.OrderBy(_ => Math.Abs((_.Date - trans.DataValuta).Ticks)).FirstOrDefault();

                        if (firstPossible.Date != trans.DataValuta)
                        {
                            firstPossible.Date = trans.DataValuta;
                            firstPossible.HasChanges = true;
                        }

                        if (String.IsNullOrEmpty(possibilities[0].Notes) || possibilities[0].Notes.Trim() != trans.Descrizione.Trim())
                        {
                            firstPossible.Notes = trans.Descrizione.Trim();
                            firstPossible.HasChanges = true;
                        }
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

            IOWrapper.WriteLine("Done");
            
        }

        private TransactionV4 ExactMAtch(List<TransactionV4> transactionsV4, Movimento? trans)
        {
            return transactionsV4.Where(_ =>
                                _.Account.ToLower().Trim() == trans.Account.ToLower().Trim()
                                && _.Inflow == trans.Inflow
                                && _.Outflow == trans.Outflow
                                && _.Date == trans.DataValuta
                                && _.Notes.Trim() == trans.Descrizione.Trim()
                                ).FirstOrDefault();
        }

        private List<TransactionV4> WideMatch(List<TransactionV4> transactionsV4, Movimento? trans)
        {
            return transactionsV4.Where(_ =>
                                _.Account.ToLower().Trim() == trans.Account.ToLower().Trim()
                                && _.Inflow == trans.Inflow
                                && _.Outflow == trans.Outflow
                                && (_.Date <= trans.DataRegistrazione.AddDays(2) && _.Date >= trans.DataValuta.AddDays(-2))
                                ).OrderBy(_ => _.Date).ToList();
        }
    }
}
