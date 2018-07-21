using Innouvous.Utils.MVVM;
using System.Windows.Input;
using System;
using Innouvous.Utils;
using System.IO;

namespace myJIRA.ViewModels
{
    internal class SettingsWindowViewModel : Innouvous.Utils.Merged45.MVVM45.ViewModel
    {
        private SettingsWindow settingsWindow;


        public SettingsWindowViewModel(SettingsWindow settingsWindow)
        {
            this.settingsWindow = settingsWindow;
            Cancelled = true;

            LoadSettings();
        }

        private void LoadSettings()
        {
            var settings = AppStateManager.Settings;
            DBPath = settings.DBPath;
            ServerURL = settings.ServerUrl;
            CustomBrowserPath = settings.CustomBrowserPath;
            SaveWindowSize = settings.SaveWindowSize;
        }

        
        public bool Cancelled { get; private set; }

        public string DBPath
        {
            get { return Get<string>(); }
            set
            {
                Set(value);
                RaisePropertyChanged();
            }
        }

        public string ServerURL
        {
            get { return Get<string>(); }
            set
            {
                Set(value);
                RaisePropertyChanged();
            }
        }

        public bool SaveWindowSize { get; set; }

        public string CustomBrowserPath
        {
            get { return Get<string>(); }
            set
            {
                Set(value);
                RaisePropertyChanged();
            }
        }



        public ICommand SelectBrowserCommand
        {
            get => new CommandHelper(SelectBrowser);
        }

        private void SelectBrowser()
        {
            try
            {
                var dlg = DialogsUtility.CreateOpenFileDialog();
                DialogsUtility.AddExtension(dlg, "Application (*.exe)", "*.exe");

                dlg.ShowDialog();

                if (!string.IsNullOrEmpty(dlg.FileName))
                    CustomBrowserPath = dlg.FileName;
            }
            catch (Exception e)
            {
                MessageBoxFactory.ShowError(e);
            }
        }
        public ICommand SelectDBCommand
        {
            get => new CommandHelper(SelectDB);
        }

        private void SelectDB()
        {
            try
            {
                var dlg = DialogsUtility.CreateOpenFileDialog(checkFileExists: false);
                DialogsUtility.AddExtension(dlg, "myJIRA Database", "*.jdb");

                dlg.ShowDialog();

                if (!string.IsNullOrEmpty(dlg.FileName))
                    DBPath = dlg.FileName;
            }
            catch (Exception e)
            {
                MessageBoxFactory.ShowError(e);
            }
        }

        public ICommand CancelCommand
        {
            get => new CommandHelper(() => settingsWindow.Close());
        }

        public ICommand SaveCommand
        {
            get => new CommandHelper(SaveSettings);
        }

        private void SaveSettings()
        {
            try
            {

                if (string.IsNullOrEmpty(DBPath))
                {
                    throw new Exception("Database is required.");
                }

                try
                {
                    FileInfo fi = new FileInfo(DBPath);

                    if (fi.FullName != DBPath)
                        DBPath = fi.FullName;
                }
                catch (Exception e)
                {
                    throw new Exception("Database path is not valid.", e);
                }

                var settings = AppStateManager.Settings;
                settings.DBPath = DBPath;
                settings.ServerUrl = ServerURL;
                settings.CustomBrowserPath = CustomBrowserPath;
                settings.SaveWindowSize = SaveWindowSize;

                settings.Save();

                Cancelled = false;
                settingsWindow.Close();
            }
            catch (Exception e)
            {
                MessageBoxFactory.ShowError(e);
            }
        }
    }
}