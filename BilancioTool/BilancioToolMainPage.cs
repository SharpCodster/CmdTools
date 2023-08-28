using CmdTools.Core.CmdMenuAndPages;

namespace BilancioTool
{
    public class BilancioToolMainPage : MenuPage
    {
        public BilancioToolMainPage(ICmdWizard program, IIOWrapper ioWrapper)
            : base("Bilancio Tool", "What do you want to do?", program, ioWrapper,
                  new Option("Generate Forecastd", () => program.NavigateTo<GenerateForecast>()),
                  //new Option("Correct Capture Date", () => program.NavigateTo<CorrectCaptureDate>()),
                  //new Option("Rname and Set Capture Date", () => program.NavigateTo<RenameAndSetCaptureDate>()),
                  //new Option("Generate report", () => program.NavigateTo<GenerateReport>()),
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