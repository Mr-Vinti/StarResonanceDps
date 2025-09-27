using System.Windows;

namespace StarResonanceDpsAnalysis.WPF.Views;

/// <summary>
/// ModuleSolveView.xaml 的交互逻辑
/// </summary>
public partial class ModuleSolveView : Window
{
    public ModuleSolveView()
    {
        InitializeComponent();
    }

    private void Footer_ConfirmClick(object sender, RoutedEventArgs e)
    {
        Close();
    }

    private void Footer_CancelClick(object sender, RoutedEventArgs e)
    {
        Close();
    }
}