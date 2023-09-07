namespace CmdTools.Core.UserSettings
{
    public sealed class UserSettings
    {
        public MoveAndOrganizeSettings MoveAndOrganize { get; set; }
        public BilancioUserSettings BilancioSettings { get; set; }
    }

    public sealed class BilancioUserSettings
    {
        public string ImportFolder { get; set; }
        public string BooktFolder { get; set; }
        public string ForecastFolder { get; set; }
    }



    public sealed class OneDriveSettings
    {
        public string Username { get; set; }
        public string OneDrivePicturesFolderPath { get; set; }
        public string CameraRolFolderName { get; set; }
    }

    public sealed class MoveAndOrganizeSettings
    {
        public List<OneDriveSettings> Users { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
    }
}
