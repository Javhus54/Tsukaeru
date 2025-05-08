using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.Vml;
using Tsukaeru.Helpers;

namespace Tsukaeru
{
    class WhatsAppLogin : BasePage
    {
        public WhatsAppLogin()
        {
            PageTitle = "WhatsApp";
            PageUrl = "https://web.whatsapp.com/";
            XPathValidator = "//div[text()='Log into WhatsApp Web']";
            WebDriverHelper.AddPageObject(PageUrl, this.ToString());
        }
        #region PageElements
        private BaseElement loggedInConformation = null;
        private BaseElement contanctSearchBox = null;
        private BaseElement firstSearchResult = null;
        private BaseElement typeAMessage = null;
        private BaseElement sendMessage = null;
        private BaseElement settingsIcon = null;
        private BaseElement logOut = null;
        private BaseElement confirmLogOut = null;

        public BaseElement LoggedInConformation
        {
            get
            {
                return this.loggedInConformation ?? (
                    this.loggedInConformation = new BaseElement(BaseElement.SelectBy.XPath, "//h1[text()='Chats']"));
            }
        }
        public BaseElement ContanctSearchBox
        {
            get
            {
                return this.contanctSearchBox ?? (
                    this.contanctSearchBox = new BaseElement(BaseElement.SelectBy.XPath, "//div[@id='side']//p"));
            }
        }
        public BaseElement FirstSearchResult
        {
            get
            {
                return this.firstSearchResult ?? (
                    this.firstSearchResult = new BaseElement(BaseElement.SelectBy.XPath, "//div[@aria-label='Search results.']/div[2]"));
            }
        }
        public BaseElement TypeAMessage
        {
            get
            {
                return this.typeAMessage ?? (
                    this.typeAMessage = new BaseElement(BaseElement.SelectBy.XPath, "//div[@aria-label='Type a message']/p"));
            }
        }
        public BaseElement SendMessage
        {
            get
            {
                return this.sendMessage ?? (
                    this.sendMessage = new BaseElement(BaseElement.SelectBy.XPath, "//button[@aria-label='Send']"));
            }
        }
        public BaseElement SettingsIcon
        {
            get
            {
                return this.settingsIcon ?? (
                    this.settingsIcon = new BaseElement(BaseElement.SelectBy.XPath, "//button[@aria-label='Settings']"));
            }
        }
        public BaseElement LogOut
        {
            get
            {
                return this.logOut ?? (
                    this.logOut = new BaseElement(BaseElement.SelectBy.XPath, "//span[text()='Log out']"));
            }
        }
        public BaseElement ConfirmLogOut
        {
            get
            {
                return this.confirmLogOut ?? (
                    this.confirmLogOut = new BaseElement(BaseElement.SelectBy.XPath, "//div[text()='Log out']"));
            }
        }

        #endregion PageElements

        #region PageMethods
        #endregion PageMethods
    } // LoginPage
}//Tsukaeru