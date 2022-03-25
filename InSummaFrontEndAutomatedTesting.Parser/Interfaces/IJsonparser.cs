using InSummaFrontEndAutomatedTesting.BusinessEntities;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("InSummaFrontEndAutomatedTesting.Orchestrator")]
namespace InSummaFrontEndAutomatedTesting.Parser.Interfaces
{
    internal interface IJsonParser
    {
        TestingTemplate? ParseJsonToTestingTemplate(Uri fileLocation);
    }
}
