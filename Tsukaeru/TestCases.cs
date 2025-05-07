using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.ObjectModel;
using System.IO;

namespace SeleniumCsharp
{
    public class Tests
    {
        IWebDriver webDriver;
        //private  webDriverWait = null;
        [OneTimeSetUp]
        public void Setup()
        {
            ChromeOptions chromeOptions = new ChromeOptions();
            chromeOptions.AddArgument("--no-sandbox");
            chromeOptions.AddArgument("--start-maximized");
            chromeOptions.AddArgument("--disable-xss-auditor");
            chromeOptions.AddArgument("--remote-debugging-port=9222");
            webDriver = new ChromeDriver(ChromeDriverService.CreateDefaultService(), chromeOptions, TimeSpan.FromMinutes(2));
            var webDriverWait = new WebDriverWait(webDriver, new TimeSpan(0, 0, 10));

        }

        [Test]
        public void VerifyGmailAvailable()
        {
            webDriver.Navigate().GoToUrl("https://www.google.com/");
            Assert.That(webDriver.FindElement(By.XPath("//a[text()='Gmail']")).Displayed);
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            if (webDriver != null)
            {
                webDriver.Quit();
                webDriver.Dispose();
            }
        }
    }//Tests
}//SeleniumCsharp