using myJIRA.Models;

namespace myJIRA
{
    public class JIRAItemViewModel
    {
        internal readonly JIRAItem item;

        public JIRAItemViewModel(JIRAItem item)
        {
            this.item = item;
        }

        public string Title
        {
            get { return item.Title; }
            set
            {
                //TODO: Add VM
            }
        }

        public int? BoardId { get { return item.BoardId; } }
    }
}