using myJIRA.Models;
using myJIRA.ViewModels;
using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace myJIRA.UserControls
{
    /// <summary>
    /// Interaction logic for BoardControl.xaml
    /// </summary>
    public partial class BoardControl : UserControl
    {
        private readonly BoardControlViewModel vm;
        private readonly BoardName boardName;

        public const int FirstBoardOrder = -1;
        public const int LastBoardOrder = -2;

        public BoardControl(BoardName boardName, ObservableCollection<JIRAItemViewModel> jiras, Func<JIRAItemViewModel, bool> customFilter = null)
        {
            InitializeComponent();
            
            //TODO: first and last boards the custom filter  messes it up

            vm = new BoardControlViewModel(boardName, jiras, customFilter);
            this.boardName = boardName;

            DataContext = vm;
        }


        internal static BoardControl CreateFirstBoard(string name, ObservableCollection<JIRAItemViewModel> jiras)
        {
            return new BoardControl(new BoardName(name, FirstBoardOrder), jiras, 
                new Func<JIRAItemViewModel, bool>((i) => i.Data.BoardId == null && i.Data.DoneDate == null));
        }

        internal static BoardControl CreateLastBoard(string name, ObservableCollection<JIRAItemViewModel> jiras)
        {
            return new BoardControl(new BoardName(name, LastBoardOrder), jiras, 
                new Func<JIRAItemViewModel, bool>((i) => i.Data.DoneDate != null));
        }

        #region Drag Drop

        private static object dragSource = null;

        private void ListBox_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (sender != null && e.LeftButton == MouseButtonState.Pressed)
            {
                ListBox parent = (ListBox)sender;
                object data = GetDataFromListBox(parent, e.GetPosition(parent));

                if (data != null)
                {
                    DragDrop.DoDragDrop(parent, data, DragDropEffects.Move);
                }
            }
        }

        private void ListBox_Drop(object sender, DragEventArgs e)
        {
            if (sender != dragSource)
            {
                JIRAItemViewModel data = (JIRAItemViewModel)e.Data.GetData(typeof(JIRAItemViewModel));

                if (boardName.Order == FirstBoardOrder)
                {
                    data.Data.BoardId = null;
                    data.Data.DoneDate = null;
                }
                else if (boardName.Order == LastBoardOrder)
                {
                    data.Data.DoneDate = DateTime.Now;
                    data.Data.BoardId = null;
                }
                else
                {
                    data.Data.BoardId = boardName.ID;
                    data.Data.DoneDate = null;
                }

                //TODO: Update DAO
                AppStateManager.DataStore.UpsertJIRA(data.Data);


                //Update UI
                AppStateManager.RefreshBoards();
            }

            dragSource = null;
        }

        private static object GetDataFromListBox(ListBox source, Point point)
        {
            UIElement element = source.InputHitTest(point) as UIElement;
            if (element != null)
            {
                object data = DependencyProperty.UnsetValue;
                while (data == DependencyProperty.UnsetValue)
                {
                    data = source.ItemContainerGenerator.ItemFromContainer(element);

                    if (data == DependencyProperty.UnsetValue)
                    {
                        element = VisualTreeHelper.GetParent(element) as UIElement;
                    }

                    if (element == source)
                    {
                        return null;
                    }
                }

                if (data != DependencyProperty.UnsetValue)
                {
                    return data;
                }
            }

            return null;
        }

        internal void Refresh()
        {
            vm.ItemsList.Refresh();
        }

        #endregion

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
