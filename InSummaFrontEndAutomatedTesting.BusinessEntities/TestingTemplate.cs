using InSummaFrontEndAutomatedTesting.BusinessEntities.Enums;

namespace InSummaFrontEndAutomatedTesting.BusinessEntities
{
    public class TestingTemplate
    {
        public TestingTemplate()
        {
            Actions = new List<Action>();
        }
        public string? TestName { get; set; }
        public string? OsType { get; set; }
        public WebBrowserType WebBrowserType { get; set; }
        public Uri? DriverPath { get; set; }
        public Uri? Url { get; set; }
        public IEnumerable<Action> Actions { get; set; }
        public FinalCondition? FinalCondition { get; set; }
    }
}
