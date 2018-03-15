using System;
using BoDi;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.IE;
using System.Reflection;
using CDAT.Tests.E2E.Helpers;
using OpenQA.Selenium.Remote;
using TechTalk.SpecFlow;

namespace SpecflowParallelTest
{
    [Binding]
    public class Hooks
    {
        private readonly IObjectContainer _objectContainer;
        private BrowserHelper _browserHelper;
        private IWebDriver _driver;

        public Hooks(IObjectContainer objectContainer)
        {
            _objectContainer = objectContainer;
        }

        [BeforeScenario]
        public void Initialize()
        {
            SelectBrowser(BrowserType.GridIe);
        }

        [AfterScenario]
        public void CleanUp()
        {
            _driver?.Quit();
        }

        internal void SelectBrowser(BrowserType browserType)
        {
            var options =
                new InternetExplorerOptions
                {
                    IntroduceInstabilityByIgnoringProtectedModeSettings = true,
                    EnsureCleanSession = true,
                    InitialBrowserUrl = "http://www.google.co.uk" // this gets around an issue with ie not working with grid in parallel
                };
            switch (browserType)
            {
                case BrowserType.Chrome:
                    //ChromeOptions option = new ChromeOptions();
                    //option.AddArgument("--headless");
                    //_driver = new ChromeDriver(option);
                    _browserHelper = new BrowserHelper("Chrome");
                    _driver = _browserHelper.Driver;
                    //_driver = new ChromeDriver();
                    _objectContainer.RegisterInstanceAs<IWebDriver>(_driver);
                    break;
                case BrowserType.Firefox:
                    var driverDir = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembl‌​y().Location);
                    FirefoxDriverService service = FirefoxDriverService.CreateDefaultService(driverDir, "geckodriver.exe");
                    service.FirefoxBinaryPath = @"C:\Program Files (x86)\Mozilla Firefox\firefox.exe";
                    service.HideCommandPromptWindow = true;
                    service.SuppressInitialDiagnosticInformation = true;
                    _driver = new FirefoxDriver(service);
                    _objectContainer.RegisterInstanceAs<IWebDriver>(_driver);
                    break;
                case BrowserType.Ie:
                    _driver = new InternetExplorerDriver(options);
                    _objectContainer.RegisterInstanceAs<IWebDriver>(_driver);
                    break;
                case BrowserType.GridChrome:
                    ChromeOptions coptions = new ChromeOptions();
                    _driver = new RemoteWebDriver(new Uri("http://localhost:4444/wd/hub"), coptions.ToCapabilities());
                    _objectContainer.RegisterInstanceAs<IWebDriver>(_driver);
                    break;
                case BrowserType.GridIe:
                    _driver = new RemoteWebDriver(new Uri("http://localhost:4444/wd/hub"), options.ToCapabilities());
                    _objectContainer.RegisterInstanceAs<IWebDriver>(_driver);
                    break;
                default:
                    break;
            }
        }
    }

    enum BrowserType
    {
        Chrome,
        Firefox,
        Ie,
        GridChrome,
        GridIe
    }
}
