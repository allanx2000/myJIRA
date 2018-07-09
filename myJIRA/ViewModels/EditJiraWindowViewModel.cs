using System;
using myJIRA.Models;

namespace myJIRA.ViewModels
{
    internal class EditJiraWindowViewModel
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
                

                IsDone = existing.DoneDate == null;
                IsArchived = existing.ArchivedDate == null;
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
    }
}