using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using NUnit.Framework;
using OpenQA.Selenium;
using TechTalk.SpecFlow;

namespace Tsukaeru
{
	public static class LogHelper
	{
		public enum LEVEL { ERROR, WARN, DEBUG, INFO, FATAL };
		private static Dictionary<string, List<String>> LoggerDict = new Dictionary<string, List<String>>();
		private static object DictLock = new object(); // Used to coordinate thread access of WedDriverDict
		public static void InstantiateLogger()
		{
			lock (DictLock)
			{
				LoggerDict.Add(WebDriverHelper.GetTestFixtureName(), new List<String>());
			}
		}
		public static void Log(LEVEL level, Type callerObject, string format, params object[] args)
		{
			lock (DictLock)
			{
				if (!LoggerDict.ContainsKey(WebDriverHelper.GetTestFixtureName()))
				{
					InstantiateLogger();
				}
				string timeStampedEntry = DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss.fff tt") + " [" + level.ToString() + "] ";
				timeStampedEntry = timeStampedEntry.PadRight(35, ' '); // Alignment
				if (callerObject != null)
				{
					timeStampedEntry += "- " + callerObject.Name + "." + string.Format(format, args);
				}
				else
				{
					timeStampedEntry += "- " + string.Format(format, args);
				}

				// We insert a new line in between scenarios except the first
				if(string.Format(format, args).Contains(WebDriverHelper.GetTestScenarioName()) && LoggerDict[WebDriverHelper.GetTestFixtureName()].Count != 0)
				{
					InsertNewLine();
				}

				switch (level)
				{
					case LEVEL.ERROR:
						LoggerDict[WebDriverHelper.GetTestFixtureName()].Add(timeStampedEntry);
						if (!WebDriverHelper.GetTestAssemblyName().Equals("API"))
						{
							CaptureScreen();
						}
						string file = Constants.LOGS_DIRECTORY + WebDriverHelper.GetTestFixtureName() + ".log";
						FileHelper.WriteListToFile(file, LoggerDict[WebDriverHelper.GetTestFixtureName()], true);
						Assert.Fail(string.Format(format, args));
						break;

					case LEVEL.WARN:
						LoggerDict[WebDriverHelper.GetTestFixtureName()].Add(timeStampedEntry);
						break;

					case LEVEL.DEBUG:
						if (true)
						{
							LoggerDict[WebDriverHelper.GetTestFixtureName()].Add(timeStampedEntry);
						}
						break;

					case LEVEL.INFO:
						LoggerDict[WebDriverHelper.GetTestFixtureName()].Add(timeStampedEntry);
						break;

					case LEVEL.FATAL:
						throw new Exception(timeStampedEntry);

					default:
						break;
				}
			}
		}
		public static void Clear()
		{
			lock (DictLock)
			{
				LoggerDict.Remove(WebDriverHelper.GetTestFixtureName());
			}
		}
		public static void InsertNewLine()
		{
			lock (DictLock)
			{
				LoggerDict[WebDriverHelper.GetTestFixtureName()].Add(System.Environment.NewLine);
			}
		}
		public static void IsTrue(bool condition, Type callerObject, string format, params object[] args)
		{
			if(condition)
			{
				Log(LEVEL.INFO, callerObject, format, args);
			}
			else
			{
				Log(LEVEL.ERROR, callerObject, format, args);
			}
		}
		public static void IsFalse(bool condition, Type callerObject, string format, params object[] args)
		{
			if (!condition)
			{
				Log(LEVEL.INFO, callerObject, format, args);
			}
			else
			{
				Log(LEVEL.ERROR, callerObject, format, args);
			}
		}
		public static void LogTable(Table table)
		{
			if (table.Rows.Count != 0)
			{
				// Iterate through headers and rows once to determine the max lengths for the columns
				int[] maxLength = new int[table.Rows[0].Keys.Count];
				int iter = 0;
				foreach (string header in table.Rows[0].Keys)
				{
					maxLength[iter] = header.Length + 2; // Include whitespace
					for (int rowIndex = 0; rowIndex < table.Rows.Count; rowIndex++)
					{
						string reparsedString = table.Rows[rowIndex][iter];
						FileHelper.ReparseNewlineChars(ref reparsedString);
						if (maxLength[iter] < (reparsedString.Length + 2)) // Include whitespace
						{
							maxLength[iter] = reparsedString.Length + 2;
						}
					}
					iter++;
				}
				// Log headers
				string headerString = "|";
				iter = 0;
				foreach (string header in table.Rows[0].Keys)
				{
					string reparsedString = " " + header;
					reparsedString = reparsedString.PadRight(maxLength[iter], ' ');
					headerString += reparsedString + "|";
					iter++;
				}
				Log(LEVEL.INFO, null, headerString);
				// Log rows
				for (int rowIndex = 0; rowIndex < table.Rows.Count; rowIndex++)
				{
					string rowString = "|";
					iter = 0;
					for (int colIndex = 0; colIndex < table.Rows[rowIndex].Count; colIndex++)
					{
						string reparsedString = " " + table.Rows[rowIndex][colIndex].ToString();
						FileHelper.ReparseNewlineChars(ref reparsedString);
						reparsedString = reparsedString.PadRight(maxLength[iter], ' ');
						rowString += reparsedString + "|";
						iter++;
					}
					Log(LEVEL.INFO, null, rowString);
				}
			}
		}
		public static void CaptureScreen()
		{
			Exception commandException = new Exception("Failed capturing screenshot.");
			try
			{
				string file = Constants.LOGS_DIRECTORY + WebDriverHelper.GetTestScenarioName() + ".jpg";
				Screenshot ss = ((ITakesScreenshot)WebDriverHelper.GetCurrentWebDriver()).GetScreenshot();
				ss.SaveAsFile(file);
				//var screen = WebDriverHelper.GetCurrentWebDriver().TakeScreenshot(new VerticalCombineDecorator(new ScreenshotMaker()));
				//System.IO.MemoryStream stream = new System.IO.MemoryStream(screen);
				//System.Drawing.Image myImage = System.Drawing.Image.FromStream(stream);
				//myImage.Save(file, System.Drawing.Imaging.ImageFormat.Jpeg);

			}
			catch (Exception e)
			{
				commandException = e;
			}
		}
		public static void CaptureScreen(string label)
		{
			Exception commandException = new Exception("Failed capturing screenshot.");
			try
			{
				string file = Constants.LOGS_DIRECTORY + WebDriverHelper.GetTestScenarioName() + " " + label + ".jpg";
				Screenshot ss = ((ITakesScreenshot)WebDriverHelper.GetCurrentWebDriver()).GetScreenshot();
				ss.SaveAsFile(file);
				//var screen = WebDriverHelper.GetCurrentWebDriver().TakeScreenshot(new VerticalCombineDecorator(new ScreenshotMaker()));
				//System.IO.MemoryStream stream = new System.IO.MemoryStream(screen);
				//System.Drawing.Image myImage = System.Drawing.Image.FromStream(stream);
				//myImage.Save(file, System.Drawing.Imaging.ImageFormat.Jpeg);

			}
			catch (Exception e)
			{
				commandException = e;
			}
		}
	}
}
