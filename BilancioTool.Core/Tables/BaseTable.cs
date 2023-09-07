using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OfficeOpenXml;
using OfficeOpenXml.Table;

namespace BilancioTool.Core.Tables
{
    public abstract class BaseTable<TValue>
    {
        protected string _tableName;
        protected bool _hasHeader;
        protected int _totalColumns;


        public BaseTable(string tableName, bool hasHeader, int totalColumns)
        {
            _tableName = tableName;
            _hasHeader = hasHeader;
            _totalColumns = totalColumns;
        }


        public virtual void BuildTable(ExcelWorkbook workBook, List<TValue> data, bool clearData = false)
        {
            int toalRosw = data.Count;

            if (_hasHeader)
            {
                toalRosw += 1;
            }

            ExcelWorksheet workSheet = workBook.Worksheets.FirstOrDefault(v => v.Name == _tableName);

            if (workSheet == null)
            {
                workSheet = workBook.Worksheets.Add(_tableName);
            }
            else if (clearData == true) 
            {
                workBook.Worksheets.Delete(workSheet);
                workSheet = workBook.Worksheets.Add(_tableName);
            }

            ExcelTable newTable = workSheet.Tables.FirstOrDefault(v => v.Name == _tableName);

            if (newTable == null)
            {
                string letter = ExcelCellBase.GetAddressCol(_totalColumns);
                newTable = workSheet.Tables.Add(new ExcelAddressBase($"A1:{letter}{toalRosw}"), _tableName);
            }
            PopulateWorksheet(workSheet, newTable, data);

        }

        public virtual void UpdateTable(ExcelWorkbook workBook, List<TValue> data)
        {

        }
        //{
        //    ExcelWorksheet workSheet = (from ws in workBook.Worksheets
        //                                where ws.Name == _tableName
        //                                select ws).FirstOrDefault();

        //    ExcelTable table = (from title in workSheet.Tables
        //                        where title.Name == _tableName
        //                        select title).FirstOrDefault();

        //    workSheet.Tables.Delete(table, true);




        //    //int firstDataRow = table.Address.Start.Row;
        //    //if (_hasHeader)
        //    //{
        //    //    firstDataRow += 1;
        //    //}
        //    //int totalDataRows = table.Address.End.Row;

        //    //if (table.ShowTotal == true)
        //    //{
        //    //    totalDataRows -= 1;
        //    //}


        //}




        protected abstract void PopulateWorksheet(ExcelWorksheet workSheet, ExcelTable table, List<TValue> data);


        public virtual List<TValue> ReadTable(ExcelWorkbook workBook)
        {
            ExcelWorksheet workSheet = (from ws in workBook.Worksheets
                                        where ws.Name == _tableName
                                        select ws).FirstOrDefault();

            ExcelTable table = (from title in workSheet.Tables
                                where title.Name == _tableName
                                select title).FirstOrDefault();

            int firstDataRow = table.Address.Start.Row;
            if (_hasHeader)
            {
                firstDataRow += 1;
            }
            int totalDataRows = table.Address.End.Row;

            if (table.ShowTotal == true)
            {
                totalDataRows -= 1;
            }

            return ReadAllRows(workSheet, firstDataRow, totalDataRows);
        }

        protected abstract List<TValue> ReadAllRows(ExcelWorksheet workSheet, int firstRow, int totalRows);


    }
}
