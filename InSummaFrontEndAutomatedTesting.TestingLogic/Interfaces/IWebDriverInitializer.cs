using InSummaFrontEndAutomatedTesting.BusinessEntities.Enums;
using OpenQA.Selenium;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("InSummaFrontEndAutomatedTesting.Orchestrator")]
namespace InSummaFrontEndAutomatedTesting.TestingLogic.Interfaces
{
    public interface IWebDriverInitializer
    {
        IWebDriver? InitializeWebDriver(WebBrowserType webBrowserType, Uri? driverPath, Uri webUrl);
    }
}
