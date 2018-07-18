using myJIRA.ViewModels;
using System;
using System.Collections.Generic;
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
    }
}
