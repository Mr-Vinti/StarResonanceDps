using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
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

namespace StarResonanceDpsAnalysis.WPF.Views
{
    /// <summary>
    /// AboutView.xaml 的交互逻辑
    /// </summary>
    public partial class AboutView : Window
    {
        public static string Version
        { 
            get
            {
                var v = Assembly
                    .GetEntryAssembly()
                    ?.GetCustomAttribute<AssemblyFileVersionAttribute>()?
                    .Version ?? "-.-.-";
                return $"v{v.Split('+')[0]}";
            }
        }

        public AboutView()
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

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri) { UseShellExecute = true });
            e.Handled = true;
        }
    }
}
