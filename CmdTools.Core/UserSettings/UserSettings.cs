namespace CmdTools.Core.UserSettings
{
    public class UserSettings
    {
        public MoveAndOrganizeSettings MoveAndOrganize { get; set; }
    }

    public class OneDriveSettings
    {
        public string Username { get; set; }
        public string OneDrivePicturesFolderPath { get; set; }
        public string CameraRolFolderName { get; set; }
    }

    public class MoveAndOrganizeSettings
    {
        public List<OneDriveSettings> Users { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
    }
}
