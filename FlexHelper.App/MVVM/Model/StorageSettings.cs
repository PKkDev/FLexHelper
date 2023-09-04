namespace FlexHelper.App.MVVM.Model
{
    public class StorageSettings
    {
        public MouseMoverSettings MouseMoverSettings { get; set; }

        public StorageSettings()
        {
            MouseMoverSettings = null;
        }

        public static StorageSettings CreateDefault()
        {
            return new StorageSettings()
            {
                MouseMoverSettings = new MouseMoverSettings()
                {
                    Distance = 150,
                    Interval = 10,
                    CoefFast = 1
                }
            };
        }
    }

    public class MouseMoverSettings
    {
        public int Distance { get; set; }
        public int Interval { get; set; }
        public int CoefFast { get; set; }

        public MouseMoverSettings() { }
    }
}
