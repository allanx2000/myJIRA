using myJIRA.DAO;
using myJIRA.Models;
using myJIRA.UserControls;
using myJIRA.ViewModels;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace myJIRA
{
    internal static class AppStateManager
    {
        private static Properties.Settings settings = Properties.Settings.Default;
        private static ObservableCollection<JIRAItemViewModel> openJiras;

        internal static void Initialize(MainWindow mainWindow)
        {
            var kb = mainWindow.KanbanBoard;

            kb.Children.Clear();

            DataStore ds = GetDataStore();

            var boardNames = ds.GetBoards();

            openJiras = CreateViewModelsFromJIRAs(ds.LoadOpenJIRAs());

            var first = BoardControl.CreateFirstBoard("Imported", openJiras);
            var last = BoardControl.CreateLastBoard("Ready for Release", openJiras);

            List<BoardControl> boards = new List<BoardControl>();
            boards.Add(first);

            foreach (var b in boardNames)
            {
                BoardControl boardControl = new BoardControl(
                    b, openJiras);

                boards.Add(boardControl);
            }

            boards.Add(last);

            foreach (var b in boards)
                kb.Children.Add(b);

        }

        private static DataStore GetDataStore()
        {
            //TODO: Create Settings and load db
            
            return new MockDataStore();
        }
        
        private static ObservableCollection<JIRAItemViewModel> CreateViewModelsFromJIRAs(List<JIRAItem> list)
        {
            ObservableCollection<JIRAItemViewModel> vms = new ObservableCollection<JIRAItemViewModel>();
            foreach (var i in list)
                vms.Add(new JIRAItemViewModel(i));

            return vms;
        }
    }
}