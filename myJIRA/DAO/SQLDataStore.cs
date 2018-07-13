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
        private const string Aux = "tbl_aux";


        public SQLDataStore(string path)
        {
            bool isNew = !File.Exists(path);

            var args = new Dictionary<string, string>() {
                { "FKSupport", "True"}
            };

            client = new SQLiteClient(path, isNew, args);

            CreateTables();
            
        }

        private bool TableExists(string name)
        {
            string cmd = string.Format("SELECT name FROM sqlite_master WHERE type = 'table' AND name = '{0}'", name);

            return !SQLUtils.IsNull(client.ExecuteScalar(cmd));
        }

        private void AddTestBoards()
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
            if (!TableExists(Boards)) //TODO: Fix in SQLUtils
            {
                CreateBoardTable();
            }

            if (!TableExists(Jiras))
            {
                CreateJiraTable();
            }

            if (!TableExists(Aux))
            {
                CreateAuxTable();
            }

        }

        private void CreateAuxTable()
        {
            string cmd = LoadFT("CreateAuxTable");
            client.ExecuteNonQuery(cmd);
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
            string cmd = string.Format("select * from {0} where archived_date is null", Jiras);
            var results = client.ExecuteSelect(cmd);

            var list = new List<JIRAItem>();

            foreach (DataRow r in results.Rows)
            {
                list.Add(ParseJiraItem(r));
            }

            return list;
        }

        private JIRAItem ParseJiraItem(DataRow r)
        {
            JIRAItem jira = new JIRAItem();

            jira.ID = Convert.ToInt32(r["id"]);

            if (!SQLUtils.IsNull(r["archived_date"]))
                jira.ArchivedDate = SQLUtils.ToDateTime(r["archived_date"].ToString());

            if (!SQLUtils.IsNull(r["board_id"]))
                jira.BoardId = Convert.ToInt32(r["board_id"]);

            jira.CreatedDate = SQLUtils.ToDateTime(r["created_date"].ToString());

            if (!SQLUtils.IsNull(r["done_date"]))
                jira.DoneDate = SQLUtils.ToDateTime(r["done_date"].ToString());

            
            jira.ImportedDate = jira.CreatedDate; //TODO: Track or remove? Need to add to DB

            jira.JiraKey = r["jira_key"].ToString();

            if (!SQLUtils.IsNull(r["notes"]))
                jira.Notes = r["notes"].ToString();

            if (!SQLUtils.IsNull(r["sprint_id"]))
                jira.SprintId = r["sprint_id"].ToString();

            jira.Status = r["status"].ToString();
            jira.Title = r["title"].ToString();

            //Aux
            Dictionary<AuxFields, object> aux = GetAuxFields(jira.ID.Value);
            jira.SetAuxField(aux);

            return jira;
        }

        private Dictionary<AuxFields, object> GetAuxFields(int jiraId)
        {
            string sql = "select * from {0} where jira_id = {1}";
            sql = string.Format(sql, Aux, jiraId);

            Dictionary<AuxFields, object> aux = new Dictionary<AuxFields, object>();

            var dt = client.ExecuteSelect(sql);

            foreach (DataRow row in dt.Rows)
            {
                AuxFields key = (AuxFields) Convert.ToInt32(row["aux_id"]);

                object value;

                switch (key)
                {
                    default:
                        value = row["val"].ToString();
                        break;
                }

                aux[key] = value;
            }

            return aux;
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
                        Boards,
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
            int jiraId;

            string cmd;
            if (jira.ID == null) //New
            {
                cmd = string.Format("insert into {0} values(NULL,{1},{2},{3},{4},'{5}','{6}','{7}',{8},{9})",
                    Jiras,
                    ToStringOrNull(jira.Notes),
                    ToStringOrNull(jira.ArchivedDate),
                    ToStringOrNull(jira.DoneDate), //3
                    (jira.BoardId == null ? NULL : jira.BoardId.ToString()),
                    SQLUtils.ToSQLDateTime(jira.CreatedDate),
                    SQLUtils.SQLEncode(jira.Title),
                    SQLUtils.SQLEncode(jira.JiraKey),
                    ToStringOrNull(jira.SprintId),
                    ToStringOrNull(jira.Status)
                    );

                jiraId = SQLUtils.GetLastInsertRow(client);
            }
            else //Update
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("update {0} set notes={1}, archived_date={2}, done_date={3}, board_id={4},");
                sb.AppendLine("created_date='{5}', title='{6}', jira_key='{7}', sprint_id={8}, status={9}");
                sb.AppendLine("where id = {10}");

                var str = sb.ToString();

                cmd = string.Format(str,
                    Jiras,
                    ToStringOrNull(jira.Notes),
                    ToStringOrNull(jira.ArchivedDate),
                    ToStringOrNull(jira.DoneDate),
                    jira.BoardId == null ? NULL : jira.BoardId.ToString(),
                    SQLUtils.ToSQLDateTime(jira.CreatedDate),
                    SQLUtils.SQLEncode(jira.Title),
                    SQLUtils.SQLEncode(jira.JiraKey),
                    ToStringOrNull(jira.SprintId),
                    ToStringOrNull(jira.Status),
                    jira.ID.Value
                    );

                jiraId = jira.ID.Value;
            }

            client.ExecuteNonQuery(cmd);

            UpsertAux(jiraId, jira.GetAuxFields());
        }

        private const string UpsertAuxQuery = "insert into {0} values({1},{2},'{3}')";
        private void UpsertAux(int jiraId, Dictionary<AuxFields, object> aux)
        {
            client.ExecuteNonQuery("delete from " + Aux + " where jira_id = " + jiraId);
            
            foreach (var kv in aux)
            {
                string cmd = string.Format(UpsertAuxQuery, Aux, jiraId, (int)kv.Key, kv.Value.ToString());
                client.ExecuteNonQuery(cmd);
            }
        }

        private const string NULL = "NULL";

        /// <summary>
        /// Returns NULL or quotes the value
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        private object ToStringOrNull(object val)
        {
            if (val == null)
                return NULL;

            string raw;
            if (val is DateTime?)
            {
                raw = SQLUtils.ToSQLDateTime(((DateTime?)val).Value);
            }
            else if (val is string)
                raw = SQLUtils.SQLEncode(val.ToString());
            else
                throw new NotImplementedException();

            return "'" + raw + "'";
        }
    }
}
