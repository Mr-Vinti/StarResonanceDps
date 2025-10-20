using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarResonanceDpsAnalysis.WPF.Plugins.Interfaces
{
    public interface IPlugin
    {
        public string PackageName { get; }
        public string PackageVersion { get; }
        public string GetPluginName(string calture);
        public string GetPluginDescription(string calture);

        void OnRequestRun();

        void OnRequestSetting();
    }
}
