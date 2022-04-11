using CmdTools.Core.CmdMenuAndPages;
using CmdTools.Core.UserSettings;
using PicturesTool.Core.Logics;
using Spectre.Console;

namespace PicturesTool
{
    public sealed class MoveFiles : Page
    {
        public MoveFiles(ICmdWizard program, IIOWrapper ioWrapper)
            : base("Moving files", program, ioWrapper)
        {
        }

        protected override void DisplayContent()
        {
            UserSettingsManager settingManager = new UserSettingsManager();
            var settings = settingManager.ReadFromConfigFile();

            if (IOWrapper.GetConfirmation($"Confirm moving pictures from Camera Roll to Temp folder?"))
            {
                foreach (var user in settings.MoveAndOrganize.Users)
                {
                    IOWrapper.WriteLine($"Moving User [green]{user.Username}[/] pictures");
                    MoveFileForUser(user, settings.MoveAndOrganize.Year, settings.MoveAndOrganize.Month);
                }
            }

            Program.NavigateTo<RenameFiles>();
        }

        private void MoveFileForUser(OneDriveSettings oneDrive, int year, int month)
        {
            string sourceFolder = Path.Combine(oneDrive.OneDrivePicturesFolderPath, oneDrive.CameraRolFolderName);

            IOWrapper.WriteLine($"Start searching pictures taken in [green]{month}/{year}[/] in folder path: [green]{sourceFolder}[/]");

            var allPics = Pictures.ReadFiles(new DirectoryInfo(sourceFolder));

            var filteredPics = allPics.Where(v => v.GetCaptureDate().Year == year && v.GetCaptureDate().Month == month);
            var count = filteredPics.Count();

            IOWrapper.WriteLine($"Found [green]{count}[/] pictures");

            string outPath = Path.Combine(oneDrive.OneDrivePicturesFolderPath, $"{year}_{month}");

            if (!Directory.Exists(outPath))
            {
                Directory.CreateDirectory(outPath);
            }

            if (count > 0)
            {
                IOWrapper.WriteLine($"Starting mooving pictures taken in [green]{month}/{year}[/]  to folder path: [green]{outPath}[/] ");


                AnsiConsole.Progress()
                        .Start(ctx =>
                        {
                            var task1 = ctx.AddTask("[green]Mooving files[/]", maxValue: count);

                            int i = 1;
                            foreach (var pic in filteredPics)
                            {
                                pic.File.MoveTo(Path.Combine(outPath, pic.File.Name));
                                i++;
                                task1.Increment(1);
                            }

                        });
            }
        }
    }
}
