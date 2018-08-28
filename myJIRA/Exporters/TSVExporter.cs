using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using myJIRA.Models;
using System.IO;

namespace myJIRA.Exporters
{
    public class TSVExporter : IExporter
    {
        private static TSVExporter instance;

        public static TSVExporter Instance
        {
            get
            {
                if (instance == null)
                    instance = new TSVExporter();

                return instance;
            }
        }

        private const string Delim = "\t";

        private Dictionary<int, string> BoardMap;

        public void Export(List<JIRAItem> jiras, string outputFile)
        {
            BoardMap = new Dictionary<int, string>();
            foreach (var b in AppStateManager.DataStore.GetBoards())
            {
                BoardMap[b.ID.Value] = b.Name;
            }

            StreamWriter sw = new StreamWriter(outputFile);
            
            try
            {
                sw.WriteLine(string.Join(Delim, "Key", "Created", "Done Date", "Board", "Epic", "Title", "Status"));

                foreach (var j in jiras)
                {
                    sw.WriteLine(ConvertToString(j));
                }
            }
            catch (Exception e)
            {
                throw;
            }
            finally
            {
                sw.Close();
            }
        }

        private string ConvertToString(JIRAItem j)
        {

            return string.Join(Delim, j.JiraKey, ToString(j.CreatedDate), ToString(j.DoneDate),
                GetBoard(j),
                ToString(j.GetAuxField(AuxFields.Epic)), j.Title, 
                ToString(j.ArchivedDate != null ? null : j.Status) //Don't show status for archived
                );
        }
        

        private string GetBoard(JIRAItem j)
        {
            if (j.BoardId == null)
                return AppStateManager.NotStarted;
            else if (j.DoneDate != null)
            {
                if (j.ArchivedDate != null)
                    return "Archived";
                else
                    return AppStateManager.ReadyForRelease;
            }
            else return BoardMap[j.BoardId.Value];
        }

        /// <summary>
        /// Converts Values to String
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        private string ToString(object val)
        {
            if (val == null)
                return "";
            else if (val is DateTime?)
            {
                val = ((DateTime?)val).Value;
            }

            if (val is DateTime)
            {
                return ((DateTime)val).ToShortDateString();
            }
            else
                return val.ToString();
        }
    }
}
