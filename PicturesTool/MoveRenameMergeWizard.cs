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

            Program.NavigateTo<MoveFiles>();
        }


        private void CheckUsers(UserSettings settings)
        {
            // per ogni utente: confirm, update, delete
            // alla fine chiedo se vuole aggiungerne uno

            IOWrapper.WriteLine("[lightcoral]Users:[/]");

            var users = String.Join("; ", settings.MoveAndOrganize.Users.Select(v => v.Username));

            IOWrapper.WriteLine($"Currently are [green]{settings.MoveAndOrganize.Users.Count} [[{users}]][/] users stored in config file");
            IOWrapper.WriteLine("");


            foreach (var user in settings.MoveAndOrganize.Users)
            {
                CheckUserData(user);
            }
        }

        private void CheckUserData(OneDriveSettings userSettings)
        {
            IOWrapper.WriteLine($"[lightcoral]User data:[/]");
            IOWrapper.WriteLine($"Username: {userSettings.Username}");
            IOWrapper.WriteLine($"OneDrive picture path: {userSettings.OneDrivePicturesFolderPath}");
            IOWrapper.WriteLine($"Camera Roll folder name: {userSettings.CameraRolFolderName}");
            IOWrapper.WriteLine("");

            Menu menu = new Menu(IOWrapper);

            menu.Add(new Option("Continue", null));
            menu.Add(new Option("Edit", () => EdituserData(userSettings)));
            //menu.Add(new Option("Delete", null));

            menu.Display("");
        }

        private void EdituserData(OneDriveSettings userSettings)
        {
            userSettings.Username = IOWrapper.ReadString("Username:");
            userSettings.OneDrivePicturesFolderPath = IOWrapper.ReadString("OneDrive picture path:");
            userSettings.CameraRolFolderName = IOWrapper.ReadString("Camera Roll folder name:");
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

            IOWrapper.WriteLine("[lightcoral]Month and Year:[/]");
            settings.MoveAndOrganize.Year = IOWrapper.ReadInt("Insert year: ", settings.MoveAndOrganize.Year, 1900, DateTime.Now.Year);
            settings.MoveAndOrganize.Month = IOWrapper.ReadInt("Insert month: ", settings.MoveAndOrganize.Month, 1, 12);

            IOWrapper.WriteLine("");
        }

    }
}
