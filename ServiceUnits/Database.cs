using Arc_Station_Logger.ServiceUnits.DataScripts;
using Arc_Station_Logger.ServiceUnits.Queries;
using ArcLogger.Scripts;
using System;
using System.Data.SqlClient;
using System.Diagnostics;

namespace Arc_Station_Logger.ServiceUnits
{
    class Database
    {

        static string connetionString = string.Format(@"Data Source={0};Initial Catalog={1};User ID={2};Password={3}"
                    , SettingsManager.SQLserver
                    , SettingsManager.SQLdatabase
                    , SettingsManager.SQLlogin
                    , SettingsManager.SQLpassword);

        public static SqlConnection cnn = new SqlConnection(connetionString);

        public static void Open()
        {
            try
            {
                cnn.Open();
            }
            catch (Exception ex)
            {
                var l = Environment.NewLine;
                ServiceLog.Write($"An error occured when connecting to database.{l}The service will still be running. But will possibly crash when using any database functions.{l}Reason:{l}{ex.Message}{l}{l}Connection info: {connetionString}", "SQL", EventLogEntryType.Error);
            }

        }

        public static void Close()
        {
            try
            {
                cnn.Close();
            }
            catch (Exception ex)
            {
                var l = Environment.NewLine;
                ServiceLog.Write($"An error occured when disconnecting to database.{l}The service will still be running. But will possibly crash when using any database functions.{l}Reason:{l}{ex.Message}{l}{l}Connection info: {connetionString}", "SQL", EventLogEntryType.Error);
            }
        }

        public static void InsertToDB()
        {

            if (SettingsManager.f_DailySpins == 1)
            {
                //DailySpins.Run();
            }
            if (SettingsManager.f_PlayLog == 1)
            {
                //PlayLog.Run();
            }
            if (SettingsManager.f_ArtistCount == 1)
            {
                //ArtistCount.Run();
            }
            if (SettingsManager.f_PlayCount == 1)
            {
                //PlayCount.Run();
            }

            cnn.Open();

            sql_Artists.Run();
            sql_Songs.Run();
            sql_MonthlySpins.Run();
            sql_WeeklySpins.Run();

            cnn.Close();
        }

    }
}
