using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arc_Station_Logger.ServiceUnits.Queries
{
    internal class sql_WeeklySpins
    {

        static string TableName = "03_WeeklySpins_" + DateTime.Now.Year;
        static string WeekColumn = $"Wk{WeekNr()}_{DateTimeExtensions.FirstDayOfWeek(DateTime.Now).ToString("MM")}";

        public static void Run()
        {
            SqlCommand cmdCheckTable = new SqlCommand("SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = @TableName", Database.cnn);
            cmdCheckTable.Parameters.AddWithValue("@TableName", TableName);
            if ((int)cmdCheckTable.ExecuteScalar() == 0)
            {
                SqlCommand cmdCreateTable = new SqlCommand($"CREATE TABLE [dbo].[{TableName}] (" +
                    $"[Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY, " +
                    $"[Artist] NVARCHAR(MAX) NOT NULL, " +
                    $"[Title] NVARCHAR(MAX) NOT NULL, " +
                    $"[{WeekColumn}] INT NOT NULL)", Database.cnn);
                cmdCreateTable.ExecuteNonQuery();
            }

            CheckColumn();
        }

        private static void CheckColumn()
        {
            SqlCommand cmdCheckColumn = new SqlCommand($"SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS WHERE table_name = '{TableName}' AND column_name = '{WeekColumn}'", Database.cnn);
            if((int)cmdCheckColumn.ExecuteScalar() == 0)
            {
                SqlCommand cmdCreateColumn = new SqlCommand($"ALTER TABLE [dbo].[{TableName}] ADD {WeekColumn} INT NOT NULL DEFAULT(0)", Database.cnn);
                cmdCreateColumn.ExecuteNonQuery();
            }

            InsertRecord();
        }

        private static void InsertRecord()
        {
            SqlCommand cmdCheckSong = new SqlCommand($"SELECT COUNT(*) FROM [dbo].[{TableName}] WHERE Artist = @Artist AND Title = @Title;", Database.cnn);
            cmdCheckSong.Parameters.AddWithValue("@Artist", SettingsManager.CurrentArtist);
            cmdCheckSong.Parameters.AddWithValue("@Title", SettingsManager.CurrentTitle);
            int results = Convert.ToInt32(cmdCheckSong.ExecuteScalar());

            if (results == 0)        // Run if song doesn't exists.
            {
                SqlCommand cmdInsertSong = new SqlCommand($"INSERT INTO [dbo].[{TableName}] (Artist,Title,{WeekColumn}) VALUES (@Artist,@Title,@Week)", Database.cnn);
                cmdInsertSong.Parameters.AddWithValue("@Artist", SettingsManager.CurrentArtist);
                cmdInsertSong.Parameters.AddWithValue("@Title", SettingsManager.CurrentTitle);
                cmdInsertSong.Parameters.AddWithValue("@Week", 1);
                cmdInsertSong.ExecuteNonQuery();
            }
            else if (results > 0)    // Run if song is already in the datatable.
            {
                SqlCommand cmdUpdateSong = new SqlCommand($"UPDATE [dbo].[{TableName}] SET {WeekColumn} = {WeekColumn} + 1 WHERE Artist = @Artist AND Title = @Title", Database.cnn);
                cmdUpdateSong.Parameters.AddWithValue("@Artist", SettingsManager.CurrentArtist);
                cmdUpdateSong.Parameters.AddWithValue("@Title", SettingsManager.CurrentTitle);
                cmdUpdateSong.ExecuteNonQuery();
            }

            // Return the values how many times the song has played.
            using (SqlCommand cmdReadSpins = new SqlCommand($"SELECT {WeekColumn} FROM [dbo].[{TableName}] WHERE Artist = @Artist AND Title = @Title", Database.cnn))
            {
                cmdReadSpins.Parameters.AddWithValue("@Artist", SettingsManager.CurrentArtist);
                cmdReadSpins.Parameters.AddWithValue("@Title", SettingsManager.CurrentTitle);
                using (SqlDataReader dr = cmdReadSpins.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        try
                        {
                            SettingsManager.Spins_W_Song = (int)dr[WeekColumn];
                        }
                        catch(InvalidCastException ice)
                        {
                            SettingsManager.Spins_W_Song = 0;
                        }
                        
                    }
                }
            }
        }

        private static int WeekNr()
        {
            DateTimeFormatInfo currentInfo = DateTimeFormatInfo.CurrentInfo;
            Calendar calendar = currentInfo.Calendar;
            int weekOfYear = calendar.GetWeekOfYear(DateTime.Now, currentInfo.CalendarWeekRule, currentInfo.FirstDayOfWeek);
            return weekOfYear;
        }
    }

    public static partial class DateTimeExtensions
    {
        public static DateTime FirstDayOfWeek(this DateTime dt)
        {
            var culture = System.Threading.Thread.CurrentThread.CurrentCulture;
            var diff = dt.DayOfWeek - culture.DateTimeFormat.FirstDayOfWeek;

            if (diff < 0)
            {
                diff += 7;
            }

            return dt.AddDays(-diff).Date;
        }

        public static DateTime LastDayOfWeek(this DateTime dt) =>
            dt.FirstDayOfWeek().AddDays(6);

        public static DateTime FirstDayOfMonth(this DateTime dt) =>
            new DateTime(dt.Year, dt.Month, 1);

        public static DateTime LastDayOfMonth(this DateTime dt) =>
            dt.FirstDayOfMonth().AddMonths(1).AddDays(-1);

        public static DateTime FirstDayOfNextMonth(this DateTime dt) =>
            dt.FirstDayOfMonth().AddMonths(1);
    }
}
