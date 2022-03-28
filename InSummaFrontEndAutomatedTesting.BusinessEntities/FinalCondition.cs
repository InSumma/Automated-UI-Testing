using InSummaFrontEndAutomatedTesting.BusinessEntities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InSummaFrontEndAutomatedTesting.BusinessEntities
{
    public class FinalCondition
    {
        public string? TargetElement { get; set; }
        public ValidatorLocatorType LocatorType { get; set; }
        public string? Url { get; set; }
    }
}
