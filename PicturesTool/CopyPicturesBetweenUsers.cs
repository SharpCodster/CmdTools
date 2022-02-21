using CmdTools.Core.CmdMenuAndPages;
using CmdTools.Core.UserSettings;
using PicturesTool.Core.Logics;
using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PicturesTool
{
    public class CopyPicturesBetweenUsers : Page
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

            // spostare file nella temp nell'anno utente 1
            // spostare file nella temp nell'anno utente 2
            // spostare folder non (fam) nell'anno utente 1
            // spostare folder non (fam) nell'anno utente 2
            // Copiare folder (fam) nell'anno dell'altro utente
            // Copiare foldere (fam) nell'anno dell'altro utente
            // Mergiare i file delle folter (fam) in temp nell'anno utente 1
            // Mergiare i file delle folter (fam) in temp nell'anno utente 2


            if (IOWrapper.GetConfirmation($"Confirm moving temp sub folders to destination folder?"))
            {
                foreach (var user in settings.MoveAndOrganize.Users)
                {
                    if (IOWrapper.GetConfirmation($"Skip user [green]{user.Username}[/]?", false))
                    {
                        IOWrapper.WriteLine("");
                        IOWrapper.WriteLine($"Moving User [green]{user.Username}[/] pictures");
                        MoveFileForUser(user, settings.MoveAndOrganize.Year, settings.MoveAndOrganize.Month);
                    }
                }
            }



            Program.NavigateHome();
        }

        private void MoveFileForUser(OneDriveSettings user, int year, int month)
        {
            string sourceDir = Path.Combine(user.OneDrivePicturesFolderPath, $"{year}_{month}");
            string destDir = Path.Combine(user.OneDrivePicturesFolderPath, $"{year}");
            MoveOrCopyFilesInDirs(sourceDir, destDir, true);
        }

        private void MoveFolderForUser(OneDriveSettings user, int year, int month)
        {
            string sourceDir = Path.Combine(user.OneDrivePicturesFolderPath, $"{year}_{month}");
            var allDirs = Directory.EnumerateDirectories(sourceDir);

            var dirs = allDirs.Where(v => !v.EndsWith("(fam)"));

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
