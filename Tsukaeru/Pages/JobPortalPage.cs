using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tsukaeru.Helpers;

namespace Tsukaeru.Pages
{
    class JobPortalPage : BasePage
    {
        public JobPortalPage()
        {
            PageTitle = "ExampleJobPortal";
            PageUrl = "https://example.jobsite.com/jobs/";
            XPathValidator = "//span[text()='© 2024 Company Name. All rights reserved. ']";
            WebDriverHelper.AddPageObject(PageUrl, "");
        }
        #region PageElements
        private BaseElement jobTitle = null;
        private BaseElement jobCategory = null;
        private BaseElement jobLocation = null;
        private BaseElement jobAreasOfFirm = null;
        private BaseElement postingDate = null;
        private BaseElement noLongerAvailable = null;
        private BaseElement pageNotFound = null;

        public BaseElement JobTitle
        {
            get
            {
                return this.jobTitle ?? (
                    this.jobTitle = new BaseElement(BaseElement.SelectBy.XPath, "//h1[@class='heading job-details__title']"));
            }
        }
        public BaseElement JobCategory
        {
            get
            {
                return this.jobCategory ?? (
                    this.jobCategory = new BaseElement(BaseElement.SelectBy.XPath, "//span[text()='Job Category']/following-sibling::span"));
            }
        }
        public BaseElement JobLocation
        {
            get
            {
                return this.jobLocation ?? (
                    this.jobLocation = new BaseElement(BaseElement.SelectBy.XPath, "//span[text()='Locations']/following-sibling::span"));
            }
        }
        public BaseElement JobAreasOfFirm
        {
            get
            {
                return this.jobAreasOfFirm ?? (
                    this.jobAreasOfFirm = new BaseElement(BaseElement.SelectBy.XPath, "//span[text()='Areas of the Firm']/following-sibling::span"));
            }
        }
        public BaseElement PostingDate
        {
            get
            {
                return this.postingDate ?? (
                    this.postingDate = new BaseElement(BaseElement.SelectBy.XPath, "//span[text()='Posting Date']/following-sibling::span"));
            }
        }
        public BaseElement NoLongerAvailable
        {
            get
            {
                return this.noLongerAvailable ?? (
                    this.noLongerAvailable = new BaseElement(BaseElement.SelectBy.XPath, "//div[@class='job-details__expired-header error-404__header']"));
            }
        }
        public BaseElement PageNotFound
        {
            get
            {
                return this.pageNotFound ?? (
                    this.pageNotFound = new BaseElement(BaseElement.SelectBy.XPath, "//div[text()='Page not found.']"));
            }
        }
        #endregion PageElements
    }
}
