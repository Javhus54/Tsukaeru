using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace Tsukaeru
{
    public static class Defaults
    {
        public const string BrowserType = "Chrome";
        // Timeouts (seconds)
        public const double PAGE_ISOPEN_DEFAULT_TIMEOUT = 4.0;
        public const double FIND_ELEMENT_TIMEOUT = 25.0;
        public const double ELEMENT_GET_TEXT_BY_ATTRIBUTE_TIEMOUT = 0.1;
        public const double ELEMENT_GET_TEXT_BY_INNER_TEXT_TIEMOUT = 0.1;
        public const double ELEMENT_CONTAINS_INNER_TEXT_TIMEOUT = 2.0;
        public const double ELEMENT_VALIDATE_INNER_TEXT_TIMEOUT = 2.0;
        public const double ELEMENT_VALIDATE_ATTRIBUTE_TEXT_TIMEOUT = 0.1;
        public const double ELEMENT_VALIDATE_ENABLED_TIMEOUT = 1.0;
        public const double ELEMENT_VALIDATE_DISPLAYED_TIMEOUT = 3.0;
        public const double ELEMENT_VALIDATE_SELECTED_TIMEOUT = 1.0;
        public const double ELEMENT_VALIDATE_FOCUSED_TIMEOUT = 1.0;
        public const double ELEMENT_VALIDATE_STALE_TIMEOUT = 0.1;
        public const double EMAIL_RECEIVED_TIMEOUT = 60.0;
        public const double REFRESH_QUERY_TIMEOUT = 270.0;
        public const double PAGE_LOAD_TIMEOUT = 60.0;
        public const int STALE_MAX_ATTEMPTS = 3;

        // Directory Locations
        public static string IMAGES_DIRECTORY = (Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\..\..\Images\");
        public static string INPUT_DIRECTORY = (Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\..\..\File Input\");

        // Page Tracking Tools
        public static string DefaultLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location).ToString();
        public static object PageDictLock = new object(); // Used to coordinate thread access of PAGE_OBJECT_URLS
        public static Dictionary<String, String> PAGE_OBJECT_URLS = new Dictionary<String, String>();
        public static string CurrentPage = "InvalidPage";
        public static string LOGS_DIRECTORY = (Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\..\..\Logs\");
        public static string DOWNLOADS_DIRECTORY = DefaultLocation.Substring(0, DefaultLocation.Length - 9) + @"\..\..\Downloads\";
        public static string OUTPUT_DIRECTORY = DefaultLocation.Substring(0, DefaultLocation.Length - 9) + @"\..\..\Output\";
        public static string TEMPORARY_DIRECTORY = DefaultLocation.Substring(0, DefaultLocation.Length - 9) + @"\..\..\Temp\";
        public static string INPUT_DIRECOTRY = DefaultLocation.Substring(0, DefaultLocation.Length - 9) + @"\..\..\TestInputs\";
    }
}
