﻿using CmdTools.Core.Entities;
using Newtonsoft.Json;

namespace CmdTools.Core.UserSettings
{
    public sealed class UserSettingsManager
    {
        public string FolderPath
        {
            get
            {
                return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Codster/CmdTools");
            }
        }
        public string FilePath
        {
            get
            {
                return Path.Combine(FolderPath, "UserSettings.json");
            }
        }

        public UserSettingsManager()
        {
            if (!Directory.Exists(FolderPath))
            {
                Directory.CreateDirectory(FolderPath);
            }

            if (!File.Exists(FilePath))
            {
                UserSettings readedPreferences = UserPreferencesPopulatedDefaults();
                SaveToConfigFile(readedPreferences);
            }
        }

        public UserSettings ReadFromConfigFile()
        {
            string fileContent = File.ReadAllText(FilePath);
            UserSettings readedPreferences = JsonConvert.DeserializeObject<UserSettings>(fileContent);
            return readedPreferences;
        }

        public void SaveToConfigFile(UserSettings userSettings)
        {
            string json = JsonConvert.SerializeObject(userSettings);
            File.WriteAllText(FilePath, json);
        }

        private UserSettings UserPreferencesPopulatedDefaults()
        {
            UserSettings userDefaults = new UserSettings
            {
                MoveAndOrganize = new MoveAndOrganizeSettings()
                {
                    Month = 1,
                    Year = 2000,
                    Users = new List<OneDriveSettings>()
                    {
                        new OneDriveSettings()
                        {
                            Username = "User1",
                            OneDrivePicturesFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures),
                            CameraRolFolderName = "Camera Roll"
                        },
                        new OneDriveSettings()
                        {
                            Username = "User2",
                            OneDrivePicturesFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures),
                            CameraRolFolderName = "Camera Roll"
                        }
                    }
                },
                BilancioSettings = new BilancioUserSettings()
                {
                    ImportFolder = "C:\\Users\\mfusi\\OneDrive\\Documenti\\Finanza\\ImportFiles",
                    BooktFolder = "C:\\Users\\mfusi\\OneDrive\\Documenti\\Finanza\\Libri Mastri",
                    ForecastFolder = "C:\\Users\\mfusi\\OneDrive\\Documenti\\Finanza\\Previsioni",
                    DevCredential = new Credential()
                    {
                        Email = "Username",
                        Password = "Password"
                    }
                }
            };

            return userDefaults;
        }
    }
}

