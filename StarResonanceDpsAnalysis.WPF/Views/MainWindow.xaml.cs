using System.Windows;
using System.Windows.Controls.Primitives;
using StarResonanceDpsAnalysis.WPF.Themes.SystemThemes;
using StarResonanceDpsAnalysis.WPF.ViewModels;

namespace StarResonanceDpsAnalysis.WPF.Views;

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

    public bool IsDebugContentVisible { get; } =
#if DEBUG
        true;
#else
        false;
#endif

    private void UnderConstructionButton_Click(object sender, RoutedEventArgs e)
    {
        MessageBox.Show("This feature is under construction.", "Info", MessageBoxButton.OK,
            MessageBoxImage.Information);
    }

    private void TabSelector_Click(object sender, RoutedEventArgs e)
    {
        if (sender is not ToggleButton tb || !int.TryParse(tb.Tag?.ToString(), out var index)) return;
        MainTabControl.SelectedIndex = index;
        SyncSelectorWithTab();
    }

    private void SyncSelectorWithTab()
    {
        if (TabControlIndexChanger == null) return;

        foreach (var child in LogicalTreeHelper.GetChildren(TabControlIndexChanger))
        {
            if (child is not ToggleButton t || !int.TryParse(t.Tag?.ToString(), out var tagIndex)) continue;
            t.IsChecked = tagIndex == MainTabControl.SelectedIndex;
        }
    }

}
