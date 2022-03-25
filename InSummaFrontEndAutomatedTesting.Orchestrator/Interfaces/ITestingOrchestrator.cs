using InSummaFrontEndAutomatedTesting.BusinessEntities;

namespace InSummaFrontEndAutomatedTesting.Orchestrator.Interfaces
{
    internal interface ITestingOrchestrator
    {
        RunResult Orchestrate(Uri jsonTemplateFilePath);
    }
}
