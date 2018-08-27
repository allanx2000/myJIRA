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

        public void Export(List<JIRAItem> jiras, string outputFile)
        {
            StreamWriter sw = new StreamWriter(outputFile);
            
            try
            {
            
                sw.WriteLine(string.Join(Delim, "Key", "Created", "Done Date", "Epic", "Title", "Status"));

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
                ToString(j.GetAuxField(AuxFields.Epic)), j.Title, 
                ToString(j.ArchivedDate != null ? null : j.Status) //Don't show status for archived
                );
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
