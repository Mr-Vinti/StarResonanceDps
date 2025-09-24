using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using StarResonanceDpsAnalysis.WPF.Themes.SystemThemes;
using StarResonanceDpsAnalysis.WPF.Views;
using StarResonanceDpsAnalysis.WPF.ViewModels;

namespace StarResonanceDpsAnalysis.WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow(MainViewModel viewModel, SystemThemeWatcher watcher)
        {
            watcher.Watch(this);
            InitializeComponent();
            DataContext = viewModel;
        }

        private void DPS_Button_Click(object sender, RoutedEventArgs e)
        {
            // TODO: Open DPS Statistics Window
        }

        private void UnderConstructionButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("This feature is under construction.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}