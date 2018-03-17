using System;
using System.IO;
using System.Linq;
using System.Configuration;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace SpecflowParallelTest.Support
{
    public static class TestInitializer
    {
        private static int _iisPort;
        private static Process _iisProcess;

        private static readonly string _applicationName = "CDAT";
        private static readonly string _physicalPathReplacementString = "{site_location}";
        private static readonly string _portReplacementString = "{iis_port}";
        private static readonly string CIMachineName = ConfigurationManager.AppSettings["CIMachineName"];

        public static void StartIIS()
        {
            if (_iisProcess != null && !_iisProcess.HasExited)
                return;
            
            if (Environment.UserDomainName.Equals("WWTHC", StringComparison.OrdinalIgnoreCase))
            {
                _iisPort = 8081;
                return;
            }

            _iisPort = new Random().Next(44300, 44399);

            var solutionPath = GetSolutionFolder();

            if (Regex.IsMatch(Environment.MachineName, CIMachineName, RegexOptions.IgnoreCase))
            {
                solutionPath = Directory.GetParent(solutionPath).FullName;
            }

            var applicationPath = Directory.GetDirectories(solutionPath, "CDAT").FirstOrDefault();

            var configPath = Directory.GetFiles($@"{solutionPath}\", "hostConfigTemplate.config*", SearchOption.AllDirectories).FirstOrDefault();
            var templateFile = new FileInfo(configPath);
            var tempConfigPath = $@"{templateFile.Directory.FullName}\tempHost.config";

            string text = File.ReadAllText(configPath);
            text = text.Replace(_physicalPathReplacementString, applicationPath);
            text = text.Replace(_portReplacementString, _iisPort.ToString());
            File.WriteAllText(tempConfigPath, text);

            var programFiles = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);

            _iisProcess = new Process
            {
                StartInfo =
                {
                    FileName = programFiles + @"\IIS Express\iisexpress.exe",
                    Arguments = $@"/config:{tempConfigPath} /site:{_applicationName} /trace:info /systray:false"
                }
            };
            _iisProcess.Start();
        }

        private static string GetSolutionFolder()
        {
            return Path.GetDirectoryName(Path.GetDirectoryName(Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory)));
        }

        public static string GetAbsoluteUrl(string relativeUrl)
        {
            if (!relativeUrl.StartsWith("/"))
            {
                relativeUrl = "/" + relativeUrl;
            }
            return $"https://localhost:{_iisPort}{relativeUrl}";
        }

        public static void KillIIS()
        {
            if (_iisProcess != null && _iisProcess.HasExited == false)
            {
                _iisProcess.Kill();
            }
        }
    }
}
