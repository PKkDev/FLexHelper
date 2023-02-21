using FLexHelper.UIT.MVVM.Model;
using System;
using System.IO;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace FLexHelper.UIT.Services
{
    public static class ConfiguratorService
    {
        private static AppSettingsConfig _appSettings;

        public static AppSettingsConfig GetConfig()
        {
            return _appSettings;
        }

        public static void GetConfigFromFile()
        {
            var pathToApp = AppDomain.CurrentDomain.BaseDirectory;
            var pathToFolder = Path.Combine(pathToApp, "config");

            var directoryInfo = new DirectoryInfo(pathToFolder);
            if (!directoryInfo.Exists)
                directoryInfo.Create();

            var pathToFile = Path.Combine(pathToFolder, "config.json");

            var fileInfo = new FileInfo(pathToFile);
            if (!fileInfo.Exists)
                SetDefaoultConfig(pathToFile);
            else
            {
                //using StreamReader file = File.OpenText(pathToFile);
                //JsonSerializer serializer = new JsonSerializer();
                //var Movie = (AppSettingsConfig)serializer.Deserialize(file, typeof(AppSettingsConfig));

                var jsonString = File.ReadAllText(pathToFile);
                _appSettings = JsonConvert.DeserializeObject<AppSettingsConfig>(jsonString);
            }
        }

        private static void SetDefaoultConfig(string pathToFile)
        {
            _appSettings = new AppSettingsConfig()
            {
                MouseMoverSettings = new MouseMoverSettings()
                {
                    Distance = 150,
                    Interval = 10,
                    CoefFast = 1
                },
                RDPConnectSettings = new List<RDPConnectSettings>()
            };

            using StreamWriter file = File.CreateText(pathToFile);
            JsonSerializer serializer = new JsonSerializer();
            serializer.Serialize(file, _appSettings);
        }

        public static void UpdateConfig(AppSettingsConfig config)
        {
            var pathToApp = AppDomain.CurrentDomain.BaseDirectory;
            var pathToFile = Path.Combine(pathToApp, "config\\config.json");

            var fileInfo = new FileInfo(pathToFile);
            if (fileInfo.Exists)
            {
                using StreamWriter file = File.CreateText(pathToFile);
                JsonSerializer serializer = new JsonSerializer();
                serializer.Serialize(file, config);
            }
        }
    }
}
