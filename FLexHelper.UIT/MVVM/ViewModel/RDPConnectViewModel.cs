using FLexHelper.UIT.Core;
using FLexHelper.UIT.MVVM.Model;
using FLexHelper.UIT.Services;
using Microsoft.Win32;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;

namespace FLexHelper.UIT.MVVM.ViewModel
{
    public class RDPConnectViewModel : ObservableObject
    {
        public ObservableCollection<RDPConnectSettings> Connections { get; set; }

        private string _newConnectionKey;
        public string NewConnectionKey
        {
            get { return _newConnectionKey; }
            set { OnSetNewValue(ref _newConnectionKey, value); }
        }

        private RDPConnectSettings _selectedConnection;
        public RDPConnectSettings SelectedConnection
        {
            get { return _selectedConnection; }
            set { OnSetNewValue(ref _selectedConnection, value); }
        }

        public RelyCommand SaveConfigCommand { get; set; }
        public RelyCommand AddConnectCommand { get; set; }
        public RelyCommand RemoveConnectCommand { get; set; }
        public RelyCommand GeneraeCommand { get; set; }

        private AppSettingsConfig Config;

        public RDPConnectViewModel()
        {
            Connections = new ObservableCollection<RDPConnectSettings>();
            Config = ConfiguratorService.GetConfig();
            foreach (var config in Config.RDPConnectSettings)
                Connections.Add(config);

            if (Connections.Any())
                SelectedConnection = Connections.First();

            SaveConfigCommand = new RelyCommand(param =>
            {
                Config.RDPConnectSettings = Connections.ToList();
                ConfiguratorService.UpdateConfig(Config);
            });

            AddConnectCommand = new RelyCommand(param =>
            {
                var search = Connections.FirstOrDefault(x => x.KeyName == NewConnectionKey);
                if (search != null)
                {
                    MessageBox.Show($"{NewConnectionKey} already exist");
                }
                else
                {
                    Connections.Add(new RDPConnectSettings(NewConnectionKey));
                }
            });

            RemoveConnectCommand = new RelyCommand(param =>
            {
                var search = Connections.FirstOrDefault(x => x.KeyName == SelectedConnection.KeyName);
                if (search != null)
                {
                    Connections.Remove(search);
                }
                else
                {
                    MessageBox.Show($"{SelectedConnection.KeyName} not found");
                }
            });

            GeneraeCommand = new RelyCommand((param) =>
            {
                using Process process = new Process();
                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.UseShellExecute = false;
                startInfo.Verb = "runas";
                startInfo.FileName = "powershell.exe";
                startInfo.RedirectStandardInput = true;
                startInfo.RedirectStandardOutput = true;
                // startInfo.StandardOutputEncoding = Encoding.GetEncoding(1251);

                process.StartInfo = startInfo;
                process.Start();

                StreamWriter myStreamWriter = process.StandardInput;
                var passCommand = $@"(""{SelectedConnection.Password}"" | ConvertTo-SecureString -AsPlainText -Force) | ConvertFrom-SecureString";
                myStreamWriter.WriteLine(passCommand);
                myStreamWriter.Close();

                var response = process.StandardOutput.ReadToEnd();

                var start = response.IndexOf("ConvertFrom-SecureString") + 24;
                var end = response.LastIndexOf("PS");
                var passHesh = response.Substring(start, end - start).Trim(new char[] { '\r', '\n' });

                process.WaitForExit();
                process.Close();

                SaveFileDialog sfd = new SaveFileDialog();
                sfd.Filter = "RDP File (*.rdp)|*.rdp";
                sfd.DefaultExt = ".rdp";
                sfd.FileName = $"{SelectedConnection.KeyName}.rdp";
                sfd.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
                if (sfd.ShowDialog() == true)
                {
                    StreamWriter sw = new StreamWriter(sfd.FileName, false);
                    sw.WriteLine($"full address:s:{SelectedConnection.Adress}:{SelectedConnection.Port}");
                    sw.WriteLine($"prompt for credentials:i:0");
                    sw.WriteLine($"administrative session:i:1");
                    sw.WriteLine($"username:s:{SelectedConnection.UserName}");
                    sw.WriteLine($"password 51:b:{passHesh}");
                    sw.Flush();
                    sw.Close();
                }
            });
        }
    }
}
