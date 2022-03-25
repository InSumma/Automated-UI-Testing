using InSummaFrontEndAutomatedTesting.BusinessEntities.Enums;

namespace InSummaFrontEndAutomatedTesting.BusinessEntities
{
    public class ActionResult
    {
        public string? TargetElement { get; set; }
        public ActionType ActionType { get; set; }
        public bool IsSuccess { get; set; }
        public string? Message { get; set; }
    }
}
