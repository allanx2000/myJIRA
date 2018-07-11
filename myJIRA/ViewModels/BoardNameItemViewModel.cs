using Innouvous.Utils.Merged45.MVVM45;
using myJIRA.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace myJIRA.ViewModels
{
    /// <summary>
    /// View Model for the ListBox in ManageBoardsWindow
    /// </summary>
    class BoardNameItemViewModel : ViewModel
    {
        public enum Status
        {
            New,
            Existing,
            Deleted
        }

        public bool Existing { get { return existing != null; } }

        public Status CurrentStatus
        {
            get => Get<Status>();
            set
            {
                Set(value);
                RaisePropertyChanged();
                RaisePropertyChanged("TextColor");
            }
        }
        
        private readonly static SolidColorBrush DeletedColor = new SolidColorBrush(Colors.Red);
        private readonly static SolidColorBrush NewColor = new SolidColorBrush(Colors.ForestGreen);
        private readonly static SolidColorBrush Black = new SolidColorBrush(Colors.Black);

        public SolidColorBrush TextColor
        {
            get
            {
                switch (CurrentStatus)
                {
                    case Status.Deleted:
                        return DeletedColor;
                    case Status.New:
                        return NewColor;
                    default:
                        return Black;
                }
            }
        }

        public string Name {
            get => Get<string>();
            set
            {
                Set(value);

                RaisePropertyChanged();
            }
        }

        private BoardName existing;

        public BoardNameItemViewModel(string name)
        {
            Name = name;
            CurrentStatus = Status.New;
        }

        public BoardNameItemViewModel(BoardName bn)
        {
            Name = bn.Name;
            this.existing = bn;
            CurrentStatus = Status.Existing;
        }

        internal BoardName GetExisting()
        {
            return existing;
        }
    }
}
