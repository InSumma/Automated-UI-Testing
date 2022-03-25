using InSummaFrontEndAutomatedTesting.BusinessEntities;
using InSummaFrontEndAutomatedTesting.Orchestrator;
using InSummaFrontEndAutomatedTesting.Orchestrator.Interfaces;
using InSummaFrontEndAutomatedTesting.Parser;
using InSummaFrontEndAutomatedTesting.Parser.Interfaces;
using InSummaFrontEndAutomatedTesting.TestingLogic;
using InSummaFrontEndAutomatedTesting.TestingLogic.API;
using InSummaFrontEndAutomatedTesting.TestingLogic.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("InSummaFrontEndAutomatedTesting.Main")]
namespace InSummaFrontEndAutomatedTesting.Orchestrator
{
    public static class Initializer
    {
        private static ServiceProvider? _serviceProvider;

        public static RunResult Run(Uri actionTemplate)
        {
            SetUp();

            if(_serviceProvider != null)
            {
                var orchestrator = _serviceProvider.GetService<ITestingOrchestrator>();
                if (orchestrator != null)
                {
                    var result = orchestrator.Orchestrate(actionTemplate);
                    return result;
                }
            }

            return new RunResult();
        }

        private static bool SetUp()
        {
                var collection = new ServiceCollection()
                .AddScoped<ITestingOrchestrator, TestingOrchestrator>()
                .AddScoped<IJsonParser, JsonParser>()
                .AddScoped<IWebDriverInitializer, WebDriverInitializer>()
                .AddScoped<IActionService, SeleniumService>();

                collection.AddLogging(config =>
                {
                    config.AddDebug();

                    config.AddConsole();
                });

                var result = collection.Count > 0;

                _serviceProvider = collection.BuildServiceProvider();

                return result;
            }   
    }
}
