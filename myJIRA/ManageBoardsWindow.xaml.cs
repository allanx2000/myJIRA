using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;
using myJIRA.ViewModels;
using myJIRA.UserControls;

namespace myJIRA
{
    /// <summary>
    /// Interaction logic for ManageBoardsWindow.xaml
    /// </summary>
    public partial class ManageBoardsWindow : Window
    {
        private readonly ManageBoardsWindowViewModel vm;

        public ManageBoardsWindow()
        {
            InitializeComponent();

            vm = new ManageBoardsWindowViewModel(this);
            DataContext = vm;

            //Initialize ListBox
            Style itemContainerStyle = new Style(typeof(ListBoxItem));
            itemContainerStyle.Setters.Add(new Setter(ListBoxItem.AllowDropProperty, true));
            itemContainerStyle.Setters.Add(new EventSetter(ListBoxItem.MouseMoveEvent, new MouseEventHandler(ListBox_MouseMove)));
            itemContainerStyle.Setters.Add(new EventSetter(ListBoxItem.DropEvent, new DragEventHandler(ListBox_Drop)));
            BoardsList.ItemContainerStyle = itemContainerStyle;
        }

        public bool Cancelled { get => vm.Cancelled; }
        
        private void ListBox_MouseMove(object sender, MouseEventArgs e)
        {
            if (sender is ListBoxItem && e.LeftButton == MouseButtonState.Pressed)
            {
                ListBoxItem draggedItem = sender as ListBoxItem;
                draggedItem.IsSelected = true;
                //object data = BoardControl.GetDataFromListBox(draggedItem, e.GetPosition(draggedItem));

                DragDrop.DoDragDrop(draggedItem, draggedItem.DataContext, DragDropEffects.Move);                
            }
        }
        

        private void ListBox_Drop(object sender, DragEventArgs e)
        {
            BoardNameItemViewModel droppedData = e.Data.GetData(typeof(BoardNameItemViewModel)) as BoardNameItemViewModel;
            BoardNameItemViewModel target = ((ListBoxItem)(sender)).DataContext as BoardNameItemViewModel;

            int removedIdx = BoardsList.Items.IndexOf(droppedData);
            int targetIdx = BoardsList.Items.IndexOf(target);

            if (removedIdx < targetIdx)
            {
                vm.BoardsList.Insert(targetIdx + 1, droppedData);
                vm.BoardsList.RemoveAt(removedIdx);
            }
            else
            {
                int remIdx = removedIdx + 1;
                if (BoardsList.Items.Count + 1 > remIdx)
                {
                    vm.BoardsList.Insert(targetIdx, droppedData);
                    vm.BoardsList.RemoveAt(remIdx);
                }
            }
        }

        private void BoardsList_DragOver(object sender, DragEventArgs e)
        {
            e.Effects = DragDropEffects.Move;
        }
    }
}
