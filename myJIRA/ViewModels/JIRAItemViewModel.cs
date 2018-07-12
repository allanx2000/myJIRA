using Innouvous.Utils.MVVM;
using myJIRA.Models;
using System.Windows.Input;
using System;
using Innouvous.Utils;
using System.Windows;

namespace myJIRA.ViewModels
{   public class JIRAItemViewModel : Innouvous.Utils.Merged45.MVVM45.ViewModel
    {
        private JIRAItem item;

        public JIRAItemViewModel(JIRAItem item)
        {
            this.item = item;
        }

        public void DataUpdated()
        {
            RaisePropertyChanged("Data");
        }

        public JIRAItem Data
        {
            get => item;
            set
            {
                item = value;
                RaisePropertyChanged();
            }
        }

        public Visibility StatusVisibility
        {
            get => string.IsNullOrEmpty(item.Status) ? Visibility.Collapsed : Visibility.Visible;
        }

        public Visibility SprintVisibility
        {
            get => string.IsNullOrEmpty(item.Status) ? Visibility.Collapsed : Visibility.Visible;
        }

        public ICommand EditCommand
        {
            get => new CommandHelper(EditJira);
        }

        private void EditJira()
        {
            try
            {
                var dlg = new EditJiraWindow(item);
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
        
        public ICommand DeleteCommand
        {
            get => new CommandHelper(DeleteJira);
        }

        private void DeleteJira()
        {
            try
            {
                var confirm = MessageBoxFactory.ShowConfirmAsBool("Delete JIRA: " + item.Title + "?", "Confirm Delete");
                if (confirm)
                {
                    AppStateManager.DataStore.DeleteJIRA(item.ID.Value);
                    AppStateManager.ReloadOpenJIRAs();
                }
            }
            catch (Exception e)
            {
                MessageBoxFactory.ShowError(e);
            }
        }

    }
}