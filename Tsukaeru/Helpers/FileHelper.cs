using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using System.Xml;
using Tsukaeru.Helpers;

namespace Tsukaeru.Helpers
{
    public static class FileHelper
    {
        public static void UpdateJsonNode(string file, string node, string input)
        {
            try
            {
                if (File.Exists(file))
                {
                    LogHelper.Log(LogHelper.LEVEL.INFO, null, "FileHelper.UpdateJsonNode(file = '{0}', node = '{1}', input = '{2}')", file, node, input);
                    JObject jsonFile = (JObject)JsonConvert.DeserializeObject(File.ReadAllText(file));
                    jsonFile.SelectToken(node).Value<String>(input);
                    File.WriteAllText(file, jsonFile.ToString());
                    return;
                }
                LogHelper.Log(LogHelper.LEVEL.ERROR, null, "FileHelper.UpdateJsonNode(file = '{0}', node = '{1}', input = '{2}'): failed to find file", file, node, input);
            }
            catch (Exception e)
            {
                LogHelper.Log(LogHelper.LEVEL.ERROR, null, "FileHelper.UpdateJsonNode(file = '{0}', node = '{1}', input = '{2}'): failed with exception: '{3}'", file, node, input, e.Message.Substring(0, e.Message.IndexOf('}') + 1));
            }
        }

        public static void UpdateXmlNode(string file, string node, string input)
        {
            try
            {
                if (File.Exists(file))
                {
                    LogHelper.Log(LogHelper.LEVEL.INFO, null, "FileHelper.UpdateXmlNode(file = '{0}', node = '{1}', input = '{2}')", file, node, input);
                    XmlDocument jobXml = new XmlDocument();
                    jobXml.Load(file);
                    XmlNode xmlNode = jobXml.DocumentElement.SelectSingleNode(node);
                    xmlNode.InnerText = input;
                    jobXml.Save(file);
                    return;
                }
                LogHelper.Log(LogHelper.LEVEL.ERROR, null, "FileHelper.UpdateXmlNode(file = '{0}', node = '{1}', input = '{2}'): failed to find file", file, node, input);
            }
            catch (Exception e)
            {
                LogHelper.Log(LogHelper.LEVEL.ERROR, null, "FileHelper.UpdateXmlNode(file = '{0}', node = '{1}', input = '{2}'): failed with exception: '{3}'", file, node, input, e.Message.Substring(0, e.Message.IndexOf('}') + 1));
            }
        }
        public static void RemoveXmlNode(string file, string node)
        {
            try
            {
                if (File.Exists(file))
                {
                    XmlDocument jobXml = new XmlDocument();
                    jobXml.Load(file);
                    XmlNode xmlNode = jobXml.DocumentElement.SelectSingleNode(node);
                    if (xmlNode != null)
                        xmlNode.ParentNode.RemoveChild(xmlNode);
                    jobXml.Save(file);
                    LogHelper.Log(LogHelper.LEVEL.INFO, null, "FileHelper.RemoveXmlNode(file = '{0}', input = '{1}')", file, node);
                    return;
                }
                LogHelper.Log(LogHelper.LEVEL.ERROR, null, "FileHelper.RemoveXmlNode(file = '{0}', node = '{1}', failed to find file", file, node);
            }
            catch (Exception e)
            {
                LogHelper.Log(LogHelper.LEVEL.ERROR, null, "FileHelper.RemoveXmlNode(file = '{0}', node = '{1}', failed with exception: '{2}'", file, node, e.Message.Substring(0, e.Message.IndexOf('}') + 1));
            }
        }
        public static void AppendXmlNode(string file, string value, string newNode, string parentNode)
        {
            try
            {
                if (File.Exists(file))
                {
                    XmlDocument jobXml = new XmlDocument();
                    jobXml.Load(file);
                    XmlNode node = jobXml.CreateNode(XmlNodeType.Element, newNode, "");
                    node.InnerText = value;
                    jobXml.DocumentElement.SelectSingleNode(parentNode).AppendChild(node);
                    jobXml.Save(file);
                    LogHelper.Log(LogHelper.LEVEL.INFO, null, "FileHelper.AppendXmlNode(file = '{0}', value = '{1}', newNode = '{2}, parentNode='{3}' )", file, value, newNode, parentNode);
                    return;
                }
                LogHelper.Log(LogHelper.LEVEL.ERROR, null, "FileHelper.AppendXmlNode(file = '{0}', value = '{1}', newNode = '{2}, parentNode='{3}' , failed to find file", file, value, newNode, parentNode);
            }
            catch (Exception e)
            {
                LogHelper.Log(LogHelper.LEVEL.ERROR, null, "FileHelper.AppendXmlNode(file = '{0}', node = '{1}', failed with exception: '{2}'", file, newNode, e.Message.Substring(0, e.Message.IndexOf('}') + 1));
            }
        }
        public static void WriteListToFile(string file, List<String> input, bool deleteExisting = false)
        {
            if (file.Contains("Logs\\"))
            {
                String logFileName = file.Substring((file.IndexOf("Logs\\") + 5));
                int location = file.IndexOf("Logs\\") + 5;
                if (logFileName.Contains("<") || logFileName.Contains(">") || logFileName.Contains(":") || logFileName.Contains("\"") || logFileName.Contains("\\") || logFileName.Contains("/") || logFileName.Contains("|") || logFileName.Contains("?") || logFileName.Contains("*"))
                {
                    logFileName = logFileName.Replace("<", "_");
                    logFileName = logFileName.Replace(">", "_");
                    logFileName = logFileName.Replace(":", "_");
                    logFileName = logFileName.Replace("\"", "_");
                    logFileName = logFileName.Replace("\\", "_");
                    logFileName = logFileName.Replace("/", "_");
                    logFileName = logFileName.Replace("|", "_");
                    logFileName = logFileName.Replace("?", "_");
                    logFileName = logFileName.Replace("*", "_");
                    file = file.Substring(0, location) + logFileName;
                }
            }
            LogHelper.Log(LogHelper.LEVEL.DEBUG, null, "FileHelper.WriteListToFile(file = '{0}', input = List<String>, deleteExisting = '{1}')", file, deleteExisting.ToString());
            if (File.Exists(file) && deleteExisting)
            {
                File.Delete(file);
            }
            File.WriteAllLines(file, input.ToArray());
        }
        public static string GetFileContents(string file)
        {
            string contents = String.Empty;
            if (File.Exists(file))
            {
                contents = File.ReadAllText(file);
            }
            return contents;
        }
        // Issue with Gerkin parsing of \r\n
        // http://specflow.narkive.com/sq3ePRaJ/changes-to-how-backslash-characters-are-handled-in-tables
        // WorkAround
        public static void ReparseNewlineChars(ref string gherkinString)
        {
            gherkinString = gherkinString.Replace("\\r", "\r");
            gherkinString = gherkinString.Replace("\\n", "\n");
            gherkinString = gherkinString.Replace("\\s", " ");
        }

        /// <summary>
        /// This method replaces the string in a file based on the provided input.
        /// Once all the occurances of regular expression are replaced, it gets overridden with the new content.
        /// 
        /// <para>Following is a sample regular expression</para> 
        /// <para>input= E5555287.123990.234234 </para> 
        /// <para>pattern = E[0-9]{7} - This means followed by seven digits</para> 
        /// <para>replacement = E9999999</para> 
        /// <para>output = E9999999.123990.234234 </para> 
        /// <see cref="https://docs.microsoft.com/en-us/dotnet/standard/base-types/regular-expression-language-quick-reference"/>
        /// </summary>
        /// <param name="inputFile">The file in which the regular expression needs to be replaced</param>
        /// <param name="pattern">The regular expression to be used for replacing</param>
        /// <param name="replacement">The replacement string which should be replaced</param>
        public static void replaceRegexInFile(string inputFile, string pattern, string replacement)
        {
            try
            {
                if (File.Exists(inputFile))
                {
                    string fileLines = File.ReadAllText(inputFile);
                    fileLines = Regex.Replace(fileLines, pattern, replacement);
                    File.WriteAllText(inputFile, fileLines);
                    return;
                }
                LogHelper.Log(LogHelper.LEVEL.ERROR, null, "FileHelper.replaceRegexInFile(file = '{0}', value = '{1}', newNode = '{2}, parentNode='{3}' , failed to find file", inputFile, pattern, replacement);
            }
            catch (Exception e)
            {
                LogHelper.Log(LogHelper.LEVEL.ERROR, null, "FileHelper.replaceRegexInFile(file = '{0}', node = '{1}', failed with exception: '{2}'", inputFile, pattern, e.Message.Substring(0, e.Message.IndexOf('}') + 1));
            }

        }

        public static void removeJsonNode(string file, string jsonPath)
        {
            try
            {
                if (File.Exists(file))

                {
                    LogHelper.Log(LogHelper.LEVEL.INFO, null, "FileHelper.UpdateJsonNode(file = '{0}', jsonPath = '{1}')", file, jsonPath);
                    JObject jsonFile = (JObject)JsonConvert.DeserializeObject(File.ReadAllText(file));
                    (jsonFile.SelectToken(jsonPath).Parent as JProperty).Remove();
                    File.WriteAllText(file, jsonFile.ToString());
                    return;
                }
                LogHelper.Log(LogHelper.LEVEL.WARN, null, "FileHelper.UpdateJsonNode(file = '{0}', jsonPath = '{1}'): failed to find file", file, jsonPath);
            }
            catch (Exception e)
            {
                LogHelper.Log(LogHelper.LEVEL.WARN, null, "FileHelper.UpdateJsonNode(file = '{0}', jsonPath = '{1}'): failed with exception: '{3}'", file, jsonPath, e.Message.Substring(0, e.Message.IndexOf('}') + 1));
            }
        }
        public static void replaceRegexInFileAtPostion(string inputFile, string pattern, Int32 replacement1, int position)
        {
            try
            {
                if (File.Exists(inputFile))
                {
                    string replacement = replacement1.ToString();
                    int cnt = 0;
                    replacement = "\"snd_ref\":\"" + replacement + "\",";
                    string fileLines = File.ReadAllText(inputFile);
                    var result = Regex.Replace(fileLines, pattern, m =>
                    {  // Match evaluator
                        cnt++; return cnt == position ? replacement : m.Value;
                    });
                    //Console.WriteLine(result);
                    File.WriteAllText(inputFile, result);
                    return;
                }
                LogHelper.Log(LogHelper.LEVEL.ERROR, null, "FileHelper.replaceRegexInFile(file = '{0}', value = '{1}', newNode = '{2}, parentNode='{3}' , failed to find file", inputFile, pattern);
            }
            catch (Exception e)
            {
                LogHelper.Log(LogHelper.LEVEL.ERROR, null, "FileHelper.replaceRegexInFile(file = '{0}', node = '{1}', failed with exception: '{2}'", inputFile, pattern, e.Message.Substring(0, e.Message.IndexOf('}') + 1));
            }

        }
        public static void RenameWithTodaysDate(string filePath, string filename, int waitTime = 60)
        {
            if (File.Exists(filePath + filename))
            {
                string[] fileName = filename.Split('.');
                int i;
                string reName;
                for (reName = $"{fileName[0]}_{DateTime.Now:ddMMMyyyy}.{fileName[1]}", i = 0; File.Exists(filePath + reName); i++, reName = $"{fileName[0]}_{DateTime.Now:ddMMMyyyy}_{i}.{fileName[1]}") ;
                File.Move(filePath + filename, destFileName: filePath + reName);
                LogHelper.Log(LogHelper.LEVEL.INFO, null, $"Succesfull rename{filePath + filename} TO {filePath + reName}");
            }
            else
            {
                if (waitTime == 0)
                    LogHelper.Log(LogHelper.LEVEL.ERROR, null, $"File Dosent exist {filePath + filename}");
                else
                {
                    Thread.Sleep(2000);
                    RenameWithTodaysDate(filePath, filename, waitTime - 2);
                }
            }
        }
    }// FileHelper
} // NSpector.Core