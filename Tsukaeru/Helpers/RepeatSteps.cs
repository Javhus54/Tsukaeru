using System;
using System.IO;
using System.Reflection;
using System.Runtime.Remoting;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using Tsukaeru;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using NUnit.Framework;
using System.Globalization;
using System.Threading;
using System.Data;
//using System.Windows.Forms;
//using AutoIt;
using System.Configuration;
//using OpenDialogWindowHandler;

namespace Tsukaeru.Helpers
{
    public class RepeatSteps
    {
        public Object InvokeElementMethod(string elementName, string methodName, object[] parameters)
        {
            // Retrieve the current page that the user is on
            string pageObject = WebDriverHelper.GetCurrentPageObject(Assembly.GetExecutingAssembly());
            if (pageObject.Equals(String.Empty))
            {
                LogHelper.Log(LogHelper.LEVEL.ERROR, this.GetType(), "InvokeElementMethod(elementName = '{0}', methodName = '{1}', parameters = object[])a: failed to retrieve PageObject for url '{2}'", elementName, methodName, WebDriverHelper.GetCurrentWebDriver().Url.ToString());
            }
            ObjectHandle handle = Activator.CreateInstance(Assembly.GetExecutingAssembly().FullName, pageObject);
            Object page = handle.Unwrap();

            // Retrieve the element based on p0
            Type type = page.GetType();
            PropertyInfo property = null;
            DateTime cutoffTime = DateTime.Now.AddSeconds(Defaults.FIND_ELEMENT_TIMEOUT);
            while (property == null && cutoffTime > DateTime.Now) // Continue trying to GetProperty() until cutoffTime is hit
            {
                property = type.GetProperty(elementName);
            }
            if (property == null)
            {
                LogHelper.Log(LogHelper.LEVEL.ERROR, this.GetType(), "InvokeElementMethod(elementName = '{0}', methodName = '{1}', parameters = object[]): failed to retrieve property on PageObject '{2}'", elementName, methodName, pageObject);
                return null;
            }
            cutoffTime = DateTime.Now.AddSeconds(Defaults.FIND_ELEMENT_TIMEOUT);
            Object element = null;
            while (element == null && cutoffTime > DateTime.Now) // Continue trying to GetValue() until cutoffTime is hit
            {
                element = property.GetValue(page, null);
            }
            if (element == null)
            {
                LogHelper.Log(LogHelper.LEVEL.WARN, this.GetType(), "InvokeElementMethod(elementName = '{0}', methodName = '{1}', parameters = object[]): failed to retrieve element on PageObject '{2}'", elementName, methodName, pageObject);
                return null;
            }
            type = element.GetType();

            // Invoke the method
            MethodInfo mInfo;
            if (parameters != null) // Workaround for overloaded methods
            {
                Type[] parTypes = new Type[parameters.Length];
                for (int pIndex = 0; pIndex < parameters.Length; pIndex++)
                {
                    parTypes[pIndex] = parameters[pIndex].GetType();
                }
                mInfo = type.GetMethod(methodName, parTypes);
            }
            else
            {
                mInfo = type.GetMethod(methodName);
            }
            //For taking screen shots when unhandeled exception is thrown by the invoked element method
            try
            {
                return mInfo.Invoke(element, parameters);
            }
            catch (Exception e)
            {
                LogHelper.CaptureScreen("Element Exception");
                throw e;
            }
        }
        public Object InvokePageMethod(string methodName, object[] parameters)
        {
            // Retrieve the current page that the user is on
            string pageObject = WebDriverHelper.GetCurrentPageObject(Assembly.GetExecutingAssembly());
            if (pageObject.Equals(String.Empty))
            {
                LogHelper.Log(LogHelper.LEVEL.ERROR, this.GetType(), "InvokePageMethod(methodName = '{0}', parameters = object[]): failed to retrieve PageObject for url '{1}'", methodName, WebDriverHelper.GetCurrentWebDriver().Url.ToString());
            }
            ObjectHandle handle = Activator.CreateInstance(Assembly.GetExecutingAssembly().FullName, pageObject);
            Object page = handle.Unwrap();
            Type type = page.GetType();

            // Invoke the method
            MethodInfo mInfo;
            if (parameters != null) // Workaround for overloaded methods
            {
                Type[] parTypes = new Type[parameters.Length];
                for (int pIndex = 0; pIndex < parameters.Length; pIndex++)
                {
                    parTypes[pIndex] = parameters[pIndex].GetType();
                }
                mInfo = type.GetMethod(methodName, parTypes);
            }
            else
            {
                mInfo = type.GetMethod(methodName);
            }
            if (mInfo == null)
            {
                LogHelper.Log(LogHelper.LEVEL.ERROR, this.GetType(), "InvokePageMethod(methodName = '{0}', parameters = object[]): failed to retrieve element on PageObject '{1}'", methodName, pageObject);
                return null;
            }
            //For taking screen shots when unhandeled exception is thrown by the invoked page method
            try
            {
                return mInfo.Invoke(page, parameters);
            }
            catch (Exception e)
            {
                LogHelper.CaptureScreen("Page Exception");
                throw e;
            }
        }
        public void ClickElement(string element)
        {
            Object[] parameters = { 0 };
            InvokeElementMethod(element, "Click", parameters);
        }
    }//RepeatedSteps
}//Tsukaeru.Helpers
