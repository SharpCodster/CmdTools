using BilancioTool.Core.Entities;
using BilancioTool.Core.Entities.NewFolder;
using BilancioTool.Core.Helpers;
using BilancioTool.Core.Tables;
using CmdTools.Core.CmdMenuAndPages;
using CmdTools.Core.Helpers;
using CmdTools.Core.UserSettings;
using Newtonsoft.Json;
using OfficeOpenXml;
using Spectre.Console;
using System.Text.RegularExpressions;

namespace BilancioTool
{
    public class MigrateToCodio : Page
    {
        public MigrateToCodio(ICmdWizard program, IIOWrapper ioWrapper)
            : base("Migrate to Web Codio - MyMoney", program, ioWrapper)
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

            var env = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Chose environment for [green]migration[/]?")
                    .PageSize(10)
                    .MoreChoicesText("[grey](Move up and down to reveal more)[/]")
                    .AddChoices(new[] {
                        "Local", "Production",
                    }));



            var mastroPath = $"{settings.BilancioSettings.BooktFolder}/LibroMastro.xlsx";
            List<TransactionV4> excelLedgerList = new List<TransactionV4>();
            List<Account> excelAccountList = new List<Account>();
            using (ExcelPackage package = new ExcelPackage(new FileInfo(mastroPath)))
            {
                TransactionsTable tt = new TransactionsTable("Trans");
                excelLedgerList = tt.ReadTable(package.Workbook);
                AccountTable accountTable = new AccountTable("Accounts");
                excelAccountList = accountTable.ReadTable(package.Workbook);
            }

            var codioUrl = settings.BilancioSettings.DevCodioUrl;
            var credentials = settings.BilancioSettings.DevCredential;
            if (env == "Production")
            {
                codioUrl = settings.BilancioSettings.ProdCodioUrl;
                credentials = settings.BilancioSettings.ProdCredential;
            }

            CodioApiClient codioClient = new CodioApiClient(codioUrl, credentials);


            var codioAccounts = codioClient.GetAsync<List<AccountDtoRead>>("mymoney/api/accounts").GetAwaiter().GetResult();

            DateTime dateStart = new DateTime(2013, 1, 1);
            DateTime dateEnd = new DateTime(2015, 12, 31);


            var excelRowsToMigrate = excelLedgerList.Where(x => x.Date >= dateStart && x.Date <= dateEnd).ToList();

            var accountsToMigrate = excelRowsToMigrate.Select(x => x.Account).Distinct().ToList();

            foreach (var account in accountsToMigrate)
            {
                if (codioAccounts.Any(_=> _.Name == account) == false)
                {
                    var excelAccount = excelAccountList.FirstOrDefault(_ => _.Name == account);
                    var newAccount = new AccountDtoWrite()
                    {
                        Name = account,
                        BalanceSheetType = excelAccount.BalanceSheetClass,
                        CashFlowType = excelAccount.CashFlowClass,
                        IsActive = true
                    };

                    var response = codioClient.PostAsync<AccountDtoRead, AccountDtoWrite>("mymoney/api/accounts", newAccount).GetAwaiter().GetResult();
                    codioAccounts.Add(response);
                }
            }


            var transExcel = excelRowsToMigrate
                .GroupBy(x => new { x.Id, x.Date, x.Payee, x.Tags })
                .ToList();


            foreach (var exceTrans in transExcel)
            {
                var newTransaction = new TransactionDtoCreate()
                {
                    Date = exceTrans.Key.Date,
                    ExcelId = exceTrans.Key.Id,
                    Payee = exceTrans.Key.Payee,
                    Tags = exceTrans.Key.Tags?.Split('|').ToList() ?? new List<string>()
                };


                foreach (var excelRow in exceTrans)
                {
                    newTransaction.Records.Add(new RecordDtoWrite()
                    {
                        Account = excelRow.Account,
                        Inflow = (decimal)excelRow.Inflow,
                        Outflow = (decimal)excelRow.Outflow,
                        Notes = excelRow.Notes
                    });
                }

                try
                {
                    var response = codioClient.PostAsync<TransactionDtoRead, TransactionDtoCreate>("mymoney/api/transactions", newTransaction).GetAwaiter().GetResult();
                }
                catch (Exception ex)
                {
                    int i = 0;
                }
            }




            //var potentialErrors = excelRowsToMigrate
            //    .GroupBy(x => new { x.Date, x.Account })
            //    .Where(g => g.Count() > 1)
            //    .SelectMany(g => g)
            //    .ToList();

            //if (potentialErrors.Any())
            //{
            //    var table = new Table();
            //    table.AddColumn("Date");
            //    table.AddColumn("Account");
            //    table.AddColumn("ExcelId");

            //    foreach (var error in potentialErrors)
            //    {
            //        table.AddRow(
            //            error.Date.ToString("yyyy-MM-dd"),
            //            error.Account,
            //            error.Id
            //        );
            //    }

            //    AnsiConsole.Write(table);
            //}

            if (IOWrapper.GetConfirmation($"Done. Go home?"))
            {

            }

            Program.NavigateHome();
        }
    }
}


