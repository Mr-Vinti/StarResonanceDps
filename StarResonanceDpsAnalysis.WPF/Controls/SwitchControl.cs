using System.Windows;
using System.Windows.Controls.Primitives;

namespace StarResonanceDpsAnalysis.WPF.Controls;

public class SwitchControl : ToggleButton
{
    public static readonly DependencyProperty OnContentProperty = DependencyProperty.Register(
        nameof(OnContent), typeof(object), typeof(SwitchControl), new PropertyMetadata("On"));

    public static readonly DependencyProperty OffContentProperty = DependencyProperty.Register(
        nameof(OffContent), typeof(object), typeof(SwitchControl), new PropertyMetadata("Off"));

    static SwitchControl()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(SwitchControl),
            new FrameworkPropertyMetadata(typeof(SwitchControl)));
    }

    public object OnContent
    {
        get => GetValue(OnContentProperty);
        set => SetValue(OnContentProperty, value);
    }

    public object OffContent
    {
        get => GetValue(OffContentProperty);
        set => SetValue(OffContentProperty, value);
    }
}