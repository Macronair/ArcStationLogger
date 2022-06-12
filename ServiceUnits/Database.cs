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

            if(SettingsManager.NewSong == true)
            {
                ServiceLog.WriteFile($"Now Playing (NEW SONG): {SettingsManager.CurrentArtist} - {SettingsManager.CurrentTitle} |T:{SettingsManager.Spins_T_Song}|W:{SettingsManager.Spins_W_Song}|M:{SettingsManager.Spins_M_Song}|Y:{SettingsManager.Spins_Y_Song}|");

                var nl = Environment.NewLine;
                string eventmessage =
                    $"-= SONG ADDED TO DATABASE =-{nl}" +
                    $"Now Playing: {SettingsManager.CurrentArtist} - {SettingsManager.CurrentTitle} {nl}" +
                    $"----------------{nl}" +
                    $"Song Spins: {nl}" +
                    $" - This week: {SettingsManager.Spins_W_Song}{nl}" +
                    $" - This month: {SettingsManager.Spins_M_Song}{nl}" +
                    $" - This year: {SettingsManager.Spins_Y_Song}{nl}" +
                    $" - Total: {SettingsManager.Spins_T_Song}{nl}" +
                    $"{nl}" +
                    $"Artist Spins:{nl}" +
                    $" - This year: {SettingsManager.Spins_Y_Artist}{nl}" +
                    $" - Total: {SettingsManager.Spins_T_Artist}";
                ServiceLog.WriteEvent(eventmessage, "Now Playing (NEW SONG)", EventLogEntryType.Information);
            }
            else
            {
                ServiceLog.WriteFile($"Now Playing: {SettingsManager.CurrentArtist} - {SettingsManager.CurrentTitle} |T:{SettingsManager.Spins_T_Song}|W:{SettingsManager.Spins_W_Song}|M:{SettingsManager.Spins_M_Song}|Y:{SettingsManager.Spins_Y_Song}|");

                var nl = Environment.NewLine;
                string eventmessage =
                    $"Now Playing: {SettingsManager.CurrentArtist} - {SettingsManager.CurrentTitle} {nl}" +
                    $"----------------{nl}" +
                    $"Song Spins: {nl}" +
                    $" - This week: {SettingsManager.Spins_W_Song}{nl}" +
                    $" - This month: {SettingsManager.Spins_M_Song}{nl}" +
                    $" - This year: {SettingsManager.Spins_Y_Song}{nl}" +
                    $" - Total: {SettingsManager.Spins_T_Song}{nl}" +
                    $"{nl}" +
                    $"Artist Spins:{nl}" +
                    $" - This year: {SettingsManager.Spins_Y_Artist}{nl}" +
                    $" - Total: {SettingsManager.Spins_T_Artist}";
                ServiceLog.WriteEvent(eventmessage, "Now Playing", EventLogEntryType.Information);
            }
        }

    }
}
