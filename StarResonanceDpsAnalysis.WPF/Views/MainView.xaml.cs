using System.Linq;
using System.Windows;
using System.Windows.Controls.Primitives;
using StarResonanceDpsAnalysis.WPF.Themes.SystemThemes;
using StarResonanceDpsAnalysis.WPF.ViewModels;

namespace StarResonanceDpsAnalysis.WPF.Views;

/// <summary>
/// Interaction logic for MainView.xaml
/// </summary>
public partial class MainView : Window
{
    public MainView(MainViewModel viewModel, SystemThemeWatcher watcher)
    {
        watcher.Watch(this);
        InitializeComponent();
        DataContext = viewModel;

        Loaded += (_, _) => viewModel.InitializeTrayCommand.Execute(null);
        StateChanged += (_, _) =>
        {
            if (WindowState == WindowState.Minimized)
            {
                viewModel.MinimizeToTrayCommand.Execute(null);
            }
        };
        Closing += (s, e) =>
        {
            // default: hide instead of exit; user can Exit from tray menu
            e.Cancel = true;
            viewModel.MinimizeToTrayCommand.Execute(null);
        };
    }

    public bool IsDebugContentVisible { get; } =
#if DEBUG
        true;
#else
        false;
#endif

    private void Footer_OnConfirmClick(object sender, RoutedEventArgs e)
    {
        WindowState = WindowState.Minimized;
    }

}
