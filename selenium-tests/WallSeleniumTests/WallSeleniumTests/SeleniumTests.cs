
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;

namespace SeleniumTests
{
    public abstract class SeleniumTests
    {
        protected readonly string userID = "1";

        public SeleniumTests() { }

        protected IWebDriver driver;

        protected void LogIn()
        {
            driver.FindElement(By.Id("log-in-input-userID")).SendKeys(userID);
            driver.FindElement(By.Id("log-in-button")).Click();
            Thread.Sleep(2000);
        }

        protected List<int> GetDivIdsList(string divName)
        {
            List<int> Ids = new List<int>();
            ReadOnlyCollection<IWebElement> div = driver.FindElements(By.Id(divName));
            for (int i = 0; i < div.Count; i++)
            {
                Ids.Add(int.Parse(div[i].GetAttribute("innerHTML").Split('-')[1]));
            }
            return Ids;
        }

        protected int GetNewId(List<int> oldIds, string divName)
        {
            ReadOnlyCollection<IWebElement> div = driver.FindElements(By.Id(divName));
            for (int i = 0; i < div.Count; i++)
            {
                int id = int.Parse(div[i].GetAttribute("innerHTML").Split('-')[1]);
                if (!oldIds.Contains(id))
                {
                    return id;
                }
            }
            return -1;
        }

        protected void SetUpDriver(string url)
        {
            FirefoxOptions options = new FirefoxOptions
            {
                AcceptInsecureCertificates = true
            };
            options.AddArgument("--headless");

            driver = new FirefoxDriver(options);
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
            driver.Navigate().GoToUrl(url);
        }

        protected void WaitAfterReloadUntil(Func<IWebDriver, IWebElement> conditions)
        {
            Thread.Sleep(2500);
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
            wait.Until(conditions);
        }
    }
}
