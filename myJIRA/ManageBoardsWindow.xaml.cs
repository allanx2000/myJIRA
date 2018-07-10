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
using myJIRA.ViewModels;

namespace myJIRA
{
    /// <summary>
    /// Interaction logic for ManageBoardsWindow.xaml
    /// </summary>
    public partial class ManageBoardsWindow : Window
    {
        private readonly ManageBoardsWindowViewModel vm;

        public ManageBoardsWindow()
        {
            InitializeComponent();

            vm = new ManageBoardsWindowViewModel(this);
            DataContext = vm;
        }

        public bool Cancelled { get => vm.Cancelled; }
    }
}
