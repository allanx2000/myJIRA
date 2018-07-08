using myJIRA.Models;

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
    }
}