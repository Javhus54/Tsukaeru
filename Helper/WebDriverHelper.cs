using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium.Interactions;

namespace Tsukaeru
{
    public class WebDriverHelper
    {
        IWebDriver driver;
        public IWebDriver GetWebDriver()
        {
            return driver;
        }
        public void Start_Browser()
        {
            driver = new FirefoxDriver();
            driver.Manage().Window.Maximize();
        }
        public void Close_Browser()
        {
            driver.Quit();
        }
    }
}
