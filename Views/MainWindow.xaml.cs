using PFSoftware.LensesTracker.Models;
using System.Windows;

namespace PFSoftware.LensesTracker.Views
{
    /// <summary>Interaction logic for MainWindow.xaml</summary>
    public partial class MainWindow : Window
    {
        public MainWindow() => InitializeComponent();

        private void WindowMain_Loaded(object sender, RoutedEventArgs e) => AppState.MainWindow = this;
    }
}