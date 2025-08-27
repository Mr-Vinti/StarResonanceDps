using System.Text;
using System.Windows.Forms;
using AntdUI;
using StarResonanceDpsAnalysis.Forms;
using StarResonanceDpsAnalysis.Forms.ModuleForm;
using StarResonanceDpsAnalysis.Plugin;
using StarResonanceDpsAnalysis.Plugin.LaunchFunction;

namespace StarResonanceDpsAnalysis
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

            //Console.OutputEncoding = Encoding.UTF8;
            //Application.SetHighDpiMode(HighDpiMode.PerMonitorV2); // �ؼ�

           // ���������ֱ������� AntdUI ȫ�� DPI ���ţ�ʹ 1080p=1.0��2K��1.33��4K=2.0
           // float dpiScale = GetPrimaryResolutionScale();
            AppConfig.dpi = AntdUI.Config.Dpi;

       
            AntdUI.Config.TextRenderingHighQuality = true;

            

            Application.SetHighDpiMode(HighDpiMode.PerMonitorV2);
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.

            ApplicationConfiguration.Initialize();
            FormManager.dpsStatistics = new DpsStatisticsForm();
           Application.Run(FormManager.dpsStatistics);

            //Application.Run(new ModuleCalculationForm());
        }

        private static float GetPrimaryResolutionScale()
        {
            try
            {
                var bounds = Screen.PrimaryScreen?.Bounds ?? new Rectangle(0, 0, 1920, 1080);
                // ���߶��ж���1080->1.0, 1440->1.33, >=2160->2.0
                if (bounds.Height >= 2160) return 2.0f;       // 4K
                if (bounds.Height >= 1440) return 1.3333f;    // 2K
                return 1.0f;                                   // 1080p ������
            }
            catch
            {
                return 1.0f;
            }
        }
    }
}