using System;
using System.Configuration;
using System.IO;
using System.Threading;
using AngleSharp.Dom.Html;
using AngleSharp.Parser.Html;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.IE;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using TechTalk.SpecFlow;

namespace CDAT.Tests.E2E.Helpers
{
    public class BrowserHelper
    {
        private readonly HtmlParser HtmlParser = new HtmlParser();

        private IWebDriver _driver;
        public WebDriverWait Wait;

        public BrowserHelper(string browserType)
        {

        }

        public IWebDriver Driver
        {
            get
            {
                if (_driver != null)
                    return _driver;

                if (ConfigurationManager.AppSettings["UseIE"] == "true")
                {
                    var options =
                        new InternetExplorerOptions
                        {
                            IntroduceInstabilityByIgnoringProtectedModeSettings = true,
                            EnsureCleanSession = true
                        };
                    _driver = new InternetExplorerDriver(options);
                }
                else
                {
                    _driver = new ChromeDriver();
                }

                _driver.Manage().Window.Maximize();
                _driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(3);

                Wait = new WebDriverWait(_driver, new TimeSpan(0,0,3));
                Wait.IgnoreExceptionTypes(typeof(StaleElementReferenceException), typeof(InvalidOperationException), typeof(AssertionException));
                return _driver;
            }
        }

        public void ShutdownBrowser()
        {
            if (_driver != null)
            {
                _driver.Quit();
                _driver.Dispose();
            }
        }

        public void TakeScreenshot(string filePath="")
        {
            if (string.IsNullOrEmpty(filePath))
            {
                string filename = "ScreenshotImage";

                if (ScenarioContext.Current != null)
                    filename = ScenarioContext.Current.ScenarioInfo.Title;

                filename += "_" + DateTime.Now.ToString("yyyy-MM-dd_hh-mm-ss-tt") + ".png";
                filePath = Path.Combine(Environment.CurrentDirectory, filename);
            }

            var screenshot = ((ITakesScreenshot)Driver).GetScreenshot();
            
            screenshot.SaveAsFile(filePath, ScreenshotImageFormat.Png);
        }


        public void DeleteCookiesAndGoToUrl(string url)
        {
            Driver.Manage().Cookies.DeleteAllCookies();
            GoToUrl(url);
        }

        public void GoToUrl(string url)
        {
            string currenturl = Driver.Url;
            string newUrl = TestInitializer.GetAbsoluteUrl(url);

            if (currenturl != newUrl)
                Driver.Navigate().GoToUrl(newUrl);
            else
                Driver.Navigate().Refresh();

            WaitForAjax();
        }

        public void WaitFor(Func<bool> isComplete, string timeoutMsg, int timeout = 3000)
        {
            var waitTime = 200;
            var delay = 0;

            while (delay < timeout)
            {
                if (isComplete())
                    return;

                Thread.Sleep(waitTime);
                delay += waitTime;
            }

            throw new TimeoutException(timeoutMsg);
        }        

        public void WaitForAjax()
        {
            try
            {
                Wait.Until(x => (bool)ExecuteScript("return jQuery && jQuery != null"));
                Wait.Until(x =>
                {
                    var active = (long)ExecuteScript("return jQuery.active");
                    return active == 0;
                });
            }
            catch { }
        }

        public object ExecuteScript(string script)
        {
            return ((IJavaScriptExecutor)Driver).ExecuteScript(script);
        }

        /// <summary>
        /// Returns an IHtmlDocument for parsing the DOM with AngleSharp. Much faster than Selenium WebDriver selectors, especially IE.
        /// Use it when you want to validate the contents of the DOM. It is not suitable, however, if you need to interact with an element or test for visibility. 
        /// </summary>
        /// <param name="container"></param>
        /// <returns></returns>
        public IHtmlDocument ParseHtmlFromWebElement(IWebElement container)
        {
            string innerHtml = container.GetAttribute("innerHTML");
            return HtmlParser.Parse(innerHtml);
        }

        public void ClickTabNavById(string id)
        {
            var tab = Driver.FindElement(By.ClassName("multi-tab-header")).FindElement(By.Id(id));

            if (Driver.IsClickable(tab))
            {
                tab.Click();
            }
            WaitForAjax();
        }

        public void ClearThenSendKeys(IWebElement input, string value)
        {
            //focus on element
            MoveToElement(input);
            input.Clear();
            input.SendKeys(value);
        }

        public void MoveToElement(IWebElement targetElement)
        {
            if (targetElement == null)
                return;

            var builder = new Actions(_driver);
            builder.MoveToElement(targetElement).Build().Perform();
        }
    }
}
