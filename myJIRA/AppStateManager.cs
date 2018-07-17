using myJIRA.DAO;
using myJIRA.Models;
using myJIRA.UserControls;
using myJIRA.ViewModels;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System;
using System.Windows.Controls;
using Innouvous.Utils.MVVM;
using System.Linq;

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

            var first = BoardControl.CreateFirstBoard("Not Started", openJiras);
            ConfigureBoardControl(mainWindow, first);
            var last = BoardControl.CreateLastBoard("Ready for Release", openJiras);
            ConfigureBoardControl(mainWindow, last);

            boardControls = new List<BoardControl>();
            boardControls.Add(first);

            foreach (var b in boardNames)
            {
                BoardControl boardControl = new BoardControl(
                    b, openJiras);

                ConfigureBoardControl(mainWindow, boardControl);

                boardControls.Add(boardControl);
            }

            boardControls.Add(last);

            foreach (var b in boardControls)
                kb.Children.Add(b);

            //TODO: Archive Button
            Button archiveButton = new Button()
            {
                Content = "Archive",
                HorizontalAlignment = System.Windows.HorizontalAlignment.Center,
                Command = new CommandHelper(() => DoArchive(openJiras))
            };

            kb.Children.Add(archiveButton);
        }

        private static void DoArchive(ObservableCollection<JIRAItemViewModel> openJiras)
        {
            /*
             * -Find the JIRAs in Ready to release
             * -Mark them as Archived
             * -Refresh boards
             */

            var done = from x in openJiras where x.Data.DoneDate != null select x.Data;

            if (done.Count() == 0)
                return;

            foreach (var x in done)
            {
                x.ArchivedDate = DateTime.Today;
                ds.UpsertJIRA(x);
            }

            ReloadOpenJIRAs();
        }

        private static void ConfigureBoardControl(MainWindow window, BoardControl bc)
        {
            bc.MinHeight = 200;
            bc.MaxHeight = 800;
            //bc.SetBinding(BoardControl.WidthProperty, boardWidth);
        }

        private static DataStore GetDataStore()
        {
            while (string.IsNullOrEmpty(Settings.DBPath))
            {
                var dlg = new SettingsWindow();
                dlg.ShowDialog();
            }

            return new SQLDataStore(settings.DBPath);

            //return new MockDataStore();
        }
        
        private static ObservableCollection<JIRAItemViewModel> CreateViewModelsFromJIRAs(List<JIRAItem> list)
        {
            ObservableCollection<JIRAItemViewModel> vms = new ObservableCollection<JIRAItemViewModel>();
            foreach (var i in list)
                vms.Add(new JIRAItemViewModel(i));

            return vms;
        }

        /// <summary>
        /// Refreshes the Listboxes in the BoardControls
        /// </summary>
        internal static void RefreshBoards()
        {
            foreach (var b in boardControls)
            {
                b.Refresh();
            }
        }

        public static void ReloadOpenJIRAs()
        {
            openJiras.Clear();
            var jiras = CreateViewModelsFromJIRAs(ds.LoadOpenJIRAs());

            foreach (var j in jiras)
                openJiras.Add(j);
        }
    }
}