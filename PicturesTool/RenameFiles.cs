using CmdTools.Core.CmdMenuAndPages;
using CmdTools.Core.UserSettings;
using PicturesTool.Core.Logics;


namespace PicturesTool
{
    public sealed class RenameFiles : Page
    {
        public RenameFiles(ICmdWizard program, IIOWrapper ioWrapper)
            : base("Renaming files", program, ioWrapper)
        {
        }

        protected override void DisplayContent()
        {
            UserSettingsManager settingManager = new UserSettingsManager();
            var settings = settingManager.ReadFromConfigFile();

            if (IOWrapper.GetConfirmation($"Confirm renaming pictures in Temp folder?"))
            {
                foreach (var user in settings.MoveAndOrganize.Users)
                {
                    IOWrapper.WriteLine($"Renaming User [green]{user.Username}[/] pictures");
                    RenameFilesForUser(user, settings.MoveAndOrganize.Year, settings.MoveAndOrganize.Month);

                    if (IOWrapper.GetConfirmation($"Press [[Enter]] when ready."))
                    {
                    }
                }
            }

            Program.NavigateTo<CopyPicturesBetweenUsers>();
        }

        private void RenameFilesForUser(OneDriveSettings oneDrive, int year, int month)
        {
            string outPath = Path.Combine(oneDrive.OneDrivePicturesFolderPath, $"{year}_{month}");

            IOWrapper.WriteLine($"Start searching pictures in folder path: [green]{outPath}[/]");

            DirectoryInfo dir = new DirectoryInfo(outPath);
            var filesFound = Pictures.ReadFiles(dir);

            IOWrapper.WriteLine($"Found [green]{filesFound.Count}[/] pictures");

            IOWrapper.WriteLine(String.Format("\n{0,-30} {1,-30}\n", "Old Name", "New Name"));

            foreach (var item in filesFound)
            {
                string oldName = item.File.Name;
                item.Rename();
                IOWrapper.WriteLine(String.Format("{0,-30} {1,-30}", oldName, item.NewName));
            }
        }
    }
}
