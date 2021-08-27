using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Protractor;
using System.Reflection;
using OpenQA.Selenium.IE;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Edge;
using System.Runtime.Remoting;
using System.Configuration;

namespace Tsukaeru
{
    public class WebDriverHelper
    {
        private static Dictionary<string, NgWebDriver> WedDriverDict = new Dictionary<string, NgWebDriver>();
		private static object DictLock = new object(); // Used to coordinate thread access of WedDriverDict
		private static bool pageTrackerInit = false;
		public static string GetTestFixtureName()
		{
			string fixtureName = NUnit.Framework.TestContext.CurrentContext.Test.FullName.ToString();
            //Mention folder names of feature files
			List<string> folderNames = new List<string>{"Temp"};
			fixtureName = fixtureName.Substring(fixtureName.IndexOf("Features") + 9, fixtureName.Length - (fixtureName.IndexOf("Features") + 9)); // Strip DomainName
			string[] names = fixtureName.Split('.');
			if (names.Length == 1)
				fixtureName = names[0];
            else
            {
                if (folderNames.Contains(names[0]))
                    fixtureName = names[1];
                else
                    fixtureName = names[0];
            }
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
		public static void InstantiateWebDriver()
		{
			IWebDriver webDriver;
			NgWebDriver ngWebDriver;
			string browserType = "edge";
			string fixture = GetTestFixtureName();
			switch (browserType)
			{
				case "IE":
					InternetExplorerOptions IeOptions = new InternetExplorerOptions { EnableNativeEvents = false, IgnoreZoomLevel = true };
					IeOptions.AddAdditionalCapability("disable-popup-blocking", true);
					webDriver = new InternetExplorerDriver(IeOptions);
					webDriver.Manage().Window.Maximize();
					ngWebDriver = new NgWebDriver(webDriver);
					ngWebDriver.IgnoreSynchronization = string.Equals(ConfigurationManager.AppSettings.Get("IsAngular"), "True", StringComparison.OrdinalIgnoreCase) ? false : true;
					break;

				case "Chrome":
					ChromeOptions chromeOptions = new ChromeOptions();
					chromeOptions.AddArgument("--no-sandbox");
					chromeOptions.AddArgument("--start-maximized");
					chromeOptions.AddArgument("--disable-xss-auditor");
					chromeOptions.AddArgument("--remote-debugging-port=9222");
					chromeOptions.AddUserProfilePreference("download.default_directory", @Constants.DOWNLOADS_DIRECTORY.ToString());
                    webDriver = new ChromeDriver(ChromeDriverService.CreateDefaultService(), chromeOptions, TimeSpan.FromMinutes(2));
					ngWebDriver = new NgWebDriver(webDriver);
					ngWebDriver.IgnoreSynchronization = string.Equals(ConfigurationManager.AppSettings.Get("IsAngular"), "True", StringComparison.OrdinalIgnoreCase) ? false : true;
					//if(iSync)
					//{ ngWebDriver.IgnoreSynchronization = iSync; } // set IgnoreSynchronization = true if the application is NOT Anguler
					//else { }
					break;
				default:
                    webDriver = new EdgeDriver();
                    webDriver.Manage().Window.Maximize();
                    ngWebDriver = new NgWebDriver(webDriver);
                    break;
			}
            lock (DictLock)
            {
                WedDriverDict.Add(fixture, ngWebDriver);
            }
            LogHelper.Log(LogHelper.LEVEL.INFO, null, "WebDriverHelper.InstantiateWebDriver() BrowserType = '{0}': for feature '{1}'", ConfigurationManager.AppSettings.Get("BrowserType"), fixture);
		}
		public static IWebDriver GetCurrentWebDriver()
		{
			lock (DictLock)
			{
				if (!WedDriverDict.ContainsKey(GetTestFixtureName()))
				{
					InstantiateWebDriver();
				}
				LogHelper.Log(LogHelper.LEVEL.DEBUG, null, "WebDriverHelper.GetCurrentWebDriver()");
				return WedDriverDict[GetTestFixtureName()];
			}
		}
		public static void QuitCurrentWebDriver()
		{
			lock (DictLock)
			{
				string fixture = GetTestFixtureName();
				if (WedDriverDict.ContainsKey(fixture))
				{
					WedDriverDict[fixture].Quit();
					WedDriverDict.Remove(fixture);
				}
			}
			LogHelper.Clear();
		}
		public static void DeleteAllCookies()
		{
			lock (DictLock)
			{
				LogHelper.Log(LogHelper.LEVEL.INFO, null, "WebDriverHelper.DeleteAllCookies()");
				GetCurrentWebDriver().Manage().Cookies.DeleteAllCookies();
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
				currentUrl = currentUrl + Constants.CurrentPage.ToLower();
			}

			lock (Constants.PageDictLock)
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

				if (Constants.PAGE_OBJECT_URLS.ContainsKey(currentUrl))
				{
					return Constants.PAGE_OBJECT_URLS[currentUrl];
				}
				else
				{
					// We truncate the last character until we find the closest match or we match the BaseURL
					while (!String.Equals(currentUrl, ConfigurationManager.AppSettings.Get("BaseURL")) && currentUrl.Length > 1)
					{
						if (currentUrl.Equals("https://d1.fisintegratedpayables.com/fis/customerlogin.aspx"))//For loading login page's Page objects on set password page
						{
							currentUrl = "https://10.238.233.179/fis/customerlogin.aspx?";
						}
						else if (currentUrl.Contains("pr.fisintegratedpayables"))
						{
							currentUrl = currentUrl.Replace("pr.fisintegratedpayables.com", "10.238.233.179");
							currentUrl = currentUrl + "?";
						}
						currentUrl = currentUrl.Substring(0, currentUrl.Length - 1);
						if (Constants.PAGE_OBJECT_URLS.ContainsKey(currentUrl))
						{
							return Constants.PAGE_OBJECT_URLS[currentUrl];
						}
					}
				}
			}
			return String.Empty;
		}
		public static void AddPageObject(string PageUrl, string PageObject, bool isCustomerSite = false)
		{
			lock (Constants.PageDictLock)
			{
				if (!Constants.PAGE_OBJECT_URLS.ContainsKey((ConfigurationManager.AppSettings.Get("BaseURL") + PageUrl).ToLower()))
				{
					LogHelper.Log(LogHelper.LEVEL.DEBUG, null, "WebDriverHelper.AddPageObject(PageUrl = '{0}', PageObject = '{1}', isCustomerSite = '{2}') BaseURL = '{3}': added page to PAGE_OBJECT_URLS", PageUrl, PageObject, isCustomerSite.ToString(), ConfigurationManager.AppSettings.Get("BaseURL"));
					Constants.PAGE_OBJECT_URLS.Add((ConfigurationManager.AppSettings.Get("BaseURL") + PageUrl).ToLower(), PageObject);
				}
			}
		}
    }
}
