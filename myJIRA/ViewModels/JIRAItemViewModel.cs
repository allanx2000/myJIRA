using myJIRA.Models;

namespace myJIRA.ViewModels
{   public class JIRAItemViewModel : Innouvous.Utils.Merged45.MVVM45.ViewModel
    {
        private JIRAItem item;

        public JIRAItemViewModel(JIRAItem item)
        {
            this.item = item;
        }

        /*
        public string Title
        {
            get { return item.Title; }
            set
            {
                item.Title = value;
                RaisePropertyChanged();
            }
        }

        public int? BoardId {
            get { return item.BoardId; }
        }*/

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