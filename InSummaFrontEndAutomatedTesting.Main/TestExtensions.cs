using InSummaFrontEndAutomatedTesting.Orchestrator.Models;
using Newtonsoft.Json.Linq;
using System.Runtime.InteropServices;

namespace InSummaFrontEndAutomatedTesting.Main
{
    public static class TestExtensions
    {
        public static IEnumerable<TestCaseData> GetTestCases()
        {
            List<TestCaseData> testCases = new();
            DirectoryInfo d = new(Directory.GetCurrentDirectory() + "/TestCases");

            FileInfo[] Files = d.GetFiles("*.json");

            foreach (FileInfo file in Files)
            {
                try
                {
                    JObject testScript = JObject.Parse(File.ReadAllText(file.FullName));

                    if (testScript == null)
                    {
                        continue;
                    }

                    testScript.TryGetValue("osType", out JToken? osType);
                    testScript.TryGetValue("webBrowserType", out JToken? webBrowserType);
                    testScript.TryGetValue("testName", out JToken? testName);

                    if (osType == null || testName == null || webBrowserType == null)
                    {
                        continue;
                    }

                    var os = MapOsTypeToOSPlatform(osType.ToString());
                    if (os != GetSystemOS())
                    {
                        continue;
                    }

                    testCases.Add(new TestCaseData($"{testName}_{webBrowserType}_{osType}", file.FullName, os));
                }
                catch
                {
                    continue;
                }
            }

            return testCases;
        }

        private static OSPlatform GetSystemOS()
        {

            var isWindows = OperatingSystem.IsWindows();

            var isMacOs = OperatingSystem.IsMacOS();
            var isLinux = OperatingSystem.IsLinux();

            if (isWindows)
            {
                return OSPlatform.Windows;
            }

            if (isMacOs)
            {
                return OSPlatform.OSX;
            }

            if (isLinux)
            {
                return OSPlatform.Linux;
            }

            return OSPlatform.Windows;

        }

        private static OSPlatform MapOsTypeToOSPlatform(string osType)
        {
            return osType switch
            {
                "Windows" => OSPlatform.Windows,
                "MacOS" => OSPlatform.OSX,
                "Linux" => OSPlatform.Linux,
                _ => OSPlatform.Windows,
            };
        }
    }
}
