using CmdTools.Core.CmdMenuAndPages;
using CmdTools.Core.UserSettings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BilancioTool
{
    public class GenerateForecast : Page
    {
        public GenerateForecast(ICmdWizard program, IIOWrapper ioWrapper)
            : base("Generating forecast", program, ioWrapper)
        {
        }

        protected override void DisplayContent()
        {
            //UserSettingsManager settingManager = new UserSettingsManager();
            //var settings = settingManager.ReadFromConfigFile();

            //string outputFile = IOWrapper.ReadString("Insert report dir: ");

            //List<GenericFile> master = new List<GenericFile>();
            //List<GenericFile> slave = new List<GenericFile>();

            //OneDriveSettings masterUser = settings.MoveAndOrganize.Users[0];
            if (!IOWrapper.GetConfirmation($"Confirm master user as ?"))
            {
                //masterUser = settings.MoveAndOrganize.Users[1];
            }

            //while (IOWrapper.GetConfirmation($"Do you want select particupar folder?"))
            //{
            //    string selectedFolder = IOWrapper.ReadString("Folder inside the pictures dir: ");

            //    if (!Directory.Exists(Path.Combine(masterUser.OneDrivePicturesFolderPath, selectedFolder)))
            //    {
            //        throw new Exception("Directory not found!");
            //    }

            //    foreach (var user in settings.MoveAndOrganize.Users)
            //    {
            //        IOWrapper.WriteLine($"Searching pictures of User [green]{user.Username}[/]");
            //        if (user.Username == masterUser.Username)
            //        {
            //            master = ReadFiles(user, selectedFolder);
            //        }
            //        //else
            //        //{
            //        //    slave = ReadFiles(user, selectedFolder);
            //        //}
            //    }

            //    GetReport(master, slave, outputFile, selectedFolder);
            //}


            //if (IOWrapper.GetConfirmation("Goodbye"))
            //{

            //}

        }


        //private List<GenericFile> ReadFiles(OneDriveSettings oneDrive, string selectedFolder)
        //{
        //    string sourceFolder = Path.Combine(oneDrive.OneDrivePicturesFolderPath, selectedFolder);


        //    IOWrapper.WriteLine($"Start searching pictures taken in [green]{selectedFolder}[/] in folder");



        //    return
        //        Pictures.ReadFiles(new DirectoryInfo(sourceFolder));
        //}


        //private void GetReport(List<GenericFile> master, List<GenericFile> slave, string outFile, string selectedFolder)
        //{
        //    StringBuilder sb = new StringBuilder();

        //    sb.AppendLine($"\"Path\";\"FileName\";\"Extension\"");

        //    foreach (var file in master)
        //    {
        //        var pathSplittes = file.File.DirectoryName.Split(selectedFolder);
        //        var path = pathSplittes[pathSplittes.Length - 1];

        //        var fileName = file.File.Name;
        //        var extension = file.File.Extension;

        //        sb.AppendLine($"\"{path}\";\"{fileName}\";\"{extension}\"");
        //    }

        //    string filePat = Path.Combine(outFile, $"{DateTime.Now.ToString("yyyyMMddhhmmss")} - Report for {selectedFolder}.csv");

        //    File.WriteAllText(filePat, sb.ToString());
        //}
    }
}
