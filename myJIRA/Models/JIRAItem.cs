using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace myJIRA.Models
{
    public class JIRAItem : Innouvous.Utils.Merged45.MVVM45.ViewModel
    {
        public int? ID { get; set; }
     
        public string Notes { get; set; }
        
        /// <summary>
        /// Date the item was imported or updated
        /// </summary>
        public DateTime ImportedDate { get; set; }

        public DateTime? ArchivedDate { get; set; }

        public DateTime? DoneDate { get; internal set; }
        
        public int? BoardId
        {
            get
            {
                return Get<int?>();
            }
            set
            {
                Set(value);
                RaisePropertyChanged();
            }
        }

        #region From JIRA
        public string Title { get; set; }

        /// <summary>
        /// Key for lookup open issue
        /// </summary>
        public string JiraKey { get; set; }
        public string Status { get; set; }

        /// <summary>
        /// Should be latest Sprint ID
        /// </summary>
        public string SprintId { get; set; }

        /// <summary>
        /// Date the JIRA was created
        /// </summary>
        public DateTime CreatedDate { get; set; }

        internal JIRAItem Clone()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
