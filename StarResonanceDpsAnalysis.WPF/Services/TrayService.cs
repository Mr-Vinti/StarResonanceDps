using System;
using System.Windows;
using System.Windows.Controls;
using Hardcodet.Wpf.TaskbarNotification;
using System.Windows.Media.Imaging;

namespace StarResonanceDpsAnalysis.WPF.Services;

public sealed class TrayService : ITrayService, IDisposable
{
    private TaskbarIcon? _tray;
    private bool _initialized;

    public void Initialize(string? toolTip = null)
    {
        if (_initialized) return;
        _initialized = true;

        _tray = new TaskbarIcon
        {
            ToolTipText = toolTip ?? Application.Current?.MainWindow?.Title ?? "App",
            Visibility = Visibility.Visible
        };

        try
        {
            var iconUri = new Uri("pack://application:,,,/Assets/Images/ApplicationIcon.ico");
            _tray.IconSource = new BitmapImage(iconUri);
        }
        catch { }

        var menu = new ContextMenu();
        var miShow = new MenuItem { Header = "Show" };
        miShow.Click += (_, _) => Restore();
        var miExit = new MenuItem { Header = "Exit" };
        miExit.Click += (_, _) => Exit();
        menu.Items.Add(miShow);
        menu.Items.Add(new Separator());
        menu.Items.Add(miExit);
        _tray.ContextMenu = menu;
        _tray.TrayMouseDoubleClick += (_, _) => Restore();
    }

    public void MinimizeToTray()
    {
        var main = Application.Current?.MainWindow;
        if (main == null) return;
        main.Hide();
    }

    public void Restore()
    {
        var main = Application.Current?.MainWindow;
        if (main == null) return;
        main.Show();
        if (main.WindowState == WindowState.Minimized) main.WindowState = WindowState.Normal;
        main.Activate();
    }

    public void Exit()
    {
        Dispose();
        Application.Current?.Shutdown();
    }

    public void Dispose()
    {
        try { _tray?.Dispose(); } catch { }
        _tray = null;
    }
}
