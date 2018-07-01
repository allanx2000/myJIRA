using myJIRA.DAO;
using myJIRA.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            Test();
        }

        private void Test()
        {
            KanbanBoard.Children.Clear();

            DataStore ds = new MockDataStore();
            var boardNames = ds.GetBoards();

            ObservableCollection<JIRAItemViewModel> items = CreateViewModelsFromJIRAs(ds.LoadOpenJIRAs());

            var first = BoardControl.CreateFirstBoard("Imported", items);
            var last = BoardControl.CreateLastBoard("Ready for Release", items);

            List<BoardControl> boards = new List<BoardControl>();
            boards.Add(first);
            
            foreach (var b in boardNames)
            {
                BoardControl boardControl = new BoardControl(
                    b, items);

                boards.Add(boardControl);
            }
            
            boards.Add(last);

            foreach (var b in boards)
                KanbanBoard.Children.Add(b);

        }

        private ObservableCollection<JIRAItemViewModel> CreateViewModelsFromJIRAs(List<JIRAItem> list)
        {
            ObservableCollection<JIRAItemViewModel> vms = new ObservableCollection<JIRAItemViewModel>();
            foreach (var i in list)
                vms.Add(new JIRAItemViewModel(i));

            return vms;
        }
    }
}
