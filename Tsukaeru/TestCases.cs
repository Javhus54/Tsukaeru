using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.ObjectModel;
using System.IO;
using Tsukaeru.Helpers;

namespace Tsukaeru
{
    public class Tests
    {
        RepeatSteps repeatSteps = new RepeatSteps();
        WhatsAppLogin whatsAppLogin = new WhatsAppLogin();
        //private  webDriverWait = null;
        [OneTimeSetUp]
        public void Setup()
        {
            WebDriverHelper.InstantiateWebDriver();
        }

        [Test]
        public void WhatsAppSendMessageTo()
        {
            string friendName = "Javed";
            whatsAppLogin.Open();
            whatsAppLogin.LoggedInConformation.WaitForElement("Displayed", 60);
            whatsAppLogin.ContanctSearchBox.InputText(friendName);
            Thread.Sleep(100);
            whatsAppLogin.FirstSearchResult.WaitForElement("Clicks", 60);
            whatsAppLogin.FirstSearchResult.Click();
            for (int i = 0; i < 10; i++)
            {
                whatsAppLogin.TypeAMessage.WaitForElement("Clicks", 60);
                whatsAppLogin.TypeAMessage.InputText("Automated Message");
                whatsAppLogin.TypeAMessage.InputText(Keys.Enter);
            }
            whatsAppLogin.SettingsIcon.WaitForElement("Clicks", 60);
            whatsAppLogin.SettingsIcon.Click();
            whatsAppLogin.LogOut.WaitForElement("Clicks", 60);
            whatsAppLogin.LogOut.Click();
            whatsAppLogin.ConfirmLogOut.WaitForElement("Clicks", 60);
            whatsAppLogin.ConfirmLogOut.Click();
            whatsAppLogin.IsOpen();
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            WebDriverHelper.QuitCurrentWebDriver();
        }
    }//Tests
}//SeleniumCsharp