using FlexHelper.App.MVVM.Model;
using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Windows.Storage;

namespace FlexHelper.App.Services
{
    public class ConfigService
    {
        private const string FileName = "config.json";

        private StorageSettings AppSettings { get; set; }
        private StorageFile AppSettingsFile { get; set; }

        public ConfigService()
        {
            SetOrCreateConfig().GetAwaiter().GetResult();
        }

        private async Task SetOrCreateConfig()
        {
            var path = Path.Combine(ApplicationData.Current.LocalFolder.Path, FileName);
            FileInfo info = new(path);
            if (!info.Exists)
            {
                AppSettingsFile = await ApplicationData.Current.LocalFolder
                       .CreateFileAsync(FileName, CreationCollisionOption.ReplaceExisting);

                AppSettings = StorageSettings.CreateDefault();

                var text = JsonSerializer.Serialize(AppSettings);
                File.WriteAllText(path, text);
            }
            else
            {
                var text = File.ReadAllText(path);
                if (string.IsNullOrEmpty(text))
                {
                    File.Delete(path);
                    await SetOrCreateConfig();
                }

                AppSettingsFile = StorageFile.GetFileFromPathAsync(path).GetAwaiter().GetResult();
                AppSettings = JsonSerializer.Deserialize<StorageSettings>(text);
            }
        }

        public StorageSettings GetConfig() => AppSettings;

        public void UpdateConfig(StorageSettings config)
        {
            if (AppSettingsFile != null)
            {
                var text = JsonSerializer.Serialize(config);

                File.WriteAllText(AppSettingsFile.Path, text);
                AppSettings = config;
            }
        }
    }
}
