using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Data;

namespace myJIRA.ViewModels
{
    internal class ManageBoardsWindowViewModel : Innouvous.Utils.Merged45.MVVM45.ViewModel
    {
        private ManageBoardsWindow manageBoardsWindow;

        private List<EditBoardNameItemViewModel> boards = new List<EditBoardNameItemViewModel>();

        private CollectionViewSource cvs;

        public ManageBoardsWindowViewModel(ManageBoardsWindow manageBoardsWindow)
        {
            this.manageBoardsWindow = manageBoardsWindow;
            Cancelled = true;

            cvs = new CollectionViewSource();
            cvs.Source = boards;

            LoadBoards();
        }

        private void LoadBoards()
        {
            var boards = AppStateManager.DataStore.GetBoards();

            foreach (var b in boards)
            {
                this.boards.Add(new EditBoardNameItemViewModel(b));
            }
        }

        public string DeleteRestoreText
        {
            get { return SelectedBoard != null && SelectedBoard.CurrentStatus != EditBoardNameItemViewModel.Status.Deleted ? "Delete" : "Restore"; }
        }

        public EditBoardNameItemViewModel SelectedBoard
        {
            get { return Get<EditBoardNameItemViewModel>(); }
            set
            {
                Set(value);
                RaisePropertyChanged();
                RaisePropertyChanged("DeleteRestoreText");
            }
        }

        public ICollectionView Boards
        {
            get { return cvs.View; }
        }

        public bool Cancelled { get; private set; }
    }
}