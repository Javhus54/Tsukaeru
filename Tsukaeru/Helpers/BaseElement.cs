using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using SeleniumExtras.WaitHelpers;
using NUnit.Framework;
using OpenQA.Selenium.Support.UI;
using System.Collections.Generic;
using DocumentFormat.OpenXml.Bibliography;
using Tsukaeru.Helpers;
using Tsukaeru;
using DocumentFormat.OpenXml.Drawing;

namespace Tsukaeru.Helpers
{
    public class BaseElement
    {
        public enum SelectBy { ClassName, CssSelector, Id, LinkText, Name, PartialLinkText, TagName, XPath, Binding, Model, Repeater, SelectedOption };
        private SelectBy sourceBy;
        private string sourceStr;

        public BaseElement(SelectBy selectBy, string strToFind)
        {
            sourceBy = selectBy;
            sourceStr = strToFind;
        }
        protected SelectBy SourceBy
        {
            get
            {
                return sourceBy;
            }
            set
            {
                sourceBy = value;
            }
        }
        protected string SourceStr
        {
            get
            {
                return sourceStr;
            }
            set
            {
                sourceStr = value;
            }
        }
        protected IWebElement FindElement(double timeoutSeconds = Defaults.FIND_ELEMENT_TIMEOUT)
        {
            DateTime cutoffTime = DateTime.Now.AddSeconds(timeoutSeconds);
            IWebElement element = null;
            if (SourceStr != null && WebDriverHelper.GetCurrentWebDriver() != null)
            {
                while (element == null && cutoffTime > DateTime.Now) // Continue trying to FindElement() until cutoffTime is hit
                {
                    try
                    {
                        switch (sourceBy)
                        {
                            case SelectBy.ClassName:
                                element = WebDriverHelper.GetCurrentWebDriver().FindElement(By.ClassName(SourceStr));
                                break;

                            case SelectBy.CssSelector:
                                element = WebDriverHelper.GetCurrentWebDriver().FindElement(By.CssSelector(SourceStr));
                                break;

                            case SelectBy.LinkText:
                                element = WebDriverHelper.GetCurrentWebDriver().FindElement(By.LinkText(SourceStr));
                                break;

                            case SelectBy.Name:
                                element = WebDriverHelper.GetCurrentWebDriver().FindElement(By.Name(SourceStr));
                                break;

                            case SelectBy.PartialLinkText:
                                element = WebDriverHelper.GetCurrentWebDriver().FindElement(By.PartialLinkText(SourceStr));
                                break;

                            case SelectBy.TagName:
                                element = WebDriverHelper.GetCurrentWebDriver().FindElement(By.TagName(SourceStr));
                                break;

                            case SelectBy.XPath:
                                element = WebDriverHelper.GetCurrentWebDriver().FindElement(By.XPath(SourceStr));
                                break;

                            case SelectBy.Id:
                                element = WebDriverHelper.GetCurrentWebDriver().FindElement(By.Id(SourceStr));
                                break;
                            default:
                                // DO NOTHING
                                break;
                        }
                    }
                    catch (Exception e)
                    {
                        LogHelper.Log(LogHelper.LEVEL.DEBUG, this.GetType(), "FindElement(timeoutSeconds = '{0}') SourceBy = '{1}', SourceStr = '{2}': failed with exception: '{3}'", timeoutSeconds.ToString(), SourceBy.ToString(), SourceStr, e.Message.Substring(0, e.Message.IndexOf('}') + 1));
                    }
                }
            }
            if (cutoffTime < DateTime.Now)
            {
                LogHelper.Log(LogHelper.LEVEL.WARN, this.GetType(), "FindElement(timeoutSeconds = '{0}') SourceBy = '{1}', SourceStr = '{2}': failed to find element and timed out.", timeoutSeconds.ToString(), SourceBy.ToString(), SourceStr);
            }
            else if (element != null)
            {
                LogHelper.Log(LogHelper.LEVEL.DEBUG, this.GetType(), "FindElement(timeoutSeconds = '{0}') SourceBy = '{1}', SourceStr = '{2}': found element.", timeoutSeconds.ToString(), SourceBy.ToString(), SourceStr);
            }
            return element;
        }

        public void WaitForElement(string type = "Displayed", int wait = 60)
        {
            var webDriverWait = new WebDriverWait(WebDriverHelper.GetCurrentWebDriver(), new TimeSpan(0, 0, wait));
            switch (type)
            {
                case "Clicks":
                    try
                    {
                        webDriverWait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(By.XPath(SourceStr)));
                    }
                    catch (NoSuchElementException)
                    {
                        Console.WriteLine("Element Not Found");
                    }
                    break;
                case "Selects":
                    try
                    {
                        webDriverWait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeSelected(By.XPath(SourceStr)));
                    }
                    catch (NoSuchElementException)
                    {
                        Console.WriteLine("Element Not Found");
                    }
                    break;
                default:
                    try
                    {
                        webDriverWait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.XPath(SourceStr)));
                    }

                    catch (NoSuchElementException)
                    {
                        Console.WriteLine("Element Not Found");
                    }
                    break;
            }
        }
        public string GetTextByAttribute(string attribute = "value")
        {
            string output = "";
            IWebElement element = FindElement(Defaults.ELEMENT_GET_TEXT_BY_ATTRIBUTE_TIEMOUT);
            if (element != null)
            {
                try
                {
                    output = element.GetAttribute(attribute);
                }
                catch (Exception)
                {
                    LogHelper.Log(LogHelper.LEVEL.WARN, this.GetType(), "No attribute found for element {0} attribute {1}", element, attribute);
                }
            }
            LogHelper.Log(LogHelper.LEVEL.INFO, this.GetType(), "GetTextByAttribute(attribute = '{0}'): returned '{1}'", attribute, output);
            return output;
        }
        public string GetTextByInnerText()
        {
            string output = "";
            IWebElement element = FindElement(Defaults.ELEMENT_GET_TEXT_BY_INNER_TEXT_TIEMOUT);
            if (element != null)
            {
                output = element.Text;
            }
            LogHelper.Log(LogHelper.LEVEL.INFO, this.GetType(), "GetTextByInnerText(): returned '{0}'", output);
            return output;
        }
        public void ContainsInnerText(string validation, bool expectedResult, int staleAttempts = 0)
        {
            IWebElement element = FindElement(Defaults.ELEMENT_CONTAINS_INNER_TEXT_TIMEOUT);
            if (element != null)
            {
                try
                {
                    bool result = element.Text.Contains(validation);
                    LogHelper.IsTrue(result == expectedResult, this.GetType(), "ContainsInnerText(validation = '{0}', expectedResult = '{1}', staleAttempts = '{2}'): validation against innertext '{3}'", validation, expectedResult.ToString(), staleAttempts, element.Text);
                }
                catch (StaleElementReferenceException)
                {
                    if (staleAttempts < Defaults.STALE_MAX_ATTEMPTS)
                    {
                        ContainsInnerText(validation, expectedResult, staleAttempts++);
                    }
                }
            }
            else
            {
                LogHelper.Log(LogHelper.LEVEL.ERROR, this.GetType(), "ContainsInnerText(validation = '{0}', expectedResult = '{1}', staleAttempts = '{2}'): failed.", validation, expectedResult.ToString(), staleAttempts);
            }
        }

        public void ContainsInnerTextOnReplace(string validation, string replaceString, bool expectedResult, int staleAttempts = 0)
        {
            IWebElement element = FindElement(Defaults.ELEMENT_CONTAINS_INNER_TEXT_TIMEOUT);
            if (element != null)
            {
                try
                {
                    string replaceCopyRight = element.Text.Replace(replaceString, "");
                    validation = validation.Replace(replaceString, "");
                    bool result = replaceCopyRight.Contains(validation);
                    LogHelper.IsTrue(result == expectedResult, this.GetType(), "ContainsInnerText(validation = '{0}', expectedResult = '{1}', staleAttempts = '{2}'): validation against innertext '{3}'", validation, expectedResult.ToString(), staleAttempts, element.Text);
                }
                catch (StaleElementReferenceException)
                {
                    if (staleAttempts < Defaults.STALE_MAX_ATTEMPTS)
                    {
                        ContainsInnerTextOnReplace(validation, replaceString, expectedResult, staleAttempts++);
                    }
                }
            }
            else
            {
                LogHelper.Log(LogHelper.LEVEL.ERROR, this.GetType(), "ContainsInnerText(validation = '{0}', expectedResult = '{1}', staleAttempts = '{2}'): failed.", validation, expectedResult.ToString(), staleAttempts);
            }
        }
        public void ValidateInnerText(string validation, bool expectedResult, int staleAttempts = 0)
        {
            IWebElement element = FindElement(Defaults.ELEMENT_VALIDATE_INNER_TEXT_TIMEOUT);
            if (element != null)
            {
                try
                {
                    string elementText = element.Text;
                    bool result = String.Equals(validation, elementText);
                    LogHelper.IsTrue(result == expectedResult, this.GetType(), "ValidateInnerText(validation = '{0}', expectedResult = '{1}', staleAttempts = '{2}'): validation against innertext '{3}'", validation, expectedResult.ToString(), staleAttempts, elementText);
                }
                catch (StaleElementReferenceException)
                {
                    if (staleAttempts < Defaults.STALE_MAX_ATTEMPTS)
                    {
                        ValidateInnerText(validation, expectedResult, staleAttempts++);
                    }
                }
            }
            else
            {
                LogHelper.Log(LogHelper.LEVEL.ERROR, this.GetType(), "ValidateInnerText(validation = '{0}', expectedResult = '{1}', staleAttempts = '{2}'): failed.", validation, expectedResult.ToString(), staleAttempts);
            }
        }
        public void ValidateAttributeText(string validation, string attribute, bool expectedResult, int staleAttempts = 0)
        {
            IWebElement element = FindElement(Defaults.ELEMENT_VALIDATE_ATTRIBUTE_TEXT_TIMEOUT);
            if (element != null)
            {
                try
                {
                    string attributeText = element.GetAttribute(attribute);
                    bool result = String.Equals(validation, attributeText);
                    LogHelper.IsTrue(result == expectedResult, this.GetType(), "ValidateAttributeText(validation = '{0}', attribute = '{1}', expectedResult = '{2}', staleAttempts = '{3}'): validation against attribute text '{4}'", validation, attribute, expectedResult.ToString(), staleAttempts, attributeText);
                }
                catch (StaleElementReferenceException)
                {
                    if (staleAttempts < Defaults.STALE_MAX_ATTEMPTS)
                    {
                        ValidateAttributeText(validation, attribute, expectedResult, staleAttempts++);
                    }
                }
            }
            else
            {
                LogHelper.Log(LogHelper.LEVEL.ERROR, this.GetType(), "ValidateAttributeText(validation = '{0}', attribute = '{1}', expectedResult = '{2}', staleAttempts = '{3}'): failed.'", validation, attribute, expectedResult.ToString(), staleAttempts);
            }
        }
        public void ValidateAttributeTextContains(string validation, string attribute, bool expectedResult, int staleAttempts = 0)
        {
            IWebElement element = FindElement(Defaults.ELEMENT_VALIDATE_ATTRIBUTE_TEXT_TIMEOUT);
            if (element != null)
            {
                try
                {
                    string attributeText = element.GetAttribute(attribute);
                    bool result = attributeText.Contains(validation);
                    LogHelper.IsTrue(result == expectedResult, this.GetType(), "ValidateAttributeText(validation = '{0}', attribute = '{1}', expectedResult = '{2}', staleAttempts = '{3}'): validation against attribute text '{4}'", validation, attribute, expectedResult.ToString(), staleAttempts, attributeText);
                }
                catch (StaleElementReferenceException)
                {
                    if (staleAttempts < Defaults.STALE_MAX_ATTEMPTS)
                    {
                        ValidateAttributeTextContains(validation, attribute, expectedResult, staleAttempts++);
                    }
                }
            }
            else
            {
                LogHelper.Log(LogHelper.LEVEL.ERROR, this.GetType(), "ValidateAttributeTextContains(validation = '{0}', attribute = '{1}', expectedResult = '{2}', staleAttempts = '{3}'): failed.'", validation, attribute, expectedResult.ToString(), staleAttempts);
            }
        }
        public bool Displayed(int staleAttempts = 0)
        {
            bool result = false;
            IWebElement element = FindElement(Defaults.ELEMENT_VALIDATE_DISPLAYED_TIMEOUT);
            if (element != null)
            {
                try
                {
                    result = element.Displayed;
                }
                catch (StaleElementReferenceException)
                {
                    if (staleAttempts < Defaults.STALE_MAX_ATTEMPTS)
                    {
                        return Displayed(staleAttempts++);
                    }
                }
            }
            return result;
        }
        public bool Available()
        {
            IWebElement element = FindElement(1);
            if (element != null)
                return true;
            else
                return false;
        }
        public void ValidateDisplayed(bool expectedResult)
        {
            bool result = Displayed();
            LogHelper.IsTrue(result == expectedResult, this.GetType(), "ValidateDisplayed(expectedResult = '{0}'): returned '{1}'", expectedResult.ToString(), result.ToString());
        }
        public bool Enabled(int staleAttempts = 0)
        {
            bool result = false;
            IWebElement element = FindElement(Defaults.ELEMENT_VALIDATE_ENABLED_TIMEOUT);
            if (element != null)
            {
                try
                {
                    result = element.Enabled;
                }
                catch (StaleElementReferenceException)
                {
                    if (staleAttempts < Defaults.STALE_MAX_ATTEMPTS)
                    {
                        return Enabled(staleAttempts++);
                    }
                }
            }
            return result;
        }
        public void ValidateEnabled(bool expectedResult)
        {
            bool result = Enabled();
            LogHelper.IsTrue(result == expectedResult, this.GetType(), "ValidateEnabled(expectedResult = '{0}'): returned '{1}'", expectedResult.ToString(), result.ToString());
        }
        public bool Focused()
        {
            bool result = false;
            IWebElement element = FindElement(Defaults.ELEMENT_VALIDATE_FOCUSED_TIMEOUT);
            if (element != null)
            {
                IWebElement activeElement = WebDriverHelper.GetCurrentWebDriver().SwitchTo().ActiveElement();
                result = element.Equals(activeElement);
            }
            return result;
        }
        public void ValidateFocused(bool expectedResult)
        {
            bool result = Focused();
            LogHelper.IsTrue(result == expectedResult, this.GetType(), "ValidateFocused(expectedResult = '{0}'): returned '{1}'", expectedResult.ToString(), result.ToString());
        }
        public bool Selected(int staleAttempts = 0)
        {
            bool result = false;
            IWebElement element = FindElement(Defaults.ELEMENT_VALIDATE_SELECTED_TIMEOUT);
            if (element != null)
            {
                try
                {
                    result = element.Selected;
                }
                catch (StaleElementReferenceException)
                {
                    if (staleAttempts < Defaults.STALE_MAX_ATTEMPTS)
                    {
                        return Selected(staleAttempts++);
                    }
                }
            }
            return result;
        }
        public void ValidateSelected(bool expectedResult)
        {
            bool result = Selected();
            LogHelper.IsTrue(result == expectedResult, this.GetType(), "ValidateSelected(expectedResult = '{0}'): returned '{1}'", expectedResult.ToString(), result.ToString());
        }
        // This validation used in the situation where we have navigated away from a page, returned to it and the DOM object i.e. IWebElement has become 'stale'
        public bool ValidateStale()
        {
            try
            {
                IWebElement element = FindElement(Defaults.ELEMENT_VALIDATE_STALE_TIMEOUT);
                bool result = element.Enabled;
                LogHelper.Log(LogHelper.LEVEL.INFO, this.GetType(), "ValidateStale(): returned 'False'");
                return false;
            }
            catch (StaleElementReferenceException)
            {
                LogHelper.Log(LogHelper.LEVEL.INFO, this.GetType(), "ValidateStale(): returned 'True'");
                return true;
            }
        }
        public string GetElementId()
        {
            IWebElement element = FindElement(Defaults.ELEMENT_VALIDATE_ENABLED_TIMEOUT);
            string elementId = String.Empty;
            if (element != null)
            {
                elementId = element.GetAttribute("id");
            }
            LogHelper.Log(LogHelper.LEVEL.INFO, this.GetType(), "GetElementId(): returned '{0}'", elementId);
            return elementId;
        }
        public void InputText(string inputText, int staleAttempts = 0, bool clearText = true)
        {
            IWebElement element = FindElement(Defaults.ELEMENT_VALIDATE_SELECTED_TIMEOUT);
            if (element != null)
            {
                try
                {
                    LogHelper.Log(LogHelper.LEVEL.INFO, this.GetType(), "Click(staleAttempts = '{0}') SourceBy = '{1}', SourceStr = '{2}'", staleAttempts, SourceBy.ToString(), SourceStr);
                    element.Click();
                    element.Clear();
                    element.SendKeys(inputText);
                }
                catch (StaleElementReferenceException)
                {
                    if (staleAttempts < Defaults.STALE_MAX_ATTEMPTS)
                    {
                        Click(staleAttempts++);
                    }
                }
            }
            else
            {
                LogHelper.Log(LogHelper.LEVEL.ERROR, this.GetType(), "Click(staleAttempts = '{0}') SourceBy = '{1}', SourceStr = '{2}': failed to find element'", SourceBy.ToString(), SourceStr);
            }
        }
        public void Click(int staleAttempts = 0)
        {
            IWebElement element = FindElement(Defaults.ELEMENT_VALIDATE_SELECTED_TIMEOUT);
            if (element != null)
            {
                try
                {
                    LogHelper.Log(LogHelper.LEVEL.INFO, this.GetType(), "Click(staleAttempts = '{0}') SourceBy = '{1}', SourceStr = '{2}'", staleAttempts, SourceBy.ToString(), SourceStr);
                    element.Click();
                }
                catch (StaleElementReferenceException)
                {
                    if (staleAttempts < Defaults.STALE_MAX_ATTEMPTS)
                    {
                        Click(staleAttempts++);
                    }
                }
            }
            else
            {
                LogHelper.Log(LogHelper.LEVEL.ERROR, this.GetType(), "Click(staleAttempts = '{0}') SourceBy = '{1}', SourceStr = '{2}': failed to find element'", SourceBy.ToString(), SourceStr);
            }
        }
        public void ClickElementPosition(int staleAttempts = 0)
        {
            IWebElement element = FindElement(Defaults.ELEMENT_VALIDATE_SELECTED_TIMEOUT);
            if (element != null)
            {
                try
                {
                    LogHelper.Log(LogHelper.LEVEL.INFO, this.GetType(), "ClickElementPosition(staleAttempts = '{0}') SourceBy = '{1}', SourceStr = '{2}'", staleAttempts, SourceBy.ToString(), SourceStr);
                    Actions actions = new Actions(WebDriverHelper.GetCurrentWebDriver());
                    actions.MoveToElement(element);
                    actions.Click();
                    actions.Perform();
                }
                catch (StaleElementReferenceException)
                {
                    if (staleAttempts < Defaults.STALE_MAX_ATTEMPTS)
                    {
                        ClickElementPosition(staleAttempts++);
                    }
                }
            }
            else
            {
                LogHelper.Log(LogHelper.LEVEL.ERROR, this.GetType(), "ClickElementPosition(staleAttempts = '{0}') SourceBy = '{1}', SourceStr = '{2}': failed to find element'", SourceBy.ToString(), SourceStr);
            }
        }
        public void MoveTo(int staleAttempts = 0, bool view = true)
        {
            IWebElement element = FindElement(Defaults.ELEMENT_VALIDATE_SELECTED_TIMEOUT);
            IJavaScriptExecutor executor = (IJavaScriptExecutor)WebDriverHelper.GetCurrentWebDriver();
            if (element != null)
            {
                try
                {
                    executor.ExecuteScript("arguments[0].scrollIntoView(arguments[1]);", element, view);
                }
                catch
                {
                    if (staleAttempts < Defaults.STALE_MAX_ATTEMPTS)
                    {
                        ClickElementPosition(staleAttempts++);
                    }
                }
            }
            else
            {
                LogHelper.Log(LogHelper.LEVEL.ERROR, this.GetType(), "ClickElementPosition(staleAttempts = '{0}') SourceBy = '{1}', SourceStr = '{2}': failed to find element'", SourceBy.ToString(), SourceStr);
            }
        }
        public void MoveToElement(int staleAttempts = 0)
        {
            IWebElement element = FindElement(Defaults.ELEMENT_VALIDATE_SELECTED_TIMEOUT);
            if (element != null)
            {
                try
                {
                    //LogHelper.Log(LogHelper.LEVEL.INFO, this.GetType(), "ClickElementPosition(staleAttempts = '{0}') SourceBy = '{1}', SourceStr = '{2}'", staleAttempts, SourceBy.ToString(), SourceStr);
                    Actions actions = new Actions(WebDriverHelper.GetCurrentWebDriver());
                    actions.MoveToElement(element);
                    actions.Perform();
                }
                catch (StaleElementReferenceException)
                {
                    if (staleAttempts < Defaults.STALE_MAX_ATTEMPTS)
                    {
                        MoveToElement(staleAttempts++);
                    }
                }
            }
            else
            {
                //LogHelper.Log(LogHelper.LEVEL.ERROR, this.GetType(), "ClickElementPosition(staleAttempts = '{0}') SourceBy = '{1}', SourceStr = '{2}': failed to find element'", SourceBy.ToString(), SourceStr);
            }
        }
        public void Hover()
        {
            IWebElement element = FindElement();
            if (element != null)
            {
                LogHelper.Log(LogHelper.LEVEL.DEBUG, this.GetType(), "Hover() SourceBy = '{0}', SourceStr = '{1}'", SourceBy.ToString(), SourceStr);
                Actions actions = new Actions(WebDriverHelper.GetCurrentWebDriver());
                actions.MoveToElement(element);
                actions.Perform();
                return;
            }
            LogHelper.Log(LogHelper.LEVEL.DEBUG, this.GetType(), "Hover() SourceBy = '{0}', SourceStr = '{1}': failed to find element", SourceBy.ToString(), SourceStr);
        }
        public void ClickAndHold()
        {
            IWebElement element = FindElement();
            if (element != null)
            {
                LogHelper.Log(LogHelper.LEVEL.DEBUG, this.GetType(), "ClickAndHold() SourceBy = '{0}', SourceStr = '{1}'", SourceBy.ToString(), SourceStr);
                Actions actions = new Actions(WebDriverHelper.GetCurrentWebDriver());
                actions.ClickAndHold(element);
                actions.Perform();
                return;
            }
            LogHelper.Log(LogHelper.LEVEL.DEBUG, this.GetType(), "ClickAndHold() SourceBy = '{0}', SourceStr = '{1}': failed to find element", SourceBy.ToString(), SourceStr);
        }
        public void Drop()
        {
            IWebElement element = FindElement();
            if (element != null)
            {
                LogHelper.Log(LogHelper.LEVEL.DEBUG, this.GetType(), "Drop() SourceBy = '{0}', SourceStr = '{1}'", SourceBy.ToString(), SourceStr);
                Actions actions = new Actions(WebDriverHelper.GetCurrentWebDriver());
                actions.MoveToElement(element);
                actions.Perform();
                actions.Release();
                actions.Perform();
                return;
            }
            LogHelper.Log(LogHelper.LEVEL.DEBUG, this.GetType(), "Drop() SourceBy = '{0}', SourceStr = '{1}': failed to find element", SourceBy.ToString(), SourceStr);
        }
    } // BaseObject
} // NSpector.Core