using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.ObjectModel;
using System.Data;
using System.IO;
using Tsukaeru.Helpers;
using Tsukaeru.Pages;

namespace Tsukaeru
{
    public class Tests
    {
        RepeatSteps repeatSteps = new RepeatSteps();
        //private  webDriverWait = null;
        [OneTimeSetUp]
        public void Setup()
        {
            WebDriverHelper.InstantiateWebDriver();
        }

        [Test]
        public void WhatsAppSendMessageTo()
        {
            WhatsAppLogin whatsAppLogin = new WhatsAppLogin();
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

        [Test]
        public void JobHunting()
        {
            //Setup
            JobPortalPage jobPortalPage = new JobPortalPage();
            DataTable dataTable = new DataTable();
            dataTable.Columns.Add("JobID");
            dataTable.Columns.Add("Title");
            dataTable.Columns.Add("Category");
            dataTable.Columns.Add("Location");
            dataTable.Columns.Add("Areas");
            dataTable.Columns.Add("PostingDate");

            //TestStart
            int startingJobID = 146000;
            for (int i = startingJobID; i < startingJobID + 20; i++)
            {
                jobPortalPage.DirectOpen(false, i.ToString());
                try
                {
                    jobPortalPage.NoLongerAvailable.WaitForElement("Clicks", 1);
                }
                catch (Exception)
                {
                    try
                    {
                        jobPortalPage.PageNotFound.WaitForElement("Clicks", 1);
                    }
                    catch (Exception)
                    {
                        try
                        {
                            jobPortalPage.JobTitle.WaitForElement("Clicks", 1);
                            if (jobPortalPage.JobLocation.GetTextByInnerText().Contains("United States"))
                            {
                                string jobName = jobPortalPage.JobTitle.GetTextByInnerText();
                                if (jobName.Contains("Associate") || jobName.Contains("Analyst"))
                                {
                                    string _tempJobId = i.ToString();
                                    string _tempTitle = jobName;
                                    string _tempCategory = "N/A";
                                    string _tempLocation = "N/A";
                                    string _tempAreas = "N/A";
                                    string _tempPostingDates = "N/A";
                                    try
                                    {
                                        _tempCategory = jobPortalPage.JobCategory.GetTextByInnerText();
                                    }
                                    catch (Exception) { }

                                    try
                                    {
                                        _tempLocation = jobPortalPage.JobLocation.GetTextByInnerText();
                                    }
                                    catch (Exception) { }
                                    try
                                    {
                                        _tempAreas = jobPortalPage.JobAreasOfFirm.GetTextByInnerText();
                                    }
                                    catch (Exception) { }
                                    try
                                    {
                                        _tempPostingDates = jobPortalPage.PostingDate.GetTextByInnerText();
                                    }
                                    catch (Exception) { }

                                    dataTable.Rows.Add(_tempJobId, _tempTitle, _tempCategory, _tempLocation, _tempAreas, _tempPostingDates);
                                }
                            }
                        } catch (Exception) { }
                    }
                }
            }
            DataSet dataSet = new DataSet();
            dataSet.Tables.Add(dataTable);
            ExcelHelper.CreateExcelFile(dataSet, "JobHunting.xlsx");
        }
        [OneTimeTearDown]
        public void TearDown()
        {
            WebDriverHelper.QuitCurrentWebDriver();
        }
    }//Tests
}//SeleniumCsharp