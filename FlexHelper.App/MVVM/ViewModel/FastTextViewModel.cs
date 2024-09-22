using CommunityToolkit.Mvvm.ComponentModel;
using FlexHelper.App.MVVM.Model;
using System.Collections.ObjectModel;

namespace FlexHelper.App.MVVM.ViewModel
{
    public class FastTextViewModel : ObservableRecipient
    {
        public ObservableCollection<FastTextModel> SavedStrings { get; set; }

        public FastTextViewModel()
        {
            SavedStrings = new()
            {
                new FastTextModel("aaaaa", "A"),
                new FastTextModel("ssssss", "S"),
            };
        }
    }
}
