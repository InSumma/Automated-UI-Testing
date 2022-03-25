using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace InSummaFrontEndAutomatedTesting.Orchestrator.Models
{
    public class TestCaseData
    {
        public TestCaseData()
        {

        }

        public TestCaseData(string name, string path, OSPlatform osType)
        {
            Name = name;
            Path = path;
            OsType = osType;
        }

        public string? Name { get; set; }
        public string? Path { get; set; }
        public OSPlatform? OsType { get; set; }
    }
}
