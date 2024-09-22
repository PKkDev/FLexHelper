using System.Collections.Generic;

namespace FlexHelper.App.MVVM.Model
{
    public class StorageSettings
    {
        public MouseMoverSettings MouseMoverSettings { get; set; }
        public List<FastTextModel> FastTextModelSettings { get; set; }

        public StorageSettings()
        {
            MouseMoverSettings = null;
            FastTextModelSettings = new();
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
                },
                FastTextModelSettings = new()
                {
                    new FastTextModel("Portnov.KA", "A", "Пользователь"),
                    new FastTextModel("GjhnyjdRbhbkk!2345", "S", "Пароль"),
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
