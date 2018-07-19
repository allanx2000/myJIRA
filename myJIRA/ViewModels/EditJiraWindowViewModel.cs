using System;
using myJIRA.Models;
using Innouvous.Utils;
using System.Windows.Input;
using Innouvous.Utils.MVVM;
using System.Collections;
using System.Collections.Generic;

namespace myJIRA.ViewModels
{
    internal class EditJiraWindowViewModel : Innouvous.Utils.Merged45.MVVM45.ViewModel
    {
        private EditJiraWindow editJiraWindow;
        private JIRAItem existing;

        public EditJiraWindowViewModel(EditJiraWindow editJiraWindow, JIRAItem existing)
        {
            this.editJiraWindow = editJiraWindow;
            this.existing = existing;

            Cancelled = true;

            LoadView(existing);
        }

        private void LoadView(JIRAItem existing)
        {
            if (existing != null)
            {
                JiraKey = existing.JiraKey;
                Notes = existing.Notes;
                SprintId = existing.SprintId;
                Status = existing.Status;
                JiraTitle = existing.Title;
                
                IsDone = existing.DoneDate != null;
                IsArchived = existing.ArchivedDate != null;

                Epic = existing.GetAuxField(AuxFields.Epic) as string;
                TimeEstimate = existing.GetAuxField(AuxFields.TimeEstimate) as string;
            }
        }
        
        public string Title { get => (existing == null ? "New" : "Edit") + " JIRA"; }
        public bool Cancelled { get; private set; }

        public string JiraKey { get; set; }
        public string Notes { get; set; }
        public string SprintId { get; set; }
        public string Status { get; set; }
        public string JiraTitle { get; set; }
        public bool IsDone { get; set; }
        public bool IsArchived { get; set; }

        public string Epic { get; set; }
        public string TimeEstimate { get; set; }

        public ICollection<string> Sprints { get => AppStateManager.Sprints; }
        public ICollection<string> Epics { get => AppStateManager.Epics; }

        public ICommand SaveCommand { get => new CommandHelper(Save); }

        private void Save()
        {
            try
            {
                if (string.IsNullOrEmpty(JiraKey))
                    throw new Exception("Key is required.");

                if (string.IsNullOrEmpty(JiraTitle))
                    throw new Exception("Title is required.");

                JIRAItem item = new JIRAItem();

                if (existing != null)
                {
                    item.ID = existing.ID;
                    item.BoardId = existing.BoardId;
                    item.ArchivedDate = existing.ArchivedDate;
                    item.DoneDate = existing.DoneDate;
                    item.CreatedDate = existing.CreatedDate;
                }
                else
                    item.CreatedDate = DateTime.Today;

                //Always Set
                item.JiraKey = JiraKey;
                item.Notes = Notes;
                item.SprintId = SprintId;
                item.Status = Status;
                item.Title = JiraTitle;
                item.SetAuxField(AuxFields.Epic, Epic);
                item.SetAuxField(AuxFields.TimeEstimate, TimeEstimate);

                if (IsDone)
                {
                    if (item.DoneDate == null)
                        item.DoneDate = DateTime.Today;
                }
                else
                    item.DoneDate = null;


                if (IsArchived)
                {
                    if (item.ArchivedDate == null)
                        item.ArchivedDate = DateTime.Today;
                }
                else
                    item.ArchivedDate = null;


                AppStateManager.DataStore.UpsertJIRA(item);

                Cancelled = false;
                editJiraWindow.Close();
            }
            catch (Exception e)
            {
                MessageBoxFactory.ShowError(e);
            }
        }

        public ICommand CancelCommand
        {
            get => new CommandHelper(() =>
            {
                editJiraWindow.Close();
            });
        }

    }
}