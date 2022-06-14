using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arc_Station_Logger.ServiceUnits.Queries
{
    internal class sql_Songs
    {

        static string TableName = "00_Songs";

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
                    $"[TotalSpins] INT NULL, " +
                    $"[Week] INT NULL, " +
                    $"[Month] INT NULL, " +
                    $"[Year] INT NULL, " +
                    $"[LastPlayed] DATETIME NOT NULL, " +
                    $"[FirstPlayed] DATETIME NOT NULL)", Database.cnn);
                cmdCreateTable.ExecuteNonQuery();
            }

            CheckDate();
        }

        private static void CheckDate()
        {

            // Check if saved date variables matches with the current time.
            // If no, then reset nessecary value in Week, Month or Year column.
            // It yes, do not reset anything at all.

            if (File.Exists($"{SettingsManager.bindir}\\timetracker"))
            {
                var rl = File.ReadLines($"{SettingsManager.bindir}\\timetracker").First();
                string[] currenttime = rl.Split(new string[] { " - " }, StringSplitOptions.None);

                // [0] = Week
                // [1] = Month
                // [2] = Year

                // If weeks does not match, reset the Week column.
                if (WeekNr() != Convert.ToInt32(currenttime[0]))
                {
                    SqlCommand reset = new SqlCommand($"UPDATE [dbo].[{TableName}] SET Week = 0", Database.cnn);
                    reset.ExecuteNonQuery();
                    File.WriteAllText($"{SettingsManager.bindir}\\timetracker", $"{WeekNr()} - {DateTime.Now.Month} - {DateTime.Now.Year}");
                }

                if (DateTime.Now.Month != Convert.ToInt32(currenttime[1]))
                {
                    SqlCommand reset = new SqlCommand($"UPDATE [dbo].[{TableName}] SET Month = 0", Database.cnn);
                    reset.ExecuteNonQuery();
                    File.WriteAllText($"{SettingsManager.bindir}\\timetracker", $"{WeekNr()} - {DateTime.Now.Month} - {DateTime.Now.Year}");
                }

                if (DateTime.Now.Year != Convert.ToInt32(currenttime[2]))
                {
                    SqlCommand reset = new SqlCommand($"UPDATE [dbo].[{TableName}] SET Year = 0", Database.cnn);
                    reset.ExecuteNonQuery();
                    File.WriteAllText($"{SettingsManager.bindir}\\timetracker", $"{WeekNr()} - {DateTime.Now.Month} - {DateTime.Now.Year}");
                }
            }
            else
            {
                File.WriteAllText($"{SettingsManager.bindir}\\timetracker", $"{WeekNr()} - {DateTime.Now.Month} - {DateTime.Now.Year}");
            }

            InsertRecord();
        }

        private static void InsertRecord()
        {
            SqlCommand cmdCheckSong = new SqlCommand($"SELECT COUNT(*) FROM [dbo].[{TableName}] WHERE Artist = N'{SettingsManager.CurrentArtist}' AND Title = N'{SettingsManager.CurrentTitle}';", Database.cnn);
            int results = Convert.ToInt32(cmdCheckSong.ExecuteScalar());

            if (results == 0)        // Run if song doesn't exists.
            {
                SqlCommand cmdInsertSong = new SqlCommand($"INSERT INTO [dbo].[{TableName}] (Artist,Title,TotalSpins,Week,Month,Year,LastPlayed,FirstPlayed) " +
                    $"VALUES " +
                    $"(N'{SettingsManager.CurrentArtist}',N'{SettingsManager.CurrentTitle}',@Total,@Week,@Month,@Year,@LastPlayed,@FirstPlayed)", Database.cnn);
                cmdInsertSong.Parameters.AddWithValue("@Total", 1);
                cmdInsertSong.Parameters.AddWithValue("@Week", 1);
                cmdInsertSong.Parameters.AddWithValue("@Month", 1);
                cmdInsertSong.Parameters.AddWithValue("@Year", 1);
                cmdInsertSong.Parameters.AddWithValue("@LastPlayed", DateTime.Now);
                cmdInsertSong.Parameters.AddWithValue("@FirstPlayed", DateTime.Now);
                cmdInsertSong.ExecuteNonQuery();
                SettingsManager.NewSong = true;
            }
            else if (results > 0)    // Run if song is already in the datatable.
            {
                SqlCommand cmdUpdateSong = new SqlCommand($"UPDATE [dbo].[{TableName}] SET TotalSpins = TotalSpins + 1, Week = Week + 1, Month = Month + 1, Year = Year + 1, LastPlayed = @LastPlayed WHERE Artist = N'{SettingsManager.CurrentArtist}' AND Title = N'{SettingsManager.CurrentTitle}'", Database.cnn);
                cmdUpdateSong.Parameters.AddWithValue("@LastPlayed", DateTime.Now);
                cmdUpdateSong.ExecuteNonQuery();
                SettingsManager.NewSong = false;
            }

            // Return the values how many times the song has played.

            using (SqlCommand cmdReadSpins = new SqlCommand($"SELECT TotalSpins FROM [dbo].[{TableName}] WHERE Artist = N'{SettingsManager.CurrentArtist}' AND Title = N'{SettingsManager.CurrentTitle}'", Database.cnn))
            {
                using (SqlDataReader dr = cmdReadSpins.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        SettingsManager.Spins_T_Song = (int)dr["TotalSpins"];
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
}
