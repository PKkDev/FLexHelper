using System.Collections.Generic;

namespace FLexHelper.UIT.MVVM.Model
{
    public class AppSettingsConfig
    {
        public MouseMoverSettings MouseMoverSettings { get; set; }
        public List<RDPConnectSettings> RDPConnectSettings { get; set; }

        public AppSettingsConfig()
        {
            RDPConnectSettings = new List<RDPConnectSettings>();
        }
    }

    public class MouseMoverSettings
    {
        public int Distance { get; set; }
        public int Interval { get; set; }
        public int CoefFast { get; set; }

        public MouseMoverSettings() { }
    }

    public class RDPConnectSettings
    {
        public string KeyName { get; set; }
        public string Adress { get; set; }
        public string Port { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }

        public RDPConnectSettings() { }

        public RDPConnectSettings(string keyName)
        {
            KeyName = keyName;
        }
    }
}
