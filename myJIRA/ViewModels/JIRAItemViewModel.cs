using Innouvous.Utils.MVVM;
using myJIRA.Models;
using System.Windows.Input;
using System;
using Innouvous.Utils;
using System.Windows;
using System.Diagnostics;

namespace myJIRA.ViewModels
{   public class JIRAItemViewModel : Innouvous.Utils.Merged45.MVVM45.ViewModel
    {
        private JIRAItem item;

        public JIRAItemViewModel(JIRAItem item)
        {
            this.item = item;
        }

        
        public Visibility HasNotes {
            get {
                return string.IsNullOrEmpty(item.Notes) ? Visibility.Collapsed : Visibility.Visible;
            }
        }

        public Visibility HasPullRequest
        {
            get
            {
                return string.IsNullOrEmpty(item.GetAuxField(AuxFields.PullRequest) as string) ? Visibility.Collapsed : Visibility.Visible;
            }
        }

        public void DataUpdated()
        {
            RaisePropertyChanged("Data");
        }

        #region AuxFields Getters

        public string PullRequest
        {
            get => Data.GetAuxField(AuxFields.PullRequest) as string;
        }

        public string Epic
        {
            get => Data.GetAuxField(AuxFields.Epic) as string;
        }

        public string TimeEstimate
        {
            get => Data.GetAuxField(AuxFields.TimeEstimate) as string;
        }

        #endregion

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
            get => string.IsNullOrEmpty(item.SprintId) ? Visibility.Collapsed : Visibility.Visible;
        }

        public ICommand EditCommand
        {
            get => new CommandHelper(EditJira);
        }

        public ICommand OpenPullRequestCommand
        {
            get => new CommandHelper(OpenPullRequest);
        }

        private void OpenPullRequest()
        {
            try
            {
                if (!string.IsNullOrEmpty(PullRequest))
                    Process.Start(PullRequest);
            }
            catch (Exception e)
            {
                MessageBoxFactory.ShowError(e, title: "Link not Vaild");
            }
        }

        public void EditJira()
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

        public void DeleteJira()
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