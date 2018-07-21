using myJIRA.DAO;
using myJIRA.Models;
using myJIRA.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace myJIRA
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly ViewModels.MainWindowViewModel vm;

        private readonly Properties.Settings Settings;

        public MainWindow()
        {
            InitializeComponent();

            Settings = AppStateManager.Settings;
            vm = new MainWindowViewModel(this);
            DataContext = vm;

            if (Settings.SaveWindowSize)
            {
                if (Settings.LastHeight != 0)
                    Height = Settings.LastHeight;

                if (Settings.LastWidth != 0)
                    Width = Settings.LastWidth;
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (Settings.SaveWindowSize)
            {
                Settings.LastHeight = Height;
                Settings.LastWidth = Width;
                Settings.Save();
            }
        }
    }
}
