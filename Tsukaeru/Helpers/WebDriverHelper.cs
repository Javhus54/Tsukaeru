using System;
using System.Collections.Generic;
using System.Configuration;
using System.Reflection;
using System.Runtime.Remoting;
using OpenQA.Selenium;
using OpenQA.Selenium.IE;
using OpenQA.Selenium.Chrome;
using System.Globalization;
using System.Drawing;
using System.Linq;
using DocumentFormat.OpenXml.Bibliography;
using Tsukaeru;

namespace Tsukaeru.Helpers
{
    public static class WebDriverHelper
    {
        private static readonly Dictionary<string, IWebDriver> WedDriverDict = new Dictionary<string, IWebDriver>();
        private static readonly object DictLock = new object(); // Used to coordinate thread access of WedDriverDict
        private static bool pageTrackerInit = false;
        public static string GetTestFixtureName()
        {
            string fixtureName = NUnit.Framework.TestContext.CurrentContext.Test.FullName.ToString();
            fixtureName = fixtureName.Substring(fixtureName.IndexOf("Features") + 9, fixtureName.Length - (fixtureName.IndexOf("Features") + 9)); // Strip DomainName
            if ((fixtureName.Split('.').Length - 1) >= 1)
                return fixtureName.Split('.')[1];
            //return (fixtureName.Split('.')[1][0] == '_') ? fixtureName.Split('.')[1].Substring(1, fixtureName.Split('.')[1].Length - 1) : fixtureName.Split('.')[1];
            Console.WriteLine(fixtureName);
            return fixtureName;
        }
        public static string GetTestAssemblyName()
        {
            string fixtureName = NUnit.Framework.TestContext.CurrentContext.Test.FullName.ToString();
            return fixtureName.Substring(fixtureName.IndexOf("Test") + 5, (fixtureName.IndexOf("Features") - (fixtureName.IndexOf("Test") + 6)));
        }
        public static string GetTestScenarioName()
        {
            string scenarioName = NUnit.Framework.TestContext.CurrentContext.Test.FullName.ToString();
            scenarioName = scenarioName.Substring(scenarioName.IndexOf("Features") + 9, scenarioName.Length - (scenarioName.IndexOf("Features") + 9)); // Strip DomainName
            return scenarioName;
        }
        public static string GetTestCaseName()
        {
            string scenarioName = NUnit.Framework.TestContext.CurrentContext.Test.FullName.ToString();
            scenarioName = scenarioName.Substring(scenarioName.IndexOf("Features") + 9, scenarioName.Length - (scenarioName.IndexOf("Features") + 9)); // Strip DomainName
            scenarioName = scenarioName.Replace(GetTestFixtureName(), "");
            scenarioName = scenarioName.Substring(scenarioName.IndexOf("..") + 2, scenarioName.Length - (scenarioName.IndexOf("..") + 2));
            if (scenarioName[0] == '_')
                scenarioName = scenarioName.Substring(1, scenarioName.Length - 1);
            return scenarioName;
        }
        public static void InstantiateWebDriver()
        {
            IWebDriver webDriver;
            Directory.CreateDirectory(Defaults.OUTPUT_DIRECTORY);
            Directory.CreateDirectory(Defaults.TEMPORARY_DIRECTORY);
            Directory.CreateDirectory(Defaults.DOWNLOADS_DIRECTORY);
            Directory.CreateDirectory(Defaults.LOGS_DIRECTORY);
            string fixture = GetTestFixtureName().ToString();
            LogHelper.Log(LogHelper.LEVEL.INFO, null, "WebDriverFactory.InstantiateWebDriver() BrowserType = '{0}': for feature '{1}' Starting Inititaion", ConfigurationManager.AppSettings.Get("BrowserType"), fixture);
            switch (ConfigurationManager.AppSettings.Get("BrowserType"))
            {
                case "Chrome":
                    ChromeOptions chromeOptions = new ChromeOptions();
                    chromeOptions.AddArgument("--no-sandbox");
                    chromeOptions.AddArgument("--start-maximized");
                    chromeOptions.AddArgument("--disable-xss-auditor");
                    chromeOptions.AddUserProfilePreference("download.default_directory", @Defaults.DOWNLOADS_DIRECTORY.ToString());
                    chromeOptions.AddUserProfilePreference("autofill.profile_enabled", false);
                    webDriver = new ChromeDriver(ChromeDriverService.CreateDefaultService(), chromeOptions, TimeSpan.FromMinutes(6));
                    //if(iSync)
                    //{ ngWebDriver.IgnoreSynchronization = iSync; } // set IgnoreSynchronization = true if the application is NOT Anguler
                    //else { }
                    break;

                default:
                    ChromeOptions chromeOptions1 = new ChromeOptions();
                    chromeOptions1.AddArgument("--start-maximized");
                    webDriver = new ChromeDriver(ChromeDriverService.CreateDefaultService(), chromeOptions1, TimeSpan.FromMinutes(2));
                    break;
            }
            lock (DictLock)
            {
                WedDriverDict.Add(fixture, webDriver);
            }
            LogHelper.Log(LogHelper.LEVEL.INFO, null, "WebDriverFactory.InstantiateWebDriver() BrowserType = '{0}': for feature '{1}' Inititaion Complete", ConfigurationManager.AppSettings.Get("BrowserType"), fixture);
        }
        public static IWebDriver GetCurrentWebDriver()
        {
            lock (DictLock)
            {
                if (!WedDriverDict.ContainsKey(GetTestFixtureName().ToString()))
                {
                    InstantiateWebDriver();
                }
                LogHelper.Log(LogHelper.LEVEL.DEBUG, null, "WebDriverFactory.GetCurrentWebDriver()");
                return WedDriverDict[GetTestFixtureName().ToString()];
            }
        }
        public static void QuitCurrentWebDriver()
        {
            lock (DictLock)
            {
                string fixture = GetTestFixtureName().ToString();
                if (WedDriverDict.ContainsKey(fixture))
                {
                    WedDriverDict[fixture].Quit();
                    WedDriverDict[fixture].Dispose();
                    WedDriverDict.Remove(fixture);
                }
            }
            LogHelper.Clear();
        }
        public static void DeleteAllCookies()
        {
            lock (DictLock)
            {
                LogHelper.Log(LogHelper.LEVEL.INFO, null, "WebDriverFactory.DeleteAllCookies()");
                GetCurrentWebDriver().Manage().Cookies.DeleteAllCookies();
                String originalHandle = GetCurrentWebDriver().CurrentWindowHandle;

                //Do something to open new tabs

                foreach (String handle in GetCurrentWebDriver().WindowHandles)
                {
                    if (!(handle == originalHandle))
                    {
                        GetCurrentWebDriver().SwitchTo().Window(handle);
                        GetCurrentWebDriver().Close();
                    }
                }
                GetCurrentWebDriver().SwitchTo().Window(originalHandle);
            }
        }

        public static string GetCurrentPageObject(Assembly executingAssembly)
        {
            string currentUrl = WebDriverHelper.GetCurrentWebDriver().Url.ToString().ToLower();
            // Strip all url arguments, after the .aspx extension, in order to find the page correctly e.g. "?batchId=1"
            if (currentUrl.Contains(".aspx"))
            {
                currentUrl = currentUrl.Remove((currentUrl.IndexOf(".aspx") + 5), currentUrl.Length - (currentUrl.IndexOf(".aspx") + 5));
            }

            if (currentUrl.Contains("cardpaymentdetails/payment/validation"))
            {
                currentUrl = currentUrl.Remove(currentUrl.IndexOf("validation/") + 11);
            }
            if (ConfigurationManager.AppSettings.Get("IsAngular").Equals("True", StringComparison.InvariantCultureIgnoreCase))//, StringComparison.CurrentCultureIgnoreCase)) ;
            {
                currentUrl += Defaults.CurrentPage.ToLower();
            }

            lock (Defaults.PageDictLock)
            {
                if (!pageTrackerInit)
                {
                    foreach (Type type in executingAssembly.GetTypes())
                    {
                        //The condition is ignore an dynamically created entry in the Assembly due to some switch case implemented
                        //down stream in the test code.
                        if (!type.Name.Equals("<PrivateImplementationDetails>") && type.Name.ToLower().Contains("page"))
                        {
                            ObjectHandle handle = Activator.CreateInstance(executingAssembly.FullName, type.FullName);
                            Object page = handle.Unwrap();
                        }
                    }
                    pageTrackerInit = true;
                }

                if (Defaults.PAGE_OBJECT_URLS.ContainsKey(currentUrl))
                {
                    return Defaults.PAGE_OBJECT_URLS[currentUrl];
                }
                else
                {
                    // We truncate the last character until we find the closest match or we match the BaseURL
                    while (!String.Equals(currentUrl, "") && currentUrl.Length > 1)
                    {
                        //if (currentUrl.Equals("https://d1.fisintegratedpayables.com/fis/customerlogin.aspx"))//For loading login page's Page objects on set password page
                        //{
                        //	currentUrl = "https://10.238.233.179/fis/customerlogin.aspx?";
                        //}
                        //else if (currentUrl.Contains("pr.fisintegratedpayables"))
                        //{
                        //	currentUrl = currentUrl.Replace("pr.fisintegratedpayables.com", "10.238.233.179");
                        //	currentUrl += "?";
                        //}
                        currentUrl = currentUrl.Substring(0, currentUrl.Length - 1);
                        if (Defaults.PAGE_OBJECT_URLS.ContainsKey(currentUrl))
                        {
                            return Defaults.PAGE_OBJECT_URLS[currentUrl];
                        }
                    }
                }
            }
            return String.Empty;
        }
        public static void AddPageObject(string PageUrl, string PageObject)
        {
            lock (Defaults.PageDictLock)
            {
                if (!Defaults.PAGE_OBJECT_URLS.ContainsKey(PageUrl.ToLower()))
                {
                    LogHelper.Log(LogHelper.LEVEL.DEBUG, null, "WebDriverFactory.AddPageObject(PageUrl =  '{0}', PageObject = '{1}'): added page to PAGE_OBJECT_URLS", PageUrl, PageObject);
                    Defaults.PAGE_OBJECT_URLS.Add((PageUrl).ToLower(), PageObject);
                }
            }
        }
    }

} // WebDriverFactory // NSpector.Core