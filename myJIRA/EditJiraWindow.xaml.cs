using myJIRA.Models;
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
    /// Interaction logic for EditJiraWindow.xaml
    /// </summary>
    public partial class EditJiraWindow : Window
    {
        private readonly EditJiraWindowViewModel vm;

        public EditJiraWindow(JIRAItem existing = null)
        {
            InitializeComponent();

            vm = new EditJiraWindowViewModel(this, existing);
            DataContext = vm;
        }

        public bool Cancelled { get => vm.Cancelled; }
    }
}
