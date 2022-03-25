using InSummaFrontEndAutomatedTesting.BusinessEntities.Enums;
using InSummaFrontEndAutomatedTesting.TestingLogic.Interfaces;
using Microsoft.Extensions.Logging;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Safari;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("InSummaFrontEndAutomatedTesting.Orchestrator")]
namespace InSummaFrontEndAutomatedTesting.TestingLogic
{
    public class WebDriverInitializer : IWebDriverInitializer
    {
        private readonly ILogger<WebDriverInitializer> _logger;

        public WebDriverInitializer(ILogger<WebDriverInitializer> logger)
        {
            _logger = logger;
        }

        public IWebDriver? InitializeWebDriver(WebBrowserType webBrowserType, Uri? driverPath, Uri webUrl)
        {
            try
            {
                if (webUrl is null)
                {
                    throw new ArgumentNullException(nameof(webUrl));
                }

                IWebDriver driver;

                if (driverPath is null)
                {
                    driver = webBrowserType switch
                    {
                        WebBrowserType.CHROME => new ChromeDriver(CreateChromeOptions()),
                        WebBrowserType.EDGE => new EdgeDriver(),
                        WebBrowserType.FIREFOX => new FirefoxDriver(),
                        WebBrowserType.SAFARI => new SafariDriver(),
                        _ => new ChromeDriver(CreateChromeOptions())
                    };
                }
                else
                {
                    driver = webBrowserType switch
                    {
                        WebBrowserType.CHROME => new ChromeDriver(driverPath.LocalPath, CreateChromeOptions()),
                        WebBrowserType.EDGE => new EdgeDriver(driverPath.LocalPath),
                        WebBrowserType.FIREFOX => new FirefoxDriver(driverPath.LocalPath),
                        WebBrowserType.SAFARI => new SafariDriver(driverPath.LocalPath),
                        _ => new ChromeDriver(driverPath.LocalPath, CreateChromeOptions())
                    };
                }

                driver.Url = webUrl.AbsoluteUri;
            
                return driver;
            }
            catch (Exception e)
            {
                _logger.LogError("Error occurred in WebDriverInitializer.InitializeWebDriver: {message}", e.Message);
                return null;
            }
        }

        private static ChromeOptions CreateChromeOptions()
        {
            ChromeOptions options = new();
            options.AddArguments("--no-sandbox");
            options.AddArguments("--headless");
            options.AddArguments("--disable-dev-shm-usage");
            return options;
        }
    }
}