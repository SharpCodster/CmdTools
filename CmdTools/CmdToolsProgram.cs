using CmdTools.Core.CmdMenuAndPages;
using PicturesTool;

namespace CmdTools
{
    internal class CmdToolsProgram : CmdWizard
    {
        public CmdToolsProgram(IIOWrapper ioWrapper) : base("My Cmd Tools", true, ioWrapper)
        {
            AddPage(new MainPage(this, ioWrapper));

            // Pictures Tool
            AddPage(new PicturesToolMainPage(this, ioWrapper));
            AddPage(new MoveRenameMergeWizard(this, ioWrapper));
            //AddPage(new MoveFiles(this));
            //AddPage(new RenameFiles(this));
            //AddPage(new CopyPicturesBetweenUsers(this));
            //AddPage(new Page2(this));
            //AddPage(new InputPage(this));

            SetPage<MainPage>();
        }

    }
}
