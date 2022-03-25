using InSummaFrontEndAutomatedTesting.BusinessEntities;
using OpenQA.Selenium;
using System.Runtime.CompilerServices;
using Action = InSummaFrontEndAutomatedTesting.BusinessEntities.Action;

[assembly: InternalsVisibleTo("InSummaFrontEndAutomatedTesting.Orchestrator")]
namespace InSummaFrontEndAutomatedTesting.TestingLogic.Interfaces
{
    public interface IActionService
    {
        ActionResult Click(IWebDriver driver, Action action);
        ActionResult SendKeys(IWebDriver driver, Action action);
        bool Validate(IWebDriver driver, FinalCondition validationConditions);
    }
}
