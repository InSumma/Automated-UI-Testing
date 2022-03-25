using InSummaFrontEndAutomatedTesting.BusinessEntities;
using InSummaFrontEndAutomatedTesting.Orchestrator;

namespace InSummaFrontEndAutomatedTesting.Main
{
    public static class StartUp
    {
        public static RunResult Run(Uri jsonFilePath)
        {
            var result = Initializer.Run(jsonFilePath);
            return result;
        }
    }
}
