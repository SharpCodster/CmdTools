using CmdTools.Core.CmdMenuAndPages;

namespace PicturesTool
{
    public sealed class PicturesToolMainPage : MenuPage
    {
        public PicturesToolMainPage(ICmdWizard program, IIOWrapper ioWrapper)
            : base("Pictures Tool", "What do you want to do?", program, ioWrapper,
                  new Option("Move, Rename and Merge Wizard", () => program.NavigateTo<MoveRenameMergeWizard>()),
                  new Option("Correct Capture Date", () => program.NavigateTo<CorrectCaptureDate>()),
                  new Option("Rname and Set Capture Date", () => program.NavigateTo<RenameAndSetCaptureDate>()),
                  new Option("Generate report", () => program.NavigateTo<GenerateReport>()),
                  new Option("Exit", Exit)
                  )
        {
        }

        public static void Exit()
        {
            Environment.Exit(0);
        }
    }
}
