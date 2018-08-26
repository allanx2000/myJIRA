using myJIRA.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace myJIRA
{
    /// <summary>
    /// Interaction logic for ArchiveViewerWindow.xaml
    /// </summary>
    public partial class ArchiveViewerWindow : Window
    {
        private readonly ArchiveViewerWindowViewModel vm;

        public ArchiveViewerWindow()
        {
            InitializeComponent();

            this.vm = new ArchiveViewerWindowViewModel(this);
            DataContext = vm;
        }

        private void ListBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            //TODO: Remove Dup
            var lb = sender as ListBox;

            if (lb != null && lb.SelectedItem != null)
            {
                var item = lb.SelectedItem as JIRAItemViewModel;

                if (item != null)
                {
                    string url = AppStateManager.Settings.ServerUrl + item.Data.JiraKey;

                    string cbp = AppStateManager.Settings.CustomBrowserPath;

                    if (string.IsNullOrEmpty(cbp))
                    {
                        Process.Start(url);
                    }
                    else
                        Process.Start(cbp, url);
                }
            }
        }
    }
}
