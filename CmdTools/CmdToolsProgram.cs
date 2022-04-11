using CmdTools.Core.CmdMenuAndPages;
using PicturesTool;

namespace CmdTools
{
    internal sealed class CmdToolsProgram : CmdWizard
    {
        public CmdToolsProgram(IIOWrapper ioWrapper) : base("My Cmd Tools", true, ioWrapper)
        {
            AddPage(new MainPage(this, ioWrapper));

            // Pictures Tool
            AddPage(new PicturesToolMainPage(this, ioWrapper));
            AddPage(new MoveRenameMergeWizard(this, ioWrapper));
            AddPage(new MoveFiles(this, ioWrapper));
            AddPage(new RenameFiles(this, ioWrapper));
            AddPage(new CopyPicturesBetweenUsers(this, ioWrapper));
            AddPage(new CorrectCaptureDate(this, ioWrapper));
            AddPage(new GenerateReport(this, ioWrapper));

            SetPage<MainPage>();
        }
    }
}
