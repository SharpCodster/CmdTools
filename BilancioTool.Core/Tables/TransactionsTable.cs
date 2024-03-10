using BilancioTool.Core.Entities;
using OfficeOpenXml.Table;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using OfficeOpenXml.Style;
using System.Drawing;

namespace BilancioTool.Core.Tables
{
    public class TransactionsTable : BaseTable<TransactionV4>
    {

        private Color[] customColor = 
        {
            Color.FromArgb(189, 215, 238),
            Color.FromArgb(255, 230, 153)
        };


        public TransactionsTable() : base("Trans", true, 8)
        {

        }

        public TransactionsTable(string tableName) : base(tableName, true, 8)
        {

        }

        protected override void PopulateWorksheet(ExcelWorksheet workSheet, ExcelTable table, List<TransactionV4> data)
        {
            workSheet.Cells["A1"].Value = "ID";
            workSheet.Cells["B1"].Value = "Date";
            workSheet.Cells["C1"].Value = "Account";
            workSheet.Cells["D1"].Value = "Inflow";
            workSheet.Cells["E1"].Value = "Outflow";
            workSheet.Cells["F1"].Value = "Payee";
            workSheet.Cells["G1"].Value = "Notes";
            workSheet.Cells["H1"].Value = "Tags";

            for (int i = 0; i < data.Count; i++)
            {
                workSheet.Cells[$"A{i + 2}"].Value = data[i].Id;

                workSheet.Cells[$"B{i + 2}"].Value = data[i].Date;
                workSheet.Cells[$"B{i + 2}"].Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;

                workSheet.Cells[$"C{i + 2}"].Value = data[i].Account;
                workSheet.Cells[$"D{i + 2}"].Value = data[i].Inflow;
                workSheet.Cells[$"E{i + 2}"].Value = data[i].Outflow;
                workSheet.Cells[$"F{i + 2}"].Value = data[i].Payee;
                workSheet.Cells[$"G{i + 2}"].Value = data[i].Notes;

                workSheet.Cells[$"H{i + 2}"].Value = data[i].Tags;
            }


        }

        protected override List<TransactionV4> ReadAllRows(ExcelWorksheet workSheet, int firstRow, int totalRows)
        {
            List<TransactionV4> data = new List<TransactionV4>();

            for (int i = firstRow; i <= totalRows; i++)
            {
                TransactionV4 row = new TransactionV4();

                row.Id = workSheet.GetValue<string>(i, 1);
                row.Date = workSheet.GetValue<DateTime>(i, 2);
                row.Account = workSheet.GetValue<string>(i, 3);
                row.Inflow = workSheet.GetValue<double>(i, 4);
                row.Outflow = workSheet.GetValue<double>(i, 5);
                row.Payee = workSheet.GetValue<string>(i, 6);
                row.Notes = workSheet.GetValue<string>(i, 7);
                row.Tags = workSheet.GetValue<string>(i, 8);
                row.ExcelRow = i;
                data.Add(row);
            }

            return data;

        }


        public override void UpdateTable(ExcelWorkbook workBook, List<TransactionV4> data)
        {
            ExcelWorksheet workSheet = (from ws in workBook.Worksheets
                                        where ws.Name == _tableName
                                        select ws).FirstOrDefault();

            ExcelTable table = (from title in workSheet.Tables
                                where title.Name == _tableName
                                select title).FirstOrDefault();

            var changes = data.Where(_ => _.HasChanges && !_.IsNew).ToList();

            foreach (var item in changes)
            {
                workSheet.SetValue(item.ExcelRow, 1, item.Id);
                workSheet.SetValue(item.ExcelRow, 2, item.Date);
                workSheet.SetValue(item.ExcelRow, 3, item.Account);
                workSheet.SetValue(item.ExcelRow, 4, item.Inflow);
                workSheet.SetValue(item.ExcelRow, 5, item.Outflow);
                workSheet.SetValue(item.ExcelRow, 6, item.Payee);
                workSheet.SetValue(item.ExcelRow, 7, item.Notes);
                workSheet.SetValue(item.ExcelRow, 8, item.Tags);
                workSheet.SetValue(item.ExcelRow, 9, item.Accounts);
            }

            var newItems = data.Where(_ => _.IsNew).ToList();

            if (newItems.Count > 0)
            {
                int totalDataRows = table.Address.End.Row;

                if (table.ShowTotal == true)
                {
                    totalDataRows -= 1;
                }

                table.AddRow(newItems.Count);

                for (int i = 0; i < newItems.Count; i++)
                {
                    int row = totalDataRows + 1 + i;

                    var newItem = newItems[i];

                    workSheet.SetValue(row, 1, newItem.Id);
                    workSheet.SetValue(row, 2, newItem.Date);
                    workSheet.SetValue(row, 3, newItem.Account);
                    workSheet.SetValue(row, 4, newItem.Inflow);
                    workSheet.SetValue(row, 5, newItem.Outflow);
                    workSheet.SetValue(row, 7, newItem.Notes);
                    workSheet.SetValue(row, 8, newItem.Tags);
                    workSheet.SetValue(row, 9, newItem.Accounts);

                    for (int j = 1; j < 10; j++)
                    {
                        int k = i;
                        while (k > customColor.Length - 1)
                        {
                            k = k - customColor.Length;
                        }

                        workSheet.Cells[row, j].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        workSheet.Cells[row, j].Style.Fill.BackgroundColor.SetColor(customColor[k]);
                    }
                }
            }
        }
    }
}
