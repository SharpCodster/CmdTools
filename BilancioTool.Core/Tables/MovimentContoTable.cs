using OfficeOpenXml.Table;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Text;
using BilancioTool.Core.Entities;
using System.Linq;

namespace BilancioTool.Core.Tables
{
    public class MovimentContoTable : BaseTable<MovimentoConto>
    {
        private string _account;

        public MovimentContoTable(string account) : base("ExportXLS", true, 5)
        {
            _account = account;
        }


        public override List<MovimentoConto> ReadTable(ExcelWorkbook workBook)
        {
            List<MovimentoConto> data = new List<MovimentoConto>();

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
                        MovimentoConto row = new MovimentoConto();
                        row.DataRegistrazione = workSheet.GetValue<DateTime>(counter, 1);
                        row.DataValuta = workSheet.GetValue<DateTime>(counter, 2);
                        row.Causale = workSheet.GetValue<string>(counter, 3);
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


            //for (int i = firstRow; i <= totalRows; i++)
            //{
            //    MovimentoConto row = new MovimentoConto();
            //    row.DataRegistrazione = workSheet.GetValue<DateTime>(i, 1);
            //    row.DataValuta = workSheet.GetValue<DateTime>(i, 2);
            //    row.Causale = workSheet.GetValue<string>(i, 3);
            //    row.Descrizione = workSheet.GetValue<string>(i, 4);
            //    row.Importo = workSheet.GetValue<double>(i, 5);

            //    row.Account = Account;
            //    data.Add(row);
            //}



            return data;
        }




        protected override void PopulateWorksheet(ExcelWorksheet workSheet, ExcelTable table, List<MovimentoConto> data)
        {
            throw new NotImplementedException();
        }



        protected override List<MovimentoConto> ReadAllRows(ExcelWorksheet workSheet, int firstRow, int totalRows)
        {
            List<MovimentoConto> data = new List<MovimentoConto>();

            return data;
        }
    }
}

