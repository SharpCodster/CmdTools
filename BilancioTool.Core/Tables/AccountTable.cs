using BilancioTool.Core.Entities;
using OfficeOpenXml.Table;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace BilancioTool.Core.Tables
{
    public class AccountTable : BaseTable<Account>
    {
        public AccountTable() : base("Account", true, 2)
        {

        }

        public AccountTable(string tableName) : base(tableName, true, 8)
        {

        }

        protected override void PopulateWorksheet(ExcelWorksheet workSheet, ExcelTable table, List<Account> data)
        {
            workSheet.Cells["A1"].Value = "ID";
            workSheet.Cells["B1"].Value = "Name";

            for (int i = 0; i < data.Count; i++)
            {
                workSheet.Cells[$"A{i + 2}"].Value = data[i].Id;
                workSheet.Cells[$"B{i + 2}"].Value = data[i].Name;
            }


        }

        protected override List<Account> ReadAllRows(ExcelWorksheet workSheet, int firstRow, int totalRows)
        {
            List<Account> data = new List<Account>();

            for (int i = firstRow; i <= totalRows; i++)
            {
                Account row = new Account();

                row.Id = workSheet.GetValue<int>(i, 1);
                row.Name = workSheet.GetValue<string>(i, 2);
                row.ExcelRow = i;
                data.Add(row);
            }

            return data;

        }


        public override void UpdateTable(ExcelWorkbook workBook, List<Account> data)
        {
            //ExcelWorksheet workSheet = (from ws in workBook.Worksheets
            //                            where ws.Name == _tableName
            //                            select ws).FirstOrDefault();

            //ExcelTable table = (from title in workSheet.Tables
            //                    where title.Name == _tableName
            //                    select title).FirstOrDefault();

            //var changes = data.Where(_ => _.HasChanges).ToList();

            //foreach (var item in changes)
            //{
            //    workSheet.SetValue(item.ExcelRow, 1, item.Id);
            //    workSheet.SetValue(item.ExcelRow, 2, item.Date);
            //    workSheet.SetValue(item.ExcelRow, 4, item.Inflow);
            //    workSheet.SetValue(item.ExcelRow, 5, item.Outflow);
            //    workSheet.SetValue(item.ExcelRow, 7, item.Notes);
            //}

            //var newItems = data.Where(_ => _.IsNew).ToList();

            //if (newItems.Count > 0)
            //{
            //    int totalDataRows = table.Address.End.Row;

            //    if (table.ShowTotal == true)
            //    {
            //        totalDataRows -= 1;
            //    }

            //    table.AddRow(newItems.Count);

            //    for (int i = 0; i < newItems.Count; i++)
            //    {
            //        int row = totalDataRows + 1 + i;

            //        var newItem = newItems[i];

            //        workSheet.SetValue(row, 1, newItem.Id);
            //        workSheet.SetValue(row, 2, newItem.Date);
            //        workSheet.SetValue(row, 3, newItem.Account);
            //        workSheet.SetValue(row, 4, newItem.Inflow);
            //        workSheet.SetValue(row, 5, newItem.Outflow);
            //        workSheet.SetValue(row, 7, newItem.Notes);
            //    }
            //}
        }
    }
}
