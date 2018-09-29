using Innouvous.Utils;
using Innouvous.Utils.MVVM;
using myJIRA.Exporters;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;
using System.Windows.Input;

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
            cvs.SortDescriptions.Add(new SortDescription("Data.DoneDate", ListSortDirection.Ascending));

            //TODO: Add Sort Descriptions

            FromDate = DateTime.Today.AddDays(-30);
            ToDate = DateTime.Today;

            LoadResults();
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

        public ICommand EditCommand
        {
            get => new CommandHelper(() =>
            {
                if (SelectedItem != null)
                {
                    SelectedItem.EditJira();
                    LoadResults();
                }
            });
        }

        public ICommand DeleteCommand
        {
            get => new CommandHelper(() =>
            {
                if (SelectedItem != null)
                {
                    SelectedItem.DeleteJira();
                    LoadResults();
                }
            });
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

        public ICommand CloseCommand { get => new CommandHelper(() => archiveViewerWindow.Close()); }


        public ICommand LoadCommand { get => new CommandHelper(LoadResults); }

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

        public ICommand ExportCommand { get => new CommandHelper(ExportJIRAs); }

        private void ExportJIRAs()
        {
            try
            {
                if (items.Count == 0)
                    return;

                var dlg = DialogsUtility.CreateSaveFileDialog("Export JIRAs");
                DialogsUtility.AddExtension(dlg, "TSV", "*.tsv");

                dlg.FileName = string.Format("Archived ({0} - {1}).tsv", 
                    FromDate.Value.ToString("yyyyMMdd"),
                    ToDate.Value.ToString("yyyyMMdd"));
                dlg.ShowDialog();

                if (!string.IsNullOrEmpty(dlg.FileName))
                {
                    var list = (from i in items select i.Data).ToList();
                    TSVExporter.Instance.Export(list, dlg.FileName);

                    MessageBoxFactory.ShowInfo("Done", "Exported Successfully");
                }
            }
            catch (Exception e)
            {
                MessageBoxFactory.ShowError(e);
            }
        }
    }
}