using BilancioTool.Core.Entities;
using BilancioTool.Core.Tables;
using CmdTools.Core.CmdMenuAndPages;
using CmdTools.Core.UserSettings;
using OfficeOpenXml;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text.RegularExpressions;

namespace BilancioTool
{
    public class FixDatesAndIdsPage : Page
    {
        public FixDatesAndIdsPage(ICmdWizard program, IIOWrapper ioWrapper)
            : base("Import data", program, ioWrapper)
        {
        }

        protected override void DisplayContent()
        {
            UserSettingsManager settingManager = new UserSettingsManager();
            var settings = settingManager.ReadFromConfigFile();


            if (!IOWrapper.GetConfirmation($"Confirm book folder at [green]{settings.BilancioSettings.BooktFolder}[/]?"))
            {
                settings.BilancioSettings.BooktFolder = IOWrapper.ReadString("New folder path:");
            }

            var year = IOWrapper.ReadInt("Wicth year do you want to fix?", DateTime.Now.Year, 1900, DateTime.Now.Year);
            var mastrinoPath = $"{settings.BilancioSettings.BooktFolder}/Mastrino {year}.xlsx";


            List<TransactionV4> transactionsV4 = new List<TransactionV4>();
            using (ExcelPackage package = new ExcelPackage(new FileInfo(mastrinoPath)))
            {
                TransactionsTable tt = new TransactionsTable();
                transactionsV4 = tt.ReadTable(package.Workbook);
            }

            string[] mainAccounts = {
                "Unicredit",
                "Unicredit Sofia",
                "Carta Credito Marco",
                "Carta Credito Sofia"
            };

            foreach (var account in mainAccounts)
            {
                var justAccountData = transactionsV4.Where(_ => _.Account == account).ToList();

                // FIX DATES
                foreach (var trans in justAccountData)
                {
                    var others = transactionsV4.Where(_ => _.Id == trans.Id
                        && _.Inflow == trans.Outflow
                        && _.Outflow == trans.Inflow).ToList();

                    foreach (var related in others)
                    {
                        if (related.Date != trans.Date)
                        {
                            related.Date = trans.Date;
                            related.HasChanges = true;
                        }
                    }
                }
            }


            var idsGrouped = transactionsV4.GroupBy(_ => _.Id);

            foreach (var isGroup in idsGrouped)
            {
                if (isGroup.Count() > 2)
                {
                    if (isGroup.Count() == 4 && isGroup.Where(_ => _.Account == "Banca Dal Piva").Count() == 1)
                    {
                        continue;
                    }

                    if (isGroup.Count() == 3 && isGroup.Where(_ => _.Account == "Spesa").Count() == 1
                        && isGroup.Where(_ => _.Account == "Buoni Pasto").Count() == 1)
                    {
                        continue;
                    }

                    if (isGroup.Count() == 3 && isGroup.Where(_ => _.Account == "Spesa").Count() == 1
                        && isGroup.Where(_ => _.Account == "Pellet").Count() == 1)
                    {
                        continue;
                    }

                    if (isGroup.Where(_ => _.Inflow > 0.0).Count() != isGroup.Where(_ => _.Outflow > 0.0).Count())
                    {
                        continue;
                    }

                    if (isGroup.Count() % 2 == 0)
                    {
                        int i = 1;

                        foreach (var item in isGroup)
                        {
                            if (!item.HasChanges)
                            {
                                var gemini = isGroup.FirstOrDefault(_ => _.Inflow == item.Outflow && _.Outflow == item.Inflow);

                                var newId = $"{item.Id}{i}";
                                i++;
                                item.Id = newId;
                                gemini.Id = newId;
                                item.HasChanges = true;
                                gemini.HasChanges = true;
                            }
                        }
                        continue;
                    }

                    IOWrapper.WriteLine($"Transaction {isGroup.Key} has {isGroup.Count()} records");
                }
            }

            foreach (var trans in transactionsV4)
            {
                if (trans.Inflow != 0.0 && trans.Outflow != 0.0)
                {
                    IOWrapper.WriteLine($"Transaction {trans.Id} has inflow and outflow populated");
                }
            }


            var dateGrouped = transactionsV4.GroupBy(_ => _.Date);

            foreach (var dateGroup in dateGrouped)
            {
                var idGrouped = dateGroup.GroupBy(_ => _.Id);

                var noTrans = idGrouped.Count();

                int i = 1;
                foreach (var items in idGrouped)
                {
                    var currendId = $"{dateGroup.Key.ToString("yyyyMMdd")}_{string.Format("{0:000}", i)}";
                    foreach (var item in items)
                    {
                        item.Id = currendId;
                        item.Account = item.Account.Trim();
                        if (!String.IsNullOrEmpty(item.Payee))
                        {
                            item.Payee = item.Payee.Trim();
                        }
                        if (!String.IsNullOrEmpty(item.Notes))
                        {
                            item.Notes = item.Notes.Trim();
                        }
                        item.HasChanges = true;
                    }
                    i++;
                }
            }

            using (ExcelPackage package = new ExcelPackage(new FileInfo(mastrinoPath)))
            {
                TransactionsTable tt = new TransactionsTable();

                tt.UpdateTable(package.Workbook, transactionsV4);
                package.Save();
            }

            if (IOWrapper.GetConfirmation($"Done. Go home?"))
            {

            }

            Program.NavigateHome();
        }
    }
}

