using InSummaFrontEndAutomatedTesting.BusinessEntities;
using InSummaFrontEndAutomatedTesting.Orchestrator.Interfaces;
using InSummaFrontEndAutomatedTesting.Parser.Interfaces;
using InSummaFrontEndAutomatedTesting.TestingLogic.Interfaces;
using Microsoft.Extensions.Logging;
using OpenQA.Selenium;
using System.Reflection;

namespace InSummaFrontEndAutomatedTesting.Orchestrator
{
    internal class TestingOrchestrator : ITestingOrchestrator
    {
        private readonly RunResult _runResult;

        private readonly IJsonParser _jsonParser;
        private readonly IWebDriverInitializer _webDriverInitializer;
        private readonly IActionService _actionService;
        private readonly ILogger<TestingOrchestrator> _logger;

        private IWebDriver? _webDriver;

        public TestingOrchestrator(IJsonParser jsonParser, IWebDriverInitializer webDriverInitializer, IActionService actionService, ILogger<TestingOrchestrator> logger)
        {
            _jsonParser = jsonParser;
            _webDriverInitializer = webDriverInitializer;
            _actionService = actionService;
            _logger = logger;

            _runResult = new RunResult();
        }

        public RunResult Orchestrate(Uri jsonTemplateFilePath)
        {
            try
            {
                _logger.LogInformation("Setup has been completed. Orchestrator starting...");
                _runResult.IsSetUpCompleted = true;

                var testingTemplate = ParseActionJson(jsonTemplateFilePath);
                if (testingTemplate == null)
                {
                    return _runResult;
                }
                _runResult.IsTemplateFileParsed = true;
                _logger.LogInformation("Template file has been parsed.");

                var webDriver = CreateWebDriver(testingTemplate);
                if (webDriver == null)
                {
                    return _runResult;
                }
                _webDriver = webDriver;
                _runResult.IsWebDriverCreated = true;
                _logger.LogInformation("Webdriver has been created.");

                ExecuteActions(testingTemplate, webDriver);
                _logger.LogInformation("{count} actions have been executed successfully and {countFailed} actions failed.", _runResult.ActionResults.Where(x => x.IsSuccess).Count(), _runResult.ActionResults.Where(x => !x.IsSuccess).Count());

                _runResult.TestCompletedSuccessfully = ValidateResult(testingTemplate, webDriver);
                if (_runResult.TestCompletedSuccessfully)
                {
                    _logger.LogInformation("Testscript has been executed successfully.");
                }
                else
                {
                    _logger.LogInformation("Expected final scenario doesn't meet the requirements. Testscript hasn't been executed successfully.");
                }

                StopWebDriver(webDriver);

                return _runResult;
            }
            catch (Exception e)
            {
                _logger.LogError("Error occurred in TestingOrchestrator.Orchestrate: {message}", e.Message);
                StopWebDriver(_webDriver);
                return _runResult;
            }
        }

        private TestingTemplate? ParseActionJson(Uri jsonTemplateFilePath)
        {
            var parsedFile = _jsonParser.ParseJsonToTestingTemplate(jsonTemplateFilePath);
            return parsedFile;
        }

        private IWebDriver? CreateWebDriver(TestingTemplate testingTemplate)
        {
            if(testingTemplate.Url == null)
            {
                throw new ArgumentNullException("Error occurred in TestingOrchestrator.CreateWebDriver: Url is empty.");
            }

            var webDriver = _webDriverInitializer.InitializeWebDriver(testingTemplate.WebBrowserType, testingTemplate.DriverPath, testingTemplate.Url);
            return webDriver;
        }

        private void ExecuteActions(TestingTemplate template, IWebDriver? webDriver)
        {
         
            var actionResults = new List<ActionResult>();
            string invocationDebugInformation = String.Empty; //Used to log the action that failed to invoke.
            try
            {
                foreach (var action in template.Actions)
                {
                    invocationDebugInformation = $"{action.ActionType} {action.LocatorType} {action.TargetElement}";
                    var methodName = Enum.GetName(action.ActionType);
                    if (methodName != null)
                    {
                        _logger.LogInformation("Trying to invoke method {methodName}.", methodName);
                        var t = _actionService.GetType();
                        MethodInfo? method = t.GetMethod(methodName, BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance | BindingFlags.IgnoreCase);
                        if (method != null && webDriver != null)
                        {
                            var invocationResult = method.Invoke(_actionService, new Object[] { webDriver, action });
                            if (invocationResult != null)
                            {
                                actionResults.Add((ActionResult)invocationResult);
                            }
                            else
                            {
                                _logger.LogInformation("Invocation result of method {methodName} is null.", methodName);
                            }
                        }
                        else
                        {
                            _logger.LogInformation("Could not find method {methodName}.", methodName);
                        }

                    }
                }
            } catch (Exception e)
            {
                _logger.LogError("Error occurred in TestingOrchestrator.ExecuteActions: {message}. {invocationDebugInformation}", e.Message, invocationDebugInformation);
            }

            _runResult.ActionResults = actionResults;
        }

        private bool ValidateResult(TestingTemplate? template, IWebDriver? webDriver)
        {
            if (template == null || template.FinalCondition == null || webDriver == null)
            {
                return false;
            }

            return _actionService.Validate(webDriver, template.FinalCondition);
        }

        private void StopWebDriver(IWebDriver? webDriver)
        {
            if(webDriver != null)
            {
                webDriver.Quit();
                webDriver.Dispose();
                _logger.LogInformation("WebDriver successfully stopped and disposed.");
            }
        }
    }
}