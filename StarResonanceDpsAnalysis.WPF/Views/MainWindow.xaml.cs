using System.Windows;
using StarResonanceDpsAnalysis.WPF.Themes.SystemThemes;
using StarResonanceDpsAnalysis.WPF.ViewModels;

namespace StarResonanceDpsAnalysis.WPF.Views
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

        private void UnderConstructionButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("This feature is under construction.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}