﻿using Innouvous.Utils.MVVM;
using System.Windows.Input;
using System;
using Innouvous.Utils;

namespace myJIRA.ViewModels
{
    internal class MainWindowViewModel : Innouvous.Utils.Merged45.MVVM45.ViewModel
    {
        private MainWindow mainWindow;

        public MainWindowViewModel(MainWindow mainWindow)
        {
            this.mainWindow = mainWindow;

            TryLoad();
        }

        private void TryLoad()
        {
            try
            {
                AppStateManager.Initialize(mainWindow);
            }
            catch (Exception e)
            {
                MessageBoxFactory.ShowError(e, "Loading Error");
            }
        }

        public ICommand OpenSettingsCommand
        {
            get => new CommandHelper(OpenSettings);
        }

        private void OpenSettings()
        {
            var dlg = new SettingsWindow();
            dlg.ShowDialog();

            if (!dlg.Cancelled)
            {
                AppStateManager.Initialize(mainWindow);
            }
        }
    }
}