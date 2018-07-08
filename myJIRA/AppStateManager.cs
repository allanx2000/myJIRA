using myJIRA.DAO;
using myJIRA.Models;
using myJIRA.UserControls;
using myJIRA.ViewModels;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System;

namespace myJIRA
{
    internal static class AppStateManager
    {
        private static Properties.Settings settings = Properties.Settings.Default;
        public static Properties.Settings Settings { get => settings; }


        private static ObservableCollection<JIRAItemViewModel> openJiras;
        private static List<BoardControl> boardControls;
        private static DataStore ds;

        public static DataStore DataStore { get { return ds; } }

        internal static void Initialize(MainWindow mainWindow)
        {
            var kb = mainWindow.KanbanBoard;
            kb.Orientation = System.Windows.Controls.Orientation.Vertical;
            kb.Children.Clear();

            ds = GetDataStore();

            var boardNames = ds.GetBoards();

            openJiras = CreateViewModelsFromJIRAs(ds.LoadOpenJIRAs());

            var first = BoardControl.CreateFirstBoard("Imported", openJiras);
            ConfigureBoard(first);
            var last = BoardControl.CreateLastBoard("Ready for Release", openJiras);
            ConfigureBoard(last);

            boardControls = new List<BoardControl>();
            boardControls.Add(first);

            foreach (var b in boardNames)
            {
                BoardControl boardControl = new BoardControl(
                    b, openJiras);

                ConfigureBoard(boardControl);

                boardControls.Add(boardControl);
            }

            boardControls.Add(last);

            foreach (var b in boardControls)
                kb.Children.Add(b);
            
            for (int i = 0; i < boardControls.Count; i++)
            {

            }
        }

        private static void ConfigureBoard(BoardControl bc)
        {
            bc.MinHeight = 200;
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

        internal static void RefreshBoards()
        {
            foreach (var b in boardControls)
            {
                b.Refresh();
            }
        }
    }
}