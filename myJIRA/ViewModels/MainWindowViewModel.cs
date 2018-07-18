using Innouvous.Utils.MVVM;
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
            dlg.Owner = mainWindow;
            dlg.ShowDialog();

            if (!dlg.Cancelled)
            {
                AppStateManager.Initialize(mainWindow);
            }
        }

        public ICommand CreateJiraCommand
        {
            get => new CommandHelper(CreateJIRA);
        }

        private void CreateJIRA()
        {
            try
            {
                var dlg = new EditJiraWindow();
                dlg.Owner = mainWindow;
                dlg.ShowDialog();

                if (!dlg.Cancelled)
                {
                    AppStateManager.ReloadOpenJIRAs();
                }
            }
            catch (Exception e)
            {
                MessageBoxFactory.ShowError(e);
            }
        }

        public ICommand ManageBoardsCommand
        {
            get => new CommandHelper(ManageBoards);
        }

        private void ManageBoards()
        {
            try
            {
                var dlg = new ManageBoardsWindow();
                dlg.Owner = mainWindow;
                dlg.ShowDialog();

                if (!dlg.Cancelled)
                {
                    AppStateManager.Initialize(mainWindow);
                }
            }
            catch (Exception e)
            {
                MessageBoxFactory.ShowError(e);
            }
        }

        public ICommand OpenArchiveViewerCommand
        {
            get => new CommandHelper(OpenArchiveViewer);
        }

        private void OpenArchiveViewer()
        {
            var dlg = new ArchiveViewerWindow();
            dlg.Owner = mainWindow;
            dlg.ShowDialog();

            AppStateManager.ReloadOpenJIRAs();
        }
        
    }
}