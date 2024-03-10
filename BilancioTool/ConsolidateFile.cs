using BilancioTool.Core.Entities;
using BilancioTool.Core.Tables;
using CmdTools.Core.CmdMenuAndPages;
using CmdTools.Core.UserSettings;
using OfficeOpenXml;

namespace BilancioTool
{
    public class ConsolidateFile : Page
    {
        public ConsolidateFile(ICmdWizard program, IIOWrapper ioWrapper)
            : base("Create new File Version", program, ioWrapper)
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

            var mastroPath = $"{settings.BilancioSettings.BooktFolder}/LibroMastro.xlsx";
            List<TransactionV4> transactionsV4 = new List<TransactionV4>();
            using (ExcelPackage package = new ExcelPackage(new FileInfo(mastroPath)))
            {
                TransactionsTable tt = new TransactionsTable("Trans");
                transactionsV4 = tt.ReadTable(package.Workbook);
            }

            // Fix date
            foreach (TransactionV4 tx in transactionsV4)
            {
                int year = Convert.ToInt32(tx.Id.Substring(0, 4));
                int month = Convert.ToInt32(tx.Id.Substring(4, 2));
                int day = Convert.ToInt32(tx.Id.Substring(6, 2));
                DateTime date = new DateTime(year, month, day);
                if (date != tx.Date)
                {
                    tx.Date = date;
                    tx.HasChanges = true;
                }
            }

            var grouped = transactionsV4.GroupBy(_ => _.Id);


            foreach (var tx in grouped)
            {
                var payeesList = tx.Select(x => x.Payee).Where(_ => !String.IsNullOrEmpty(_)).Distinct().ToList();

                if (payeesList.Count == 1)
                {
                    foreach (var original in tx)
                    {
                        if (original.Payee != payeesList[0])
                        {
                            original.Payee = payeesList[0];
                            original.HasChanges = true;
                        }
                    }
                }
                else if (payeesList.Count > 1)
                {
                    string id = tx.Key;
                }
            }


            foreach (var tx in grouped)
            {
                var accountList = tx.Select(x => x.Account).OrderBy(_ => _).Distinct().ToList();
                string result = String.Join("|", accountList);

                foreach (var original in tx)
                {
                    if (original.Accounts != result)
                    {
                        original.Accounts = result;
                        original.HasChanges = true;
                    }
                }

            }


            using (ExcelPackage package = new ExcelPackage(new FileInfo(mastroPath)))
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

