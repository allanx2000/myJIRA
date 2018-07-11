using Innouvous.Utils;
using Innouvous.Utils.MVVM;
using myJIRA.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;
using System.Windows.Input;

namespace myJIRA.ViewModels
{
    internal class ManageBoardsWindowViewModel : Innouvous.Utils.Merged45.MVVM45.ViewModel
    {
        private ManageBoardsWindow manageBoardsWindow;

        private ObservableCollection<BoardNameItemViewModel> boards = new ObservableCollection<BoardNameItemViewModel>();

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
                this.boards.Add(new BoardNameItemViewModel(b));
            }
        }

        public string DeleteRestoreText
        {
            get { return SelectedBoard != null && SelectedBoard.CurrentStatus != BoardNameItemViewModel.Status.Deleted ? "Delete" : "Restore"; }
        }

        public BoardNameItemViewModel SelectedBoard
        {
            get { return Get<BoardNameItemViewModel>(); }
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
        public ObservableCollection<BoardNameItemViewModel> BoardsList { get => boards; }

        public void RefreshView()
        {
            cvs.View.Refresh();
        }

        public ICommand DeleteRestoreCommand
        {
            get => new CommandHelper(DeleteRestoreBoard);
        }

        private void DeleteRestoreBoard()
        {
            try
            {
                if (SelectedBoard == null)
                    return;
                
                switch (SelectedBoard.CurrentStatus)
                {
                    case BoardNameItemViewModel.Status.Deleted: //Restore
                        SelectedBoard.CurrentStatus = BoardNameItemViewModel.Status.Existing;
                        break;
                    case BoardNameItemViewModel.Status.Existing:
                        SelectedBoard.CurrentStatus = BoardNameItemViewModel.Status.Deleted;
                        break;
                    case BoardNameItemViewModel.Status.New: //Only possible action is delete
                        boards.Remove(SelectedBoard);
                        break;
                }

                RaisePropertyChanged("DeleteRestoreText");
            }
            catch (Exception e)
            {
                MessageBoxFactory.ShowError(e);
            }
        }

        public ICommand CancelCommand
        {
            get => new CommandHelper(() => manageBoardsWindow.Close());
        }
        
        public ICommand AddCommand
        {
            get => new CommandHelper(AddBoard);
        }

        private void AddBoard()
        {
            try
            {
                var dlg = new EditBoardWindow();
                dlg.Owner = manageBoardsWindow;

                dlg.ShowDialog();

                if (!dlg.Cancelled)
                {
                    CheckDuplicate(dlg.GetName());

                    BoardNameItemViewModel vm = new BoardNameItemViewModel(dlg.GetName());
                    
                    boards.Add(vm);
                }
            }
            catch (Exception e)
            {
                MessageBoxFactory.ShowError(e);
            }

        }

        private void CheckDuplicate(string name)
        {
            foreach (var b in boards)
                if (b.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
                    throw new Exception("A board with the same name already exists.");
        }

        public ICommand EditCommand
        {
            get => new CommandHelper(EditBoard);
        }

        private void EditBoard()
        {
            try
            {
                var dlg = new EditBoardWindow(SelectedBoard.Name);
                dlg.Owner = manageBoardsWindow;

                dlg.ShowDialog();

                if (!dlg.Cancelled)
                {
                    var name = dlg.GetName();
                    if (SelectedBoard.Name != name)
                    {
                        CheckDuplicate(dlg.GetName());
                        SelectedBoard.Name = name;
                    }
                }
            }
            catch (Exception e)
            {
                MessageBoxFactory.ShowError(e);
            }
        }

        public ICommand SaveCommand
        {
            get => new CommandHelper(SaveBoards);
        }

        private void SaveBoards()
        {
            try
            {
                List<BoardName> updatedBoards = new List<Models.BoardName>();

                foreach (var b in boards)
                {
                    BoardName bn;
                    switch (b.CurrentStatus)
                    {
                        case BoardNameItemViewModel.Status.New:
                            bn = new BoardName(b.Name);
                            updatedBoards.Add(bn);
                            break;
                        case BoardNameItemViewModel.Status.Existing:
                            bn = b.GetExisting();
                            bn.Name = b.Name; //Copy from VM to Model
                            updatedBoards.Add(bn);
                            break;
                    }
                }

                AppStateManager.DataStore.UpsertBoards(updatedBoards);

                Cancelled = false;
                manageBoardsWindow.Close();
            }
            catch (Exception e)
            {
                MessageBoxFactory.ShowError(e);
            }
        }
    }
}