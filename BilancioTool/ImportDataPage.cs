using BilancioTool.Core.Entities;
using BilancioTool.Core.Tables;
using CmdTools.Core.CmdMenuAndPages;
using CmdTools.Core.UserSettings;
using OfficeOpenXml;
using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

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


            if (account.StartsWith("Unicredit"))
            {
                List<MovimentoConto> movimenti = new List<MovimentoConto>();

                using (ExcelPackage package = new ExcelPackage(new FileInfo(file)))
                {
                    MovimentContoTable tt = new MovimentContoTable(account);
                    movimenti = tt.ReadTable(package.Workbook);
                }

                if (!IOWrapper.GetConfirmation($"Confirm book folder at [green]{settings.BilancioSettings.BooktFolder}[/]?"))
                {
                    settings.BilancioSettings.BooktFolder = IOWrapper.ReadString("Import folder path:");
                }

                var year = IOWrapper.ReadInt("Wicth year do you want to import?", DateTime.Now.Year, 1900, DateTime.Now.Year);
                var mastrinoPath = $"{settings.BilancioSettings.BooktFolder}/Mastrino {year}.xlsx";

                List<TransactionV4> transactionsV4 = new List<TransactionV4>();
                using (ExcelPackage package = new ExcelPackage(new FileInfo(mastrinoPath)))
                {
                    TransactionsTable tt = new TransactionsTable();
                    transactionsV4 = tt.ReadTable(package.Workbook);
                }

                var match = 0;
                var noMatch = 0;

                foreach (var trans in movimenti.Where(_ => _.DataValuta.Year == year))
                {

                    var possibilities = transactionsV4.Where(_ =>
                        _.Account.ToLower().Trim() == trans.Account.ToLower().Trim()
                        && _.Inflow == trans.Inflow
                        && _.Outflow == trans.Outflow
                        && (_.Date <= trans.DataRegistrazione.AddDays(2) && _.Date >= trans.DataValuta.AddDays(-2))
                        ).ToList();

                    if (possibilities.Count == 1)
                    {
                        match++;
                        if (possibilities[0].Date != trans.DataValuta)
                        {
                            possibilities[0].Date = trans.DataValuta;
                            possibilities[0].HasChanges = true;
                        }

                        if (possibilities[0].Notes.Trim() != trans.Descrizione.Trim())
                        {
                            possibilities[0].Notes = trans.Descrizione.Trim();
                            possibilities[0].HasChanges = true;
                        }  
                    }
                    else
                    {
                        noMatch++;
                        TransactionV4 newTrans = new TransactionV4()
                        {
                            Id = $"{trans.DataValuta.ToString("yyyyMMdd")}_6{noMatch}",
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
                            Id = $"{trans.DataValuta.ToString("yyyyMMdd")}_6{noMatch}",
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

            if (account.StartsWith("Carta Credito"))
            {
                List<MovimentoCarta> movimenti = new List<MovimentoCarta>();

                using (ExcelPackage package = new ExcelPackage(new FileInfo(file)))
                {
                    MovimentoCartaTable tt = new MovimentoCartaTable(account);
                    movimenti = tt.ReadTable(package.Workbook);
                }

                if (!IOWrapper.GetConfirmation($"Confirm book folder at [green]{settings.BilancioSettings.BooktFolder}[/]?"))
                {
                    settings.BilancioSettings.BooktFolder = IOWrapper.ReadString("Import folder path:");
                }

                var year = IOWrapper.ReadInt("Wicth year do you want to import?", DateTime.Now.Year, 1900, DateTime.Now.Year);
                var mastrinoPath = $"{settings.BilancioSettings.BooktFolder}/Mastrino {year}.xlsx";

                List<TransactionV4> transactionsV4 = new List<TransactionV4>();
                using (ExcelPackage package = new ExcelPackage(new FileInfo(mastrinoPath)))
                {
                    TransactionsTable tt = new TransactionsTable();
                    transactionsV4 = tt.ReadTable(package.Workbook);
                }


                // UPDATE ID
                var grouped = transactionsV4.GroupBy(_ => _.Id);
                foreach (var group in grouped)
                {
                    var key = group.Key;



                }



                var match = 0;
                var noMatch = 0;
                var newTransCont = 0;

                foreach (var trans in movimenti.Where(_ => _.DataRegistrazione.Year == year))
                {
                    var currentMonth = trans.DataRegistrazione.Month;

                    var possibilities = transactionsV4.Where(_ =>
                        _.Account.ToLower().Trim() == trans.Account.ToLower().Trim()
                        && _.Inflow == trans.Inflow
                        && _.Outflow == trans.Outflow
                        && _.Notes != trans.Descrizione
                        && (_.Date.Day == 5 && (_.Date.Month == currentMonth || _.Date.Month == currentMonth - 1))
                        && _.HasChanges == false
                        ).OrderByDescending(_ => _.Date).ToList();


                    if (possibilities.Count >= 1)
                    {
                        match++;
                        if (possibilities[0].Date != trans.DataRegistrazione)
                        {
                            possibilities[0].Date = trans.DataRegistrazione;
                            possibilities[0].HasChanges = true;
                        }

                        if (possibilities[0].Notes.Trim() != trans.Descrizione.Trim())
                        {
                            possibilities[0].Notes = trans.Descrizione.Trim();
                            possibilities[0].HasChanges = true;
                        }

                        if (!possibilities[0].Id.StartsWith(trans.DataRegistrazione.ToString("yyyyMMdd")))
                        {
                            var newID = $"{trans.DataRegistrazione.ToString("yyyyMMdd")}_8{noMatch}";
                            foreach (var other in transactionsV4.Where(_ => _.Id == possibilities[0].Id))
                            {
                                other.Id = newID;
                                other.Date = possibilities[0].Date;
                                other.HasChanges = true;
                            }
                            possibilities[0].Id = newID;
                            possibilities[0].HasChanges = true;
                        }
                        
                    }
                    else
                    {
                        noMatch++;


                        var possibilities2 = transactionsV4.Where(_ =>
                        _.Account.ToLower().Trim() == trans.Account.ToLower().Trim()
                            && _.Inflow == trans.Inflow
                            && _.Outflow == trans.Outflow
                            && _.Date == trans.DataRegistrazione
                            && _.Notes.Trim() == trans.Descrizione.Trim()
                            && _.HasChanges == false
                            ).OrderByDescending(_ => _.Date).ToList();

                        if (possibilities2.Count == 1)
                        {
                            continue;

                        }
                        //else
                        //{
                        //    newTransCont++;
                        //    TransactionV4 newTrans = new TransactionV4()
                        //    {
                        //        Id = $"{trans.DataRegistrazione.ToString("yyyyMMdd")}_6{noMatch}",
                        //        Date = trans.DataRegistrazione,
                        //        Account = trans.Account,
                        //        Inflow = trans.Inflow,
                        //        Outflow = trans.Outflow,
                        //        Notes = trans.Descrizione.Trim(),
                        //        IsNew = true,
                        //    };
                        //    transactionsV4.Add(newTrans);
                        //    TransactionV4 newTrans2 = new TransactionV4()
                        //    {
                        //        Id = newTrans.Id,
                        //        Date = newTrans.Date,
                        //        Account = "No Match",
                        //        Inflow = trans.Outflow,
                        //        Outflow = trans.Inflow,
                        //        Notes = "",
                        //        IsNew = true,
                        //    };
                        //    transactionsV4.Add(newTrans2);
                        //}
                        
                    }
                }


                IOWrapper.WriteLine($"Matches: {match}");
                IOWrapper.WriteLine($"No Matches: {noMatch}");
                IOWrapper.WriteLine($"Created New: {newTransCont}");

                using (ExcelPackage package = new ExcelPackage(new FileInfo(mastrinoPath)))
                {
                    TransactionsTable tt = new TransactionsTable();

                    tt.UpdateTable(package.Workbook, transactionsV4);
                    package.Save();
                }


                IOWrapper.WriteLine("Done");

            }
        }
    }
}
