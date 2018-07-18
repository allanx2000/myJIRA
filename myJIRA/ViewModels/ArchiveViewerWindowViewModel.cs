using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;

namespace myJIRA.ViewModels
{
    internal class ArchiveViewerWindowViewModel : Innouvous.Utils.Merged45.MVVM45.ViewModel
    {
        private readonly ArchiveViewerWindow archiveViewerWindow;
        private readonly CollectionViewSource cvs;
        private readonly ObservableCollection<JIRAItemViewModel> items = new ObservableCollection<JIRAItemViewModel>();

        public ArchiveViewerWindowViewModel(ArchiveViewerWindow archiveViewerWindow)
        {
            this.archiveViewerWindow = archiveViewerWindow;

            cvs = new CollectionViewSource();
            cvs.Source = items;

            //TODO: Add Sort Descriptions

            FromDate = DateTime.Today.AddDays(-30);
            ToDate = DateTime.Today;
        }

        public JIRAItemViewModel SelectedItem
        {
            get => Get<JIRAItemViewModel>();
            set
            {
                Set(value);
                RaisePropertyChanged();
            }
        }

        public ICollectionView Items
        {
            get => cvs.View;
        }

        public DateTime? FromDate
        {
            get => Get<DateTime>();
            set
            {
                if (value != FromDate)
                {
                    Set(value);
                    RaisePropertyChanged();

                    //TODO: Move to Button
                    LoadResults();
                }
            }
        }

        public DateTime? ToDate
        {
            get => Get<DateTime>();
            set
            {
                if (value != ToDate)
                {
                    Set(value);
                    RaisePropertyChanged();

                    LoadResults();
                }
            }
        }

        public string ResultStatusText
        {
            get => Get<string>();
            set
            {
                Set(value);
                RaisePropertyChanged();
            }
        }

        private void LoadResults()
        {
            if (FromDate == null || ToDate == null)
                return;

            items.Clear();

            var jiras = AppStateManager.DataStore.LoadArchivedJIRAs(FromDate.Value, ToDate.Value);

            foreach (var i in jiras)
            {
                JIRAItemViewModel jvm = new JIRAItemViewModel(i);

                items.Add(jvm);
            }

            ResultStatusText = string.Format("{0} - {1}: {2} items found.", 
                FromDate.Value.ToShortDateString(),
                ToDate.Value.ToShortDateString(),
                items.Count);

            Items.Refresh();
        }
    }
}