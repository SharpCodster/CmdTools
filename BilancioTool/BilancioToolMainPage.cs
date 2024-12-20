﻿using CmdTools.Core.CmdMenuAndPages;
using OfficeOpenXml;

namespace BilancioTool
{
    public class BilancioToolMainPage : MenuPage
    { 
        public BilancioToolMainPage(ICmdWizard program, IIOWrapper ioWrapper)
            : base("Bilancio Tool", "What do you want to do?", program, ioWrapper,
                  new Option("Import Data", () => program.NavigateTo<ImportDataPage>()),
                  new Option("Consolidate File", () => program.NavigateTo<ConsolidateFile>()),
                  new Option("Migrate To Codio", () => program.NavigateTo <MigrateToCodio>()),
                  new Option("Generate Forecastd", () => program.NavigateTo<GenerateForecastPage>()),
                  new Option("Exit", Exit)
                  )
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        }

        public static void Exit()
        {
            Environment.Exit(0);
        }
    }
}