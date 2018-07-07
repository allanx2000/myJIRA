using System;
using System.Collections.Generic;
using myJIRA.Models;

namespace myJIRA.DAO
{
    internal class MockDataStore : DataStore
    {
        public void DeleteJIRA(int id)
        {
            throw new System.NotImplementedException();
        }

        private List<BoardName> boards = new List<BoardName>();
        private List<JIRAItem> openJIRAs;

        public MockDataStore()
        {
            CreateBoards();
            CreateOpenJIRAs();
        }

        private void CreateOpenJIRAs()
        {
            openJIRAs = new List<JIRAItem>();
            openJIRAs.Add(new JIRAItem() { BoardId = 0, Title = "Test", JiraKey="C1" });
            openJIRAs.Add(new JIRAItem() { BoardId = 0, Title = "Test7", JiraKey = "C7" });
            openJIRAs.Add(new JIRAItem() { BoardId = 1, Title = "Test2", JiraKey = "C2" });
            openJIRAs.Add(new JIRAItem() { BoardId = 2, Title = "Test3", JiraKey = "C3" });
            openJIRAs.Add(new JIRAItem() { Title = "Test4", JiraKey = "C4" });
            openJIRAs.Add(new JIRAItem() { Title = "Test5", JiraKey = "C5", DoneDate = DateTime.Today });
        }

        private void CreateBoards()
        {
            boards.Add(new BoardName(0, "Doing", 0));
            boards.Add(new BoardName(1, "Testing", 1));
            boards.Add(new BoardName(2, "Sign Off", 2));
        }

        public List<BoardName> GetBoards()
        {
            return boards;
        }

        public List<JIRAItem> LoadArchivedJIRAs(int archivedYear)
        {
            throw new System.NotImplementedException();
        }

        public List<JIRAItem> LoadOpenJIRAs()
        {
            return openJIRAs;
        }

        public void UpsertBoards(List<BoardName> boards)
        {
            throw new System.NotImplementedException();
        }

        public void UpsertJIRA(JIRAItem jira)
        {
            throw new System.NotImplementedException();
        }
    }
}