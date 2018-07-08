using Innouvous.Utils.MVVM;
using System.Windows.Input;
using System;
using Innouvous.Utils;

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
            DBPath = AppStateManager.Settings.
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

        public string CustomBrowserPath
        {
            get { return Get<string>(); }
            set
            {
                Set(value);
                RaisePropertyChanged();
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

                if (dlg.FileName != null)
                    DBPath = dlg.FileName;
            }
            catch (Exception e)
            {
                MessageBoxFactory.ShowError(e);
            }

        }
    }
}