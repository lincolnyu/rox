using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Windows;
using Microsoft.Win32;
using Rox.Core;
using Rox.FileSys;

namespace roxwpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Repo _repo;
        private Watching _watching;

        public MainWindow()
        {
            InitializeComponent();

            LoadAllSettings();

            UpdateEditingModeChanged();
        }

        public void BrowseBtnClick(object sender, RoutedEventArgs e)
        {
            var sfd = new SaveFileDialog();
            sfd.Filter = "Rox File (*.rox)|*.rox";
            if (sfd.ShowDialog() == true)
            {
                UpdateRoxFile(sfd.FileName, true);
            }
        }

        public void ResetViewBtnClick(object sender, RoutedEventArgs e)
        {
        }

        public void AddNewTagBtnClick(object sender, RoutedEventArgs e)
        {
        }

        public void EditingCheckedChanged(object sender, RoutedEventArgs e)
        {
            UpdateEditingModeChanged();
        }

        private void UpdateEditingModeChanged()
        {
            EditingGroup.IsEnabled = EditingModeChk.IsChecked==true;
        }

        private void UpdateRoxFile(string dbfn, bool updateToSettings)
        {
            RoxFile.Text = dbfn;
            var dbExists = System.IO.File.Exists(dbfn);
            _repo = new Repo();
            if (dbExists)
            {
                using (var fs = new FileStream(dbfn, FileMode.Open))
                {
                    using (var br = new BinaryReader(fs))
                    {
                        _repo.Deserialize(br);
                    }
                }
            }
            _watching = new Watching(_repo);
            if (updateToSettings)
            {
                AddUpdateAppSettings("RoxFilePath", dbfn);
            }
        }

        private void LoadAllSettings()
        {
            try
            {
                var appSettings = ConfigurationManager.AppSettings;
                var iRoxFilePath = Array.IndexOf(appSettings.AllKeys, "RoxFilePath");
                if (iRoxFilePath >= 0)
                {
                    var roxFilePath = appSettings.Get(iRoxFilePath);
                    UpdateRoxFile(roxFilePath, false);
                }
            }
            catch (ConfigurationErrorsException)
            {
                MessageBox.Show("Error reading app settings");
            }
        }


        private static void AddUpdateAppSettings(string key, string value)
        {
            try
            {
                var configFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                var settings = configFile.AppSettings.Settings;
                if (settings[key] == null)
                {
                    settings.Add(key, value);
                }
                else
                {
                    settings[key].Value = value;
                }
                configFile.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection(configFile.AppSettings.SectionInformation.Name);
            }
            catch (ConfigurationErrorsException)
            {
                Console.WriteLine("Error writing app settings");
            }
        }
    }
}
