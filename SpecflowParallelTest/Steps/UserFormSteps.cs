﻿using OpenQA.Selenium;
using System.Collections.Generic;
using System.Threading;
using SpecflowParallelTest.Support;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;

namespace SpecflowParallelTest.Steps
{
    [Binding]
    class UserFormSteps
    {
        private readonly IWebDriver _driver;

        public UserFormSteps()
        {
            _driver = BrowserHelper.Driver;
            //_driver = driver;
        }



        [Given(@"I start entering user form details like")]
        public void GivenIStartEnteringUserFormDetailsLike(Table table)
        {
            dynamic data = table.CreateDynamicInstance();
            _driver.FindElement(By.Id("Initial")).SendKeys((string)data.Initial);
            _driver.FindElement(By.Id("FirstName")).SendKeys((string)data.FirstName);
            _driver.FindElement(By.Id("MiddleName")).SendKeys((string)data.MiddleName);
            Thread.Sleep(2000);
        }

        [Given(@"I click submit button")]
        public void GivenIClickSubmitButton()
        {
            //_driver.FindElement(By.Name("Save")).Click();
            BrowserHelper.Driver.FindElement(By.Name("Save")).Click();
        }

        [Given(@"I verify the entered user form details in the application database")]
        public void GivenIVerifyTheEnteredUserFormDetailsInTheApplicationDatabase(Table table)
        {
            //Mock data collection
            List<AUTDatabase> mockAutData = new List<AUTDatabase>()
            {
                new AUTDatabase()
                {
                    FirstName = "Karthik",
                    Initial = "k",
                    MiddleName = "k"
                },

                new AUTDatabase()
                {
                    FirstName = "Prashanth",
                    Initial = "k",
                    MiddleName = "k"
                }
            };

            //For verification with single row data
            var result = table.FindAllInSet(mockAutData);

            //For verification againt Multiple row data
            var resultnew = table.FindAllInSet(mockAutData);

        }


    }

    public class AUTDatabase
    {
        public string Initial { get; set; }

        public string FirstName { get; set; }

        public string MiddleName { get; set; }

        public string Gender { get; set; }
    }



}
