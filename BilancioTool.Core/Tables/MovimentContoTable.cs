using OfficeOpenXml.Table;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Text;
using BilancioTool.Core.Entities;
using System.Linq;

namespace BilancioTool.Core.Tables
{
    public class MovimentContoTable : BaseTable<Movimento>
    {
        private string _account;

        public MovimentContoTable(string account) : base("ExportXLS", true, 5)
        {
            _account = account;
        }


        public override List<Movimento> ReadTable(ExcelWorkbook workBook)
        {
            List<Movimento> data = new List<Movimento>();

            ExcelWorksheet workSheet = (from ws in workBook.Worksheets
                                        where ws.Name == _tableName
                                        select ws).FirstOrDefault();

            bool isEOF = false;
            bool foundTable = false;
            int counter = 1;

            while (!isEOF)
            {
                if (foundTable)
                {
                    var check = workSheet.GetValue<string>(counter, 1);

                    if (string.IsNullOrEmpty(check))
                    {
                        isEOF = true;
                    }
                    else
                    {
                        Movimento row = new Movimento();
                        row.DataRegistrazione = workSheet.GetValue<DateTime>(counter, 1);
                        row.DataValuta = workSheet.GetValue<DateTime>(counter, 2);
                        //row.Causale = workSheet.GetValue<string>(counter, 3);
                        row.Descrizione = workSheet.GetValue<string>(counter, 4);
                        row.Importo = workSheet.GetValue<double>(counter, 5);
                        row.Account = _account;
                        data.Add(row);
                    }
                }
                else
                {
                    var cellValue = workSheet.GetValue<String>(counter, 1);

                    if (cellValue == "Data Registrazione")
                    {
                        foundTable = true;
                    }
                }
                counter++;
            }


            return data;
        }

        protected override void PopulateWorksheet(ExcelWorksheet workSheet, ExcelTable table, List<Movimento> data)
        {
            throw new NotImplementedException();
        }

        protected override List<Movimento> ReadAllRows(ExcelWorksheet workSheet, int firstRow, int totalRows)
        {
            List<Movimento> data = new List<Movimento>();

            return data;
        }
    }
}

