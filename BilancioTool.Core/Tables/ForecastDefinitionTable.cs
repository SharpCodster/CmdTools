﻿using BilancioTool.Core.Entities;
using OfficeOpenXml.Table;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace BilancioTool.Core.Tables
{
    public class ForecastDefinitionTable : BaseTable<ForecastDefinition>
    {
        public ForecastDefinitionTable() : base("Definitions", true, 8)
        {

        }

        public ForecastDefinitionTable(string tableName) : base(tableName, true, 8)
        {

        }

        protected override void PopulateWorksheet(ExcelWorksheet workSheet, ExcelTable table, List<ForecastDefinition> data)
        {
            throw new NotImplementedException();
        }

        protected override List<ForecastDefinition> ReadAllRows(ExcelWorksheet workSheet, int firstRow, int totalRows)
        {
            List<ForecastDefinition> data = new List<ForecastDefinition>();

            for (int i = firstRow; i <= totalRows; i++)
            {
                ForecastDefinition row = new ForecastDefinition();

                row.PartID = workSheet.GetValue<string>(i, 1);
                row.Year = workSheet.GetValue<string>(i, 2);
                row.Month = workSheet.GetValue<string>(i, 3);
                row.Day = workSheet.GetValue<string>(i, 4);
                row.DayOfWeek = workSheet.GetValue<string>(i, 5);
                row.AccountFrom = workSheet.GetValue<string>(i, 6);
                row.AccountTo = workSheet.GetValue<string>(i, 7);
                row.Total = workSheet.GetValue<double>(i, 8);

                data.Add(row);
            }

            return data;

        }
    }
}
