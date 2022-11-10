using CmdTools.Core.CmdMenuAndPages;
using CmdTools.Core.UserSettings;
using PicturesTool.Core.Logics;

namespace PicturesTool
{
    public class RenameAndSetCaptureDate : Page
    {
        public RenameAndSetCaptureDate(ICmdWizard program, IIOWrapper ioWrapper)
            : base("Renaming and set capture date", program, ioWrapper)
        {
        }

        protected override void DisplayContent()
        {
            string folderPath = IOWrapper.ReadString("Insert folder path:");

            IOWrapper.WriteLine($"Start searching pictures...");

            DirectoryInfo dir = new DirectoryInfo(folderPath);
            var filesFound = Pictures.ReadFiles(dir);

            IOWrapper.WriteLine($"Found [green]{filesFound.Count}[/] pictures");

            foreach (var item in filesFound)
            {
                item.Rename();
            }

            IOWrapper.WriteLine("\n");

            if (IOWrapper.GetConfirmation($"Done. Go home?"))
            {

            }

            Program.NavigateHome();
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
