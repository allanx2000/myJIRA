using Innouvous.Utils.MVVM;
using myJIRA.Models;
using System.Windows.Input;
using System;
using Innouvous.Utils;

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

    }
}