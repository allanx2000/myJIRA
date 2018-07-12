using myJIRA.Models;
using myJIRA.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace myJIRA.UserControls
{

    public class BoardControlViewModel
    {
        private readonly BoardName boardName;
        private readonly ObservableCollection<JIRAItemViewModel> jiras;
        private readonly CollectionViewSource cvs;
        private Func<JIRAItemViewModel, bool> filter;

        public JIRAItemViewModel SelectedJira { get; set; }

        private bool MatchBoardID(JIRAItemViewModel item)
        {
            return boardName.ID == item.Data.BoardId;
        }

        public ICollectionView ItemsList
        {
            get
            {
                return cvs.View;
            }
        }

        public BoardControlViewModel(BoardName boardName, ObservableCollection<JIRAItemViewModel> jiras, Func<JIRAItemViewModel, bool> customFilter = null)
        {
            this.boardName = boardName;
            this.jiras = jiras;

            cvs = new CollectionViewSource();
            cvs.Source = jiras;

            filter = customFilter == null ? MatchBoardID : customFilter;

            cvs.Filter += Cvs_Filter;
        }

        public string BoardName
        {
            get { return boardName.Name; }
        }

        private void Cvs_Filter(object sender, FilterEventArgs e)
        {
            var jvm = e.Item as JIRAItemViewModel;
            if (jvm != null)
            {
                e.Accepted = filter(jvm);
            }
            else
                e.Accepted = false;
        }

        public void RefreshView()
        {
            cvs.View.Refresh();
        }
    }
}
