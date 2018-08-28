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
        public const string NotStarted = "Not Started";
        public const string ReadyForRelease = "Ready for Release";

        private static Properties.Settings settings = Properties.Settings.Default;
        public static Properties.Settings Settings { get => settings; }


        private static ObservableCollection<JIRAItemViewModel> openJiras = new ObservableCollection<JIRAItemViewModel>();
        private static List<BoardControl> boardControls;
        private static DataStore ds;

        public static DataStore DataStore { get { return ds; } }

        internal static void Initialize(MainWindow mainWindow)
        {
            var kb = mainWindow.KanbanBoard;

            kb.Orientation = BoardOrientation;
            kb.Children.Clear();

            ds = GetDataStore();

            var boardNames = ds.GetBoards();

            ReloadOpenJIRAs();
            
            var first = BoardControl.CreateFirstBoard(NotStarted, openJiras);
            ConfigureBoardControl(mainWindow, first, BoardOrientation);
            var last = BoardControl.CreateLastBoard(ReadyForRelease, openJiras);
            ConfigureBoardControl(mainWindow, last, BoardOrientation);

            boardControls = new List<BoardControl>();
            boardControls.Add(first);

            foreach (var b in boardNames)
            {
                BoardControl boardControl = new BoardControl(
                    b, openJiras);

                ConfigureBoardControl(mainWindow, boardControl, BoardOrientation);

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

        private static void ConfigureBoardControl(MainWindow window, BoardControl bc, Orientation orientation)
        {
            if (orientation == Orientation.Vertical)
                bc.Height = Settings.BoardHeight;
            else
                bc.Width = Settings.BoardWidth;
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

        private static SortedSet<string> sprints = new SortedSet<string>();
        private static SortedSet<string> epics = new SortedSet<string>();

        public static ICollection<string> Sprints { get => sprints; }
        public static ICollection<string> Epics { get => epics; }
        public static Orientation BoardOrientation {
            get {
                return (Orientation)Settings.BoardOrientation;
            }
            set
            {
                Settings.BoardOrientation = (int)value;
                Settings.Save();
            }
        }

        public static List<JIRAItem> OpenJIRAs
        {
            get
            {
                return (from j in openJiras select j.Data).ToList();
            }
        }

        public static void ReloadOpenJIRAs()
        {
            openJiras.Clear();
            var jiras = CreateViewModelsFromJIRAs(ds.LoadOpenJIRAs());

            sprints.Clear();
            epics.Clear();

            foreach (var j in jiras)
            {
                openJiras.Add(j);
                sprints.Add(j.Data.SprintId);
                epics.Add(j.Epic);
            }
        }
    }
}