using System;
using System.Configuration;
using System.IO;
using System.Reflection;

namespace Arc_Station_Logger.ServiceUnits
{
    internal class SettingsManager
    {

        public static string ListenDirectory;
        public static string ListenFile;

        public static string SQLserver;
        public static string SQLdatabase;
        public static string SQLlogin;
        public static string SQLpassword;

        public static int f_PlayCount;
        public static int f_PlayLog;
        public static int f_DailySpins;
        public static int f_ArtistCount;

        public static string CurrentArtist;
        public static string CurrentTitle;
        public static int SpinsTotal;
        public static int SpinsWeek;
        public static int SpinsMonth;
        public static int SpinsYear;

        public static bool Wait = false;

        public static string bindir = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
        public static string logdir = $"{Path.GetDirectoryName(Assembly.GetEntryAssembly().Location)}\\logs";

        public static void LoadAppConfig()
        {
            ListenDirectory = ConfigurationManager.AppSettings["folder_path"];
            ListenDirectory.Replace(@"\", @"\\");

            ListenFile = ConfigurationManager.AppSettings["file_path"];
            if (!ListenFile.EndsWith(".txt"))
            {
                ListenFile = ListenFile + ".txt";
            }

            SQLserver = ConfigurationManager.AppSettings["sql_server"];
            SQLdatabase = ConfigurationManager.AppSettings["sql_database"];
            SQLlogin = ConfigurationManager.AppSettings["sql_username"];
            SQLpassword = ConfigurationManager.AppSettings["sql_password"];

            if (ConfigurationManager.AppSettings["enablePlayCount"] != "0" || ConfigurationManager.AppSettings["enablePlayCount"] != "1")
            {
                f_PlayCount = 1;
            }
            else
            {
                f_PlayCount = Convert.ToInt32(ConfigurationManager.AppSettings["enablePlayCount"]);
            }

            if (ConfigurationManager.AppSettings["enablePlayLog"] != "0" || ConfigurationManager.AppSettings["enablePlayLog"] != "1")
            {
                f_PlayLog = 1;
            }
            else
            {
                f_PlayLog = Convert.ToInt32(ConfigurationManager.AppSettings["enablePlayLog"]);
            }

            if (ConfigurationManager.AppSettings["enableDailySpins"] != "0" || ConfigurationManager.AppSettings["enableDailySpins"] != "1")
            {
                f_DailySpins = 1;
            }
            else
            {
                f_DailySpins = Convert.ToInt32(ConfigurationManager.AppSettings["enableDailySpins"]);
            }

            if (ConfigurationManager.AppSettings["enableArtistCount"] != "0" || ConfigurationManager.AppSettings["enableArtistCount"] != "1")
            {
                f_ArtistCount = 1;
            }
            else
            {
                f_ArtistCount = Convert.ToInt32(ConfigurationManager.AppSettings["enableArtistCount"]);
            }
        }

    }
}
