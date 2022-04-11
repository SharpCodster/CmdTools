using CmdTools.Core.CmdMenuAndPages;
using CmdTools.Core.UserSettings;
using Spectre.Console;

namespace PicturesTool
{
    public sealed class CopyPicturesBetweenUsers : Page
    {
        public CopyPicturesBetweenUsers(ICmdWizard program, IIOWrapper ioWrapper)
            : base("Coping Pictures between users", program, ioWrapper)
        {
        }

        protected override void DisplayContent()
        {
            UserSettingsManager settingManager = new UserSettingsManager();
            var settings = settingManager.ReadFromConfigFile();

            IOWrapper.WriteLine("[red]Before start remeber to organize files in folders, with (fam) at the end for family pictures![/]");

            foreach (var user in settings.MoveAndOrganize.Users)
            {
                IOWrapper.WriteLine($"Moving User [green]{user.Username}[/] pictures");

                if (IOWrapper.GetConfirmation($"Confirm moving pictures from temp folder to destination folder?"))
                {
                    IOWrapper.WriteLine("");
                    MoveFileForUser(user, settings.MoveAndOrganize.Year, settings.MoveAndOrganize.Month);
                }

                if (IOWrapper.GetConfirmation($"Confirm moving folders (no fam) from temp folder to destination folder?"))
                {
                    IOWrapper.WriteLine("");
                    MoveFolderForUser(user, settings.MoveAndOrganize.Year, settings.MoveAndOrganize.Month);
                }
            }

            if (IOWrapper.GetConfirmation($"Confirm copying fam folders from User [green]{settings.MoveAndOrganize.Users[0].Username}[/] to [green]{settings.MoveAndOrganize.Users[1].Username}[/]?"))
            {
                IOWrapper.WriteLine("");
                CopyFolderForUserFam(settings.MoveAndOrganize.Users[0], settings.MoveAndOrganize.Users[1], settings.MoveAndOrganize.Year, settings.MoveAndOrganize.Month);
            }

            if (IOWrapper.GetConfirmation($"Confirm copying fam folders from User [green]{settings.MoveAndOrganize.Users[1].Username}[/] to [green]{settings.MoveAndOrganize.Users[0].Username}[/]?"))
            {
                IOWrapper.WriteLine("");
                CopyFolderForUserFam(settings.MoveAndOrganize.Users[1], settings.MoveAndOrganize.Users[0], settings.MoveAndOrganize.Year, settings.MoveAndOrganize.Month);
            }

            foreach (var user in settings.MoveAndOrganize.Users)
            {
                IOWrapper.WriteLine($"Moving User fam [green]{user.Username}[/] pictures");

                if (IOWrapper.GetConfirmation($"Confirm moving folders (fam) from temp folder to destination folder?"))
                {
                    IOWrapper.WriteLine("");
                    MoveFolderForUser(user, settings.MoveAndOrganize.Year, settings.MoveAndOrganize.Month, true);
                }
            }

            if (IOWrapper.GetConfirmation($"Done! Do you want go to home?"))
            {
                Program.NavigateHome();
            }
        }

        private void MoveFileForUser(OneDriveSettings user, int year, int month)
        {
            string sourceDir = Path.Combine(user.OneDrivePicturesFolderPath, $"{year}_{month}");
            string destDir = Path.Combine(user.OneDrivePicturesFolderPath, $"{year}");
            MoveOrCopyFilesInDirs(sourceDir, destDir, true);
        }

        private void MoveFolderForUser(OneDriveSettings user, int year, int month, bool fam = false)
        {
            string sourceDir = Path.Combine(user.OneDrivePicturesFolderPath, $"{year}_{month}");
            var allDirs = Directory.EnumerateDirectories(sourceDir);

            var dirs = allDirs.Where(v => v.EndsWith("(fam)") == fam);

            string destDir = Path.Combine(user.OneDrivePicturesFolderPath, $"{year}");

            foreach (var dir in dirs)
            {
                var dirInfo = new DirectoryInfo(dir);

                var dest = Path.Combine(destDir, dirInfo.Name);
                if (!Directory.Exists(dest))
                {
                    Directory.CreateDirectory(dest);
                }
                MoveOrCopyFilesInDirs(dir, dest, true);
            }
        }

        private void CopyFolderForUserFam(OneDriveSettings user1, OneDriveSettings user2, int year, int month)
        {
            string sourceDir = Path.Combine(user1.OneDrivePicturesFolderPath, $"{year}_{month}");
            var allDirs = Directory.EnumerateDirectories(sourceDir);

            var dirs = allDirs.Where(v => v.EndsWith("(fam)"));

            string destDir = Path.Combine(user2.OneDrivePicturesFolderPath, $"{year}");

            foreach (var dir in dirs)
            {
                var dirInfo = new DirectoryInfo(dir);

                var dest = Path.Combine(destDir, dirInfo.Name);
                if (!Directory.Exists(dest))
                {
                    Directory.CreateDirectory(dest);
                }
                MoveOrCopyFilesInDirs(dir, dest, false);
            }
        }

        private void MoveOrCopyFilesInDirs(string sourceDir, string destinationDir, bool move = false)
        {
            var sourceDirInfo = new DirectoryInfo(sourceDir);

            if (!sourceDirInfo.Exists)
                throw new DirectoryNotFoundException($"Source directory not found: {sourceDirInfo.FullName}");

            var destinationDirInfo = new DirectoryInfo(destinationDir);
            if (!destinationDirInfo.Exists)
            {
                Directory.CreateDirectory(destinationDir);
            }

            var allFiles = sourceDirInfo.GetFiles();

            string action = "coping";
            if (move)
            {
                action = "moving";
            }

            if (allFiles.Length > 0)
            {
                IOWrapper.WriteLine($"Starting {action} from [green]'{sourceDir}'[/] to [green]'{destinationDir}'[/] ");


                AnsiConsole.Progress()
                        .Start(ctx =>
                        {
                            var task1 = ctx.AddTask($"[green]{action} files[/]", maxValue: allFiles.Length);

                            foreach (FileInfo file in allFiles)
                            {
                                string targetFilePath = Path.Combine(destinationDir, file.Name);

                                if (move)
                                {
                                    file.MoveTo(targetFilePath);
                                }
                                else
                                {
                                    file.CopyTo(targetFilePath);
                                }
                                task1.Increment(1);
                            }
                        });
            }
        }
    }
}
