﻿using BilancioTool;
using CmdTools.Core.CmdMenuAndPages;
using PicturesTool;

namespace CmdTools
{
    internal sealed class CmdToolsProgram : CmdWizard
    {
        public CmdToolsProgram(IIOWrapper ioWrapper) : base("My Cmd Tools", true, ioWrapper)
        {
            AddPage(new MainPage(this, ioWrapper));

            // Bilancio Tool
            AddPage(new BilancioToolMainPage(this, ioWrapper));
            AddPage(new GenerateForecastPage(this, ioWrapper));
            AddPage(new ImportDataPage(this, ioWrapper));
            AddPage(new ConsolidateFile(this, ioWrapper));

            // Pictures Tool
            AddPage(new PicturesToolMainPage(this, ioWrapper));
            AddPage(new MoveRenameMergeWizard(this, ioWrapper));
            AddPage(new MoveFiles(this, ioWrapper));
            AddPage(new RenameFiles(this, ioWrapper));
            AddPage(new CopyPicturesBetweenUsers(this, ioWrapper));
            AddPage(new CorrectCaptureDate(this, ioWrapper));
            AddPage(new GenerateReport(this, ioWrapper));
            AddPage(new RenameAndSetCaptureDate(this, ioWrapper));
            AddPage(new MigrateToCodio(this, ioWrapper));


            SetPage<MainPage>();
        }
    }
}
