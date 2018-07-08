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
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        private readonly ViewModels.SettingsWindowViewModel vm;

        public bool Cancelled { get => vm.Cancelled; }

        public SettingsWindow()
        {
            InitializeComponent();

            vm = new ViewModels.SettingsWindowViewModel(this);
            DataContext = vm;
        }
    }
}
