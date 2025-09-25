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
using System.Windows.Shapes;

namespace StarResonanceDpsAnalysis.WPF.Views
{
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
}
