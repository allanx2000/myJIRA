using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using myJIRA.Models;
using Innouvous.Utils.Data;
using System.Data;

namespace myJIRA.DAO
{
    class SQLDataStore : DataStore
    {
        private readonly SQLiteClient client;

        //Tables Names
        private const string Boards = "tbl_boards";
        private const string Jiras = "tbl_jiras";


        public SQLDataStore(string path)
        {
            bool isNew = !File.Exists(path);

            var args = new Dictionary<string, string>() {
                { "FKSupport", "True"}
            };

            client = new SQLiteClient(path, isNew, args);

            if (!SQLUtils.CheckTableExists(Boards, client))
            {
                CreateTables();
            }


            TestBoard();
        }

        private void TestBoard()
        {
            List<BoardName> boards = new List<BoardName>();
            boards.Add(new BoardName("Doing"));
            boards.Add(new BoardName("Testing"));
            boards.Add(new BoardName("Sign Off"));

            UpsertBoards(boards);
        }

        #region Create Tables
        private void CreateTables()
        {
            CreateBoardTable();
            CreateJiraTable();
        }

        private void CreateJiraTable()
        {
            string cmd = LoadFT("CreateJIRATable");
            client.ExecuteNonQuery(cmd);
        }

        private void CreateBoardTable()
        {
            string cmd = LoadFT("CreateBoardTable");
            client.ExecuteNonQuery(cmd);
        }


        private const string ScriptsPath = "TableScripts";
        private const string ScriptsFormat = "txt";

        private string LoadFT(string name, params object[] args)
        {
            return SQLUtils.LoadCommandFromText(ScriptsPath, name, ScriptsFormat, args);

        }


        #endregion

        public void DeleteJIRA(int id)
        {
            string cmd = string.Format("delete from {0} where id = {1}", Jiras, id);
            client.ExecuteNonQuery(cmd);
        }

        public List<BoardName> GetBoards()
        {
            string cmd = string.Format("select * from {0} order by \"order\" asc", Boards);

            List<BoardName> boards = new List<BoardName>();

            var results = client.ExecuteSelect(cmd);
            foreach (DataRow r in results.Rows)
            {
                boards.Add(ParseBoard(r));
            }

            return boards;
        }

        private BoardName ParseBoard(DataRow r)
        {
            BoardName board = new BoardName(
                Convert.ToInt32(r["id"]),
                r["name"].ToString(),
                Convert.ToInt32(r["order"])
                );

            return board;
        }

        public List<JIRAItem> LoadArchivedJIRAs(int archivedYear)
        {
            throw new NotImplementedException();
        }

        public List<JIRAItem> LoadOpenJIRAs()
        {
            return new List<JIRAItem>();
        }


        public void UpsertBoards(List<BoardName> boards)
        {
            int ctr = 0;
            string cmd;

            //Delete removed
            var ids = from b in boards where b.ID != null select b.ID.Value;
            cmd = string.Format("delete from {0} where id not in ({1})",
                Boards,
                string.Join(",", ids)
                );

            client.ExecuteNonQuery(cmd);
            
            //Upsert
            foreach (BoardName bn in boards)
            {
                bn.Order = ctr++;
                if (bn.ID == null)
                {
                    cmd = string.Format("insert into {0} values(NULL,'{1}',{2})",
                        Boards,
                        SQLUtils.SQLEncode(bn.Name),
                        bn.Order
                        );
                }
                else
                {
                    cmd = string.Format("update {0} set name='{1}', \"order\"={2} where id = {3}",
                        boards,
                        SQLUtils.SQLEncode(bn.Name),
                        bn.Order,
                        bn.ID.Value
                        );
                }

                client.ExecuteNonQuery(cmd);
            }
        }

        public void UpsertJIRA(JIRAItem jira)
        {
            throw new NotImplementedException();
        }
    }
}
