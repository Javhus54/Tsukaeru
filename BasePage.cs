using System;
using System.Configuration;
using OpenQA.Selenium;

namespace Tsukaeru
{
	public abstract class BasePage
	{
		public enum SelectBy { ClassName, CssSelector, Id, LinkText, Name, PartialLinkText, TagName, XPath, Binding, Model, Repeater, SelectedOption };
		protected string PageTitle = string.Empty;
		protected string PageUrl = string.Empty;
		protected string XPathValidator = string.Empty;
		public BasePage()
		{
		}
		// Open this page directly
		public void Open(bool expectToOpen = true, string urlParams = "")
		{
			if (string.Equals(ConfigurationManager.AppSettings.Get("IsAngular"), "True", StringComparison.OrdinalIgnoreCase))
			{
				WebDriverFactory.GetCurrentWebDriver().Navigate().GoToUrl(ConfigurationManager.AppSettings.Get("BaseURL") + urlParams);
				if (expectToOpen && !isOpen())
				{
					this.logout();
					WebDriverFactory.GetCurrentWebDriver().Navigate().GoToUrl(ConfigurationManager.AppSettings.Get("BaseURL") + urlParams);
					if (expectToOpen && !isOpen())
					{
						LogHelper.Log(LogHelper.LEVEL.ERROR, this.GetType(), "For Anguler Application Open(expectToOpen = '{0}', urlParams = '{1}') PageTitle = '{2}', PageUrl = '{3}', XPathValidator = '{4}': failed to open page", expectToOpen.ToString(), urlParams.ToString(), PageTitle, ConfigurationManager.AppSettings.Get("BaseURL") + PageUrl, XPathValidator);
					}

				}
			}
			else
			{
				WebDriverFactory.GetCurrentWebDriver().Navigate().GoToUrl(ConfigurationManager.AppSettings.Get("BaseURL") + PageUrl + urlParams);
				System.Threading.Thread.Sleep(2 * 1000);
				if (expectToOpen && !isOpen())
				{
					this.logout();
					WebDriverFactory.GetCurrentWebDriver().Navigate().GoToUrl(ConfigurationManager.AppSettings.Get("BaseURL") + PageUrl + urlParams);
					if (expectToOpen && !isOpen())
					{
						LogHelper.Log(LogHelper.LEVEL.ERROR, this.GetType(), "Open(expectToOpen = '{0}', urlParams = '{1}') PageTitle = '{2}', PageUrl = '{3}', XPathValidator = '{4}': failed to open page", expectToOpen.ToString(), urlParams.ToString(), PageTitle, ConfigurationManager.AppSettings.Get("BaseURL") + PageUrl, XPathValidator);
					}

				}
			}
		}
		// Validate that this page is open based on retrieval of IWebElement
		public bool isOpen(double timeoutInSeconds = Constants.PAGE_ISOPEN_DEFAULT_TIMEOUT)
		{
			// Poll every 100ms until timeout is reached
			double timerCount = 0;
			IWebDriver driver = WebDriverFactory.GetCurrentWebDriver();
			var javaScriptExecutor = driver as IJavaScriptExecutor;
			while (timerCount <= timeoutInSeconds)
			{
				if (javaScriptExecutor.ExecuteScript("return document.readyState;").Equals("complete"))
				{
					try
					{
						if (WebDriverFactory.GetCurrentWebDriver().FindElement(By.XPath(XPathValidator)) != null)
						{
							// Finally validate that complete Url has expected PageUrl
							if (WebDriverFactory.GetCurrentWebDriver().Url.Contains("localhost"))
							{
								LogHelper.Log(LogHelper.LEVEL.INFO, this.GetType(), "REWRITE isOpen(timeoutInSeconds = '{1}') XPathValidator = '{2}', timerCount = '{3}': returned 'True'", this.ToString(), timeoutInSeconds.ToString(), XPathValidator, timerCount);
								return true;
							}
							if (WebDriverFactory.GetCurrentWebDriver().Url.IndexOf(PageUrl, StringComparison.OrdinalIgnoreCase) >= 0)
							{
								LogHelper.Log(LogHelper.LEVEL.INFO, this.GetType(), "isOpen(timeoutInSeconds = '{1}') XPathValidator = '{2}', timerCount = '{3}': returned 'True'", this.ToString(), timeoutInSeconds.ToString(), XPathValidator, timerCount);
								return true;
							}
						}
					}
					catch (Exception e)
					{
						// We log at DEBUG level and truncate the Exception message to prevent spamming the log
						LogHelper.Log(LogHelper.LEVEL.DEBUG, this.GetType(), "isOpen() timerCount = '{0}': failed with exception: '{1}'", timerCount.ToString(), e.Message.Substring(0, e.Message.IndexOf('}') + 1));
					}
					timerCount += 0.1;
					System.Threading.Thread.Sleep(100);
				}
			}
			LogHelper.Log(LogHelper.LEVEL.INFO, this.GetType(), "isOpen(timeoutInSeconds = '{0}') XPathValidator = '{1}', timerCount = '{2}': returned 'False'", timeoutInSeconds.ToString(), XPathValidator, timerCount);
			return false;
		}
		// Retrieve the html source of the page
		public string GetPageSource()
		{
			string pageSource = string.Empty;
			if (isOpen())
			{
				pageSource = WebDriverFactory.GetCurrentWebDriver().PageSource;
			}
			LogHelper.Log(LogHelper.LEVEL.INFO, this.GetType(), "GetPageSource() PageTitle = '{0}', PageUrl = '{1}', XPathValidator = '{2}': returned PageSource", PageTitle, PageUrl, XPathValidator);
			return pageSource;
		}
		public virtual void logout()
		{
			try
			{
				IWebDriver driver = WebDriverFactory.GetCurrentWebDriver();
				driver.FindElement(By.XPath("//a[contains(.,'Log Out')] | //a[contains(.,'Logout')] | //a[contains(.,'Sign Out')]")).Click();
			}
			catch (Exception e)
			{
				LogHelper.Log(LogHelper.LEVEL.INFO, this.GetType(), "Error Logging out from {0} with exception {1}", this.PageTitle, e.StackTrace);

			}
		}

		// Navigation flow to this particular webpage as in not opening the page directly
		public virtual void NavigateTo(string username, string password)
		{
			//To be implemented in the respective page classes.
		}
	}
}
