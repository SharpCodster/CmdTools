using CmdTools.Core.CmdMenuAndPages;
using PicturesTool;
using BilancioTool;

namespace CmdTools
{
    internal sealed class MainPage : MenuPage
    {
        public MainPage(ICmdWizard program, IIOWrapper ioWrapper)
            : base("Main Page", "Which tool do you want to use?", program, ioWrapper,
                  new Option("Bilancio Tool Page", () => program.NavigateTo<BilancioToolMainPage>()),
                  new Option("Pictures Tool Page", () => program.NavigateTo<PicturesToolMainPage>()),
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
