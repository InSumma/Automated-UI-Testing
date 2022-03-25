using InSummaFrontEndAutomatedTesting.BusinessEntities.Enums;

namespace InSummaFrontEndAutomatedTesting.BusinessEntities
{
    public class Action
    {
        public string? BluePrintName { get; set; }
        public ActionType ActionType { get; set; }
        public string? TargetElement { get; set; }
        public LocatorType LocatorType { get; set; }
        public string? Data { get; set; }
        public int DelayInSeconds { get; set; }

    }
}
