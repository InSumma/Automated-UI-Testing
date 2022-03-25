namespace InSummaFrontEndAutomatedTesting.BusinessEntities
{
    public class RunResult
    {
        public RunResult()
        {
            ActionResults = new List<ActionResult>();
        }

        public bool IsSetUpCompleted { get; set; }
        public bool IsTemplateFileParsed { get; set; }
        public bool IsWebDriverCreated { get; set; }
        public IEnumerable<ActionResult> ActionResults { get; set; }
        public bool TestCompletedSuccessfully { get; set; }
    }
}
