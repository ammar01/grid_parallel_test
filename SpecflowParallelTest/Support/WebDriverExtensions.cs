using System;
using System.Collections.Generic;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using ExpectedConditions = SeleniumExtras.WaitHelpers.ExpectedConditions;


namespace SpecflowParallelTest.Support
{
    public static class WebDriverExtensions
    {
        public static IWebElement FindElement(this IWebDriver driver, By by, int timeoutInSeconds)
        {
            if (timeoutInSeconds > 0)
            {
                var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(timeoutInSeconds));
                return wait.Until(drv => drv.FindElement(@by));
            }
            return driver.FindElement(@by);
        }

        public static IList<IWebElement> FindElements(this IWebDriver driver, By by, int timeoutInSeconds)
        {
            if (timeoutInSeconds > 0)
            {
                var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(timeoutInSeconds));
                return wait.Until(drv => drv.FindElements(@by));
            }
            return driver.FindElements(@by);
        }
        public static void WaitForElementIsVisible(this IWebDriver driver, By by, int timeoutInSeconds)
        {
            WaitForElementIsVisible(driver, @by, TimeSpan.FromSeconds(timeoutInSeconds));
        }

        public static void WaitForElementIsVisible(this IWebDriver driver, By by, TimeSpan timeout)
        {
            if (timeout.TotalSeconds > 0)
            {
                WebDriverWait wait = new WebDriverWait(driver, timeout);
                wait.Until(ExpectedConditions.ElementIsVisible(@by));
            }
        }

        public static void PageDown(this IWebDriver driver, IWebElement element)
        {
            var action = new Actions(driver);
            action.MoveToElement(element);
            action.SendKeys(Keys.End).Build().Perform();

            BrowserHelper.WaitForAjax();
        }

        public static bool IsClickable(this IWebDriver driver, IWebElement element)
        {
            try
            {
                var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(6));
                wait.Until(ExpectedConditions.ElementToBeClickable(element));
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static bool Exists(this IWebDriver driver, By selector)
        {
            try
            {
                driver.FindElement(selector);
            }
            catch
            {
                return false;
            }

            return true;
        }
    }
}
