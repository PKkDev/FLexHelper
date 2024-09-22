using CommunityToolkit.Mvvm.ComponentModel;

namespace FlexHelper.App.MVVM.Model
{
    public class FastTextModel : ObservableObject
    {
        public string Text { get; set; }

        public string Key { get; set; }

        public string Description { get; set; }

        public FastTextModel() { }

        public FastTextModel(string text, string key, string description)
        {
            Text = text;
            Key = key;
            Description = description;
        }
    }
}
