using myJIRA.Models;
using myJIRA.ViewModels;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace myJIRA.UserControls
{
    public class BoardTemplateSelector : DataTemplateSelector
    {
        public DataTemplate Collapsed { get; set; }
        public DataTemplate Expanded { get; set; }

        //TODO: Cannot use Selector, mulst use dynamic trigger?
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {

            /*
            YourClass obj = (YourClass)item;

            if (obj.Type == "SomeType")
            {
                return Template1;
            }
            else
            {
                return Template2;
            }
            */

            bool selected = true;

            if (selected)
                return Expanded;
            else
                return Collapsed;
        }
    }

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
            return new BoardControl(new BoardName(name, -1), jiras, new Func<JIRAItemViewModel, bool>((i) => i.Data.BoardId == null && i.Data.DoneDate == null));
        }

        internal static BoardControl CreateLastBoard(string name, ObservableCollection<JIRAItemViewModel> jiras)
        {
            return new BoardControl(new BoardName(name, -2), jiras, new Func<JIRAItemViewModel, bool>((i) => i.Data.DoneDate != null));
        }
    }

    public class BoardControlViewModel
    {
        private readonly BoardName boardName;
        private readonly ObservableCollection<JIRAItemViewModel> jiras;
        private readonly CollectionViewSource cvs;
        private Func<JIRAItemViewModel, bool> filter;

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
