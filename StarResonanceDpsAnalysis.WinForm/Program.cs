using System.Text;
using System.Windows.Forms;

using StarResonanceDpsAnalysis.WinForm.Forms;

namespace StarResonanceDpsAnalysis.WinForm
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            AppDomain.CurrentDomain.UnhandledException += (sender, e) =>
            {
                // ��ʱʲô��������
            };
            Application.ThreadException += (sender, e) =>
            {
                // ��ʱʲô��������
            };

            ApplicationConfiguration.Initialize();

            FormManager.dpsStatistics = new DpsStatisticsForm();
            Application.Run(FormManager.dpsStatistics);
        }
    }
}