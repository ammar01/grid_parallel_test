using System;
using BoDi;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.IE;
using OpenQA.Selenium.Remote;
using TechTalk.SpecFlow;

namespace SpecflowParallelTest
{
    [Binding]
    public class Hooks
    {
        private readonly IObjectContainer _objectContainer;
        //private IWebDriver _driver;

        [ThreadStatic]
        private static IWebDriver _driver;

        public static IWebDriver Driver => _driver;


        public Hooks(IObjectContainer objectContainer)
        {
            _objectContainer = objectContainer;
        }

        [BeforeScenario]
        public void Initialize()
        {
            SelectBrowser(BrowserType.Ie);
        }

        [AfterScenario]
        public void CleanUp()
        {
            _driver?.Quit();
            _driver?.Dispose();
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
                    _driver = new ChromeDriver();
                    _objectContainer.RegisterInstanceAs(_driver);
                    break;
                case BrowserType.Ie:
                    _driver = new InternetExplorerDriver(options);
                    _objectContainer.RegisterInstanceAs(_driver);
                    break;
                case BrowserType.GridChrome:
                    ChromeOptions coptions = new ChromeOptions();
                    _driver = new RemoteWebDriver(new Uri("http://localhost:4444/wd/hub"), coptions.ToCapabilities());
                    _objectContainer.RegisterInstanceAs(_driver);
                    break;
                case BrowserType.GridIe:
                    _driver = new RemoteWebDriver(new Uri("http://localhost:4444/wd/hub"), options.ToCapabilities());
                    _objectContainer.RegisterInstanceAs(_driver);
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