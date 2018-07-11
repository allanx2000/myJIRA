using System;
using myJIRA.Models;
using Innouvous.Utils.MVVM;
using System.Windows.Input;

namespace myJIRA.ViewModels
{
    internal class EditBoardWindowViewModel : Innouvous.Utils.Merged45.MVVM45.ViewModel
    {
        private EditBoardWindow editBoardWindow;
        private string existingName;

        public EditBoardWindowViewModel(EditBoardWindow editBoardWindow, string existingName)
        {
            this.editBoardWindow = editBoardWindow;
            this.existingName = existingName;

            Cancelled = true;

            Name = existingName;
        }

        public string Title { get => (string.IsNullOrEmpty(existingName) ? "New" : "Edit") + " Board"; }
        public bool Cancelled { get; private set; }

        public string Name { get; set; }

        public ICommand SaveCommand { get => new CommandHelper(Save); }

        private void Save()
        {
            try
            {
                Cancelled = false;
                editBoardWindow.Close();
            }
            catch (Exception e)
            {

            }
        }

        public ICommand CancelCommand { get => new CommandHelper(() =>
        {
            editBoardWindow.Close();
        }); }


    }
}