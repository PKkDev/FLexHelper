using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FlexHelper.App.MVVM.Model;
using FlexHelper.App.Services;
using System.Collections.ObjectModel;

namespace FlexHelper.App.MVVM.ViewModel
{
    public class FastTextViewModel : ObservableRecipient
    {
        public ObservableCollection<FastTextModel> SavedStrings { get; set; }

        public RelayCommand ClickSaveCmd { get; set; }

        private StorageSettings Config { get; set; }
        private readonly ConfigService _configService;

        public FastTextViewModel(ConfigService configService)
        {
            _configService = configService;
            Config = configService.GetConfig();

            SavedStrings = new(Config.FastTextModelSettings);

            ClickSaveCmd = new RelayCommand(() =>
            {
                //Config.FastTextModelSettings = [.. SavedStrings];
                _configService.UpdateConfig(Config);
            });
        }
    }
}
