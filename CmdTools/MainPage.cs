using CmdTools.Core.CmdMenuAndPages;
using PicturesTool;

namespace CmdTools
{
    internal class MainPage : MenuPage
    {
        public MainPage(ICmdWizard program, IIOWrapper ioWrapper)
            : base("Main Page", "Which tool do you want to use?", program, ioWrapper,
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
