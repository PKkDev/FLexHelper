using CommunityToolkit.Mvvm.ComponentModel;

namespace FlexHelper.App.MVVM.Model
{
    public class FastTextModel : ObservableObject
    {
        public string Text { get; set; }

        public string Key { get; set; }

        public FastTextModel() { }

        public FastTextModel(string text, string key)
        {
            Text = text;
            Key = key;
        }
    }
}
