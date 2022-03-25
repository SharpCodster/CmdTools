using CmdTools.Core.CmdMenuAndPages;
using CmdTools.Core.UserSettings;
using PicturesTool.Core.Logics;
using Spectre.Console;

namespace PicturesTool
{
    public class CorrectCaptureDate : Page
    {
        public CorrectCaptureDate(ICmdWizard program, IIOWrapper ioWrapper)
            : base("Correct Capture Date", program, ioWrapper)
        {
        }

        protected override void DisplayContent()
        {
            string folderPath = IOWrapper.ReadString("Insert folder path:");

            int newYear = IOWrapper.ReadInt("Insert new year:", DateTime.Now.Year, 1901, DateTime.Now.Year);
            int newMonth = IOWrapper.ReadInt("Insert new month:", DateTime.Now.Month, 1, 12);
            int newDay = IOWrapper.ReadInt("Insert new day:", DateTime.Now.Day, 1, 31);

            IOWrapper.WriteLine($"Start searching pictures...");

            DirectoryInfo dir = new DirectoryInfo(folderPath);
            var filesFound = Pictures.ReadFiles(dir);

            IOWrapper.WriteLine($"Found [green]{filesFound.Count}[/] pictures");

            IOWrapper.WriteLine(String.Format("\n{0,-30} {1,-30}\n", "Old Date", "New Date"));

            foreach (var item in filesFound)
            {
                //if (item.File.Name.StartsWith("20161004") && item.File.Extension == ".mp4")
                //{
                DateTime oldDate = item.GetCaptureDate();
                DateTime newDate = new DateTime(newYear, newMonth, newDay, oldDate.Hour, oldDate.Minute, oldDate.Second, oldDate.Millisecond);//GetNewDate(oldDate);

                item.SetNewCaptureDate(newDate);
                item.Rename();

                IOWrapper.WriteLine(String.Format("{0,-30} {1,-30}", oldDate.ToString(), newDate.ToString()));
                //}
            }

            IOWrapper.WriteLine("\n");

            if (IOWrapper.GetConfirmation($"Done. Go home?"))
            {

            }

            Program.NavigateHome();
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
