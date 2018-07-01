using myJIRA.DAO;
using myJIRA.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace myJIRA
{
    /// <summary>
    /// Interaction logic for BoardControl.xaml
    /// </summary>
    public partial class BoardControl : UserControl
    {
        private readonly BoardControlViewModel vm;

        public BoardControl(BoardName boardName, ObservableCollection<JIRAItemViewModel> jiras, Func<JIRAItemViewModel, bool> customFilter = null)
        {
            InitializeComponent();

            vm = new BoardControlViewModel(boardName, jiras, customFilter);
            DataContext = vm;
        }


        internal static BoardControl CreateFirstBoard(string name, ObservableCollection<JIRAItemViewModel> jiras)
        {
            return new BoardControl(new BoardName(name, -1), jiras, new Func<JIRAItemViewModel, bool>((i) => i.item.BoardId == null && i.item.DoneDate == null));
        }

        internal static BoardControl CreateLastBoard(string name, ObservableCollection<JIRAItemViewModel> jiras)
        {
            return new BoardControl(new BoardName(name, -2), jiras, new Func<JIRAItemViewModel, bool>((i) => i.item.DoneDate != null));
        }
    }

    public class BoardControlViewModel
    {
        private readonly BoardName boardName;
        private readonly ObservableCollection<JIRAItemViewModel> jiras;
        private readonly CollectionViewSource cvs;
        private Func<JIRAItemViewModel, bool> filter;

        private bool MatchBoardName(JIRAItemViewModel item)
        {
            return boardName.ID == item.BoardId;
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

            filter = customFilter == null ? MatchBoardName : customFilter;

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
