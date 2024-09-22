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

        private readonly StorageFolder _folder;

        private StorageSettings AppSettings { get; set; }
        private StorageFile AppSettingsFile { get; set; }

        public ConfigService()
        {
            if (App.IsNonePackage)
            {
                var baseDir = AppDomain.CurrentDomain.BaseDirectory;
                var path = Path.Combine(baseDir, "LocalAppData");

                var fi = new DirectoryInfo(path);
                if (!fi.Exists)
                    fi.Create();

                var task = StorageFolder.GetFolderFromPathAsync(path);
                task.AsTask().Wait();
                var res = task.GetResults();

                _folder = res;
            }
            else
            {
                _folder = ApplicationData.Current.LocalFolder;
            }

            SetOrCreateConfig().GetAwaiter().GetResult();
        }

        private async Task SetOrCreateConfig()
        {
            var path = Path.Combine(_folder.Path, FileName);
            FileInfo info = new(path);
            if (!info.Exists)
            {
                AppSettingsFile = _folder
                    .CreateFileAsync(FileName, CreationCollisionOption.ReplaceExisting)
                    .GetAwaiter()
                    .GetResult();

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
                else
                {
                    AppSettingsFile = StorageFile.GetFileFromPathAsync(path).GetAwaiter().GetResult();
                    AppSettings = JsonSerializer.Deserialize<StorageSettings>(text);
                }
            }
        }

        public StorageSettings GetConfig() => AppSettings ?? StorageSettings.CreateDefault();

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
