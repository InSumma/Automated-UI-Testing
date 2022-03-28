using InSummaFrontEndAutomatedTesting.BusinessEntities;
using InSummaFrontEndAutomatedTesting.BusinessEntities.Enums;
using InSummaFrontEndAutomatedTesting.TestingLogic.Interfaces;
using Microsoft.Extensions.Logging;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System.Runtime.CompilerServices;
using Action = InSummaFrontEndAutomatedTesting.BusinessEntities.Action;

[assembly: InternalsVisibleTo("InSummaFrontEndAutomatedTesting.Orchestrator")]
namespace InSummaFrontEndAutomatedTesting.TestingLogic.API
{
    public class SeleniumService : IActionService
    {
        private readonly ILogger<SeleniumService> _logger;
        private static readonly TimeSpan _defaultWaitingTime = new TimeSpan(0, 0, 10);

        public SeleniumService(ILogger<SeleniumService> logger)
        {
            _logger = logger;
        }

        public ActionResult Click(IWebDriver driver, Action action)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(action.TargetElement))
                {
                    throw new ArgumentException($"'{nameof(action.TargetElement)}' cannot be null or whitespace.", nameof(action.TargetElement));
                }

                CreateDelay(action);

                IWebElement element = CreateElement(driver, action.TargetElement, action.LocatorType);   
             
                element.Click();

                _logger.LogInformation("Click action on element with {locator} {targetElement} has been executed successfully.", action.LocatorType.ToString(), action.TargetElement);

                return CreateResult(action);
            }
            catch (Exception e)
            {
                _logger.LogError("Error occurred in SeleniumService.Click (Locator: {locator}, TargetElement: {targetElement}). {message}", action.LocatorType, action.TargetElement, e.Message);
                return CreateResult(action, e);
            }
        }
              
        public ActionResult SendKeys(IWebDriver driver, Action action)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(action.TargetElement))
                {
                    throw new ArgumentException($"'{nameof(action.TargetElement)}' cannot be null or whitespace.", nameof(action.TargetElement));
                }

                if (string.IsNullOrWhiteSpace(action.Data))
                {
                    throw new ArgumentException($"'{nameof(action.Data)}' cannot be null or whitespace.", nameof(action.Data));
                }

                IWebElement element = CreateElement(driver, action.TargetElement, action.LocatorType);

                CreateDelay(action);

                element.SendKeys(action.Data);

                _logger.LogInformation("SendKeys action on element with {locator} {targetElement} has been executed successfully.", action.LocatorType.ToString(), action.TargetElement);

                return CreateResult(action);
            }
            catch (Exception e)
            {
                _logger.LogError("Error occurred in SeleniumService.SendKeys (Locator: {locator}, TargetElement: {targetElement}). {message}", action.LocatorType, action.TargetElement, e.Message);
                return CreateResult(action, e);
            }
        }

        public bool Validate(IWebDriver driver, FinalCondition validationCondition)
        {
            if (validationCondition.LocatorType != ValidatorLocatorType.URL && string.IsNullOrWhiteSpace(validationCondition.TargetElement))
            {
                throw new ArgumentException($"'{nameof(validationCondition.TargetElement)}' cannot be null or whitespace.", nameof(validationCondition.TargetElement));
            }

            if(driver == null)
            {
                throw new ArgumentNullException(nameof(driver));
            }

            if(validationCondition.LocatorType == ValidatorLocatorType.URL && validationCondition.Url != null)
            {
                return driver.Url.Equals(new Uri(validationCondition.Url).AbsoluteUri);
            }

            Thread.Sleep(_defaultWaitingTime); // Create a gap of 10 seconds between last click and the validation process.
            var searchCriteria = CreateSearchCriteria(validationCondition.TargetElement, validationCondition.LocatorType);
            return driver.FindElements(searchCriteria).Count != 0;
        }

        private static void CreateDelay(Action action)
        {
            if (action.DelayInSeconds > 0)
            {
                Thread.Sleep(new TimeSpan(0, 0, action.DelayInSeconds));
            }
        }

        private static ActionResult CreateResult(Action action)
        {
            return new ActionResult() { ActionType = action.ActionType, TargetElement = action.TargetElement, IsSuccess = true };
        }

        private static ActionResult CreateResult(Action action, Exception e)
        {
            return new ActionResult() { ActionType = action.ActionType, TargetElement = action.TargetElement, IsSuccess = false, Message = e.Message };
        }

        private static IWebElement CreateElement(IWebDriver driver, string locator, LocatorType locatorType)
        {
            WebDriverWait wait = new(driver, _defaultWaitingTime);
            wait.Until(ec => ec.FindElement(CreateSearchCriteria(locator, locatorType)));

            By searchCriteria = CreateSearchCriteria(locator, locatorType);
         
            IWebElement element = driver.FindElement(searchCriteria);
   
            return element;
        }

        private static By CreateSearchCriteria(string locator, LocatorType locatorType)
        {
            return locatorType switch
            {
                LocatorType.CLASSNAME => By.ClassName(locator),
                LocatorType.ID => By.Id(locator),
                LocatorType.XPATH => By.XPath(locator),
                _ => By.Id(locator),
            };
        }

        private static By CreateSearchCriteria(string locator, ValidatorLocatorType locatorType)
        {
            return locatorType switch
            {
                ValidatorLocatorType.CLASSNAME => By.ClassName(locator),
                ValidatorLocatorType.ID => By.Id(locator),
                ValidatorLocatorType.XPATH => By.XPath(locator),
                _ => By.Id(locator),
            };
        }
    }
}
