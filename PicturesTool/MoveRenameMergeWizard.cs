using CmdTools.Core.CmdMenuAndPages;
using CmdTools.Core.UserSettings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PicturesTool
{
    public class MoveRenameMergeWizard : Page
    {
        public MoveRenameMergeWizard(ICmdWizard program, IIOWrapper ioWrapper)
            : base("Move, Rename and Merge Wizard", program, ioWrapper)
        {
        }

        protected override void DisplayContent()
        {
            UserSettingsManager settingManager = new UserSettingsManager();
            var settings = settingManager.ReadFromConfigFile();

            CheckYearAndNMonth(settings);
            CheckUsers(settings);

            settingManager.SaveToConfigFile(settings);

            //Program.NavigateTo<MoveFiles>();
        }


        private void CheckUsers(UserSettings settings)
        {
            // per ogni utente: confirm, update, delete
            // alla fine chiedo se vuole aggiungerne uno

            IOWrapper.WriteLine("Users:");
            IOWrapper.WriteLine("");

            IOWrapper.WriteLine($"Currently are {settings.MoveAndOrganize.Users.Count} users stored in config file");

            var users = String.Join("; ", settings.MoveAndOrganize.Users.Select(v => v.Username));

            IOWrapper.WriteLine($"[ {users} ]");


            foreach (var user in settings.MoveAndOrganize.Users)
            {
                CheckUserData(user);

                


            //        Output.WriteLine($"OneDrive folder: {userArray[i - 1].OneDrivePath}");
            //        var readedsring = Input.ReadString("Insert new path or press [Enter] to confirm: ");

            //        if (!String.IsNullOrEmpty(readedsring))
            //        {
            //            if (Directory.Exists(readedsring))
            //            {
            //                userArray[i - 1].OneDrivePath = readedsring;
            //            }
            //        }
            //        Output.WriteLine("");

            //        Output.WriteLine($"Camera Roll folder: {userArray[i - 1].CameraRolFolder}");
            //        readedsring = Input.ReadString("Insert new path or press [Enter] to confirm: ");

            //        if (!String.IsNullOrEmpty(readedsring))
            //        {
            //            userArray[i - 1].CameraRolFolder = readedsring;
            //        }
            //        Output.WriteLine("");
            }

            //    Console.Clear();
            //    base.Display();
        }

        private void CheckUserData(OneDriveSettings userSettings)
        {
            IOWrapper.WriteLine($"User data:");
            IOWrapper.WriteLine("");

            IOWrapper.WriteLine($"Username: {userSettings.Username}");
            IOWrapper.WriteLine($"Username: {userSettings.OneDrivePicturesFolderPath}");
            IOWrapper.WriteLine($"Username: {userSettings.CameraRolFolderName}");
            IOWrapper.WriteLine("");

            Menu menu = new Menu(IOWrapper);

            menu.Add(new Option("Continue", null));
            menu.Add(new Option("Edit", null));
            menu.Add(new Option("Delete", null));

            menu.Display("");
        }




        private void CheckYearAndNMonth(UserSettings settings)
        {
            if (settings.MoveAndOrganize.Month >= 12)
            {
                settings.MoveAndOrganize.Month = 1;
                settings.MoveAndOrganize.Year++;
            }
            else
            {
                settings.MoveAndOrganize.Month++;
            }

            IOWrapper.WriteLine("Month and Year:");
            IOWrapper.WriteLine("");






            IOWrapper.WriteLine($"Year: {settings.MoveAndOrganize.Year}");
            int? newYear = IOWrapper.ReadInt("Insert year: ", settings.MoveAndOrganize.Year, 1900, DateTime.Now.Year);
            if(newYear != null)
            {
                settings.MoveAndOrganize.Year = (int)newYear;
            }
            IOWrapper.WriteLine("");

            IOWrapper.WriteLine($"Month: {settings.MoveAndOrganize.Month}");
            var newMonth = IOWrapper.ReadInt("Insert new month or press  to confirm: ", settings.MoveAndOrganize.Month, 1, 12);
            if (newMonth != null)
            {
                settings.MoveAndOrganize.Month = (int)newMonth;
            }
            IOWrapper.WriteLine("");

        }

    }
}
