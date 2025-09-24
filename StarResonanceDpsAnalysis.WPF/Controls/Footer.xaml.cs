using System;
using System.Collections.Generic;
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

namespace StarResonanceDpsAnalysis.WPF.Controls
{
    /// <summary>
    /// Footer.xaml 的交互逻辑
    /// </summary>
    public partial class Footer : UserControl
    {
        public event RoutedEventHandler? ConfirmClick;
        public event RoutedEventHandler? CancelClick;

        public Footer()
        {
            InitializeComponent();
        }

        private void Confirm_Click(object sender, RoutedEventArgs e)
        {
            ConfirmClick?.Invoke(sender, e);
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            CancelClick?.Invoke(sender, e);
        }
    }
}
