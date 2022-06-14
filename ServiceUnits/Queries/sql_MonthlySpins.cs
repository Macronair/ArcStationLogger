using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arc_Station_Logger.ServiceUnits.Queries
{
    internal class sql_MonthlySpins
    {

        static string TableName = "02_MonthlySpins_" + DateTime.Now.Year;
        static string CurrentMonth;

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
                    $"[Total] INT NULL, " +
                    $"[Jan] INT NULL, " +
                    $"[Feb] INT NULL, " +
                    $"[Mar] INT NULL, " +
                    $"[Apr] INT NULL, " +
                    $"[May] INT NULL, " +
                    $"[Jun] INT NULL, " +
                    $"[Jul] INT NULL, " +
                    $"[Aug] INT NULL, " +
                    $"[Sep] INT NULL, " +
                    $"[Oct] INT NULL, " +
                    $"[Nov] INT NULL, " +
                    $"[Dec] INT NULL)", Database.cnn);
                cmdCreateTable.ExecuteNonQuery();
            }

            InsertRecord();
        }

        private static void InsertRecord()
        {
            
            int MonthNumber = Convert.ToInt32(DateTime.Now.ToString("MM")); // Determines the current month

            // Convert the month number to a 3-letter string.
            switch (MonthNumber)
            {
                case 1: CurrentMonth = "Jan"; break;
                case 2: CurrentMonth = "Feb"; break;
                case 3: CurrentMonth = "Mar"; break;
                case 4: CurrentMonth = "Apr"; break;
                case 5: CurrentMonth = "May"; break;
                case 6: CurrentMonth = "Jun"; break;
                case 7: CurrentMonth = "Jal"; break;
                case 8: CurrentMonth = "Aug"; break;
                case 9: CurrentMonth = "Sep"; break;
                case 10: CurrentMonth = "Oct"; break;
                case 11: CurrentMonth = "Nov"; break;
                case 12: CurrentMonth = "Dec"; break;
            }

            SqlCommand cmdCheckSong = new SqlCommand($"SELECT COUNT(*) FROM [dbo].[{TableName}] WHERE Artist = N'{SettingsManager.CurrentArtist}' AND Title = N'{SettingsManager.CurrentTitle}';", Database.cnn);
            int results = Convert.ToInt32(cmdCheckSong.ExecuteScalar());

            if(results == 0)        // Run if song doesn't exists.
            {
                SqlCommand cmdInsertSong = new SqlCommand($"INSERT INTO [dbo].[{TableName}] (Artist,Title,Total,{CurrentMonth}) " +
                    $"VALUES " +
                    $"(N'{SettingsManager.CurrentArtist}',N'{SettingsManager.CurrentTitle}',@Total,@Month)", Database.cnn);
                cmdInsertSong.Parameters.AddWithValue("@Total", 1);
                cmdInsertSong.Parameters.AddWithValue("@Month", 1);
                cmdInsertSong.ExecuteNonQuery();
            }
            else if(results > 0)    // Run if song is already in the datatable.
            {
                SqlCommand cmdUpdateSong = new SqlCommand($"UPDATE [dbo].[{TableName}] SET Total = Total + 1, {CurrentMonth} = {CurrentMonth} + 1 WHERE Artist = N'{SettingsManager.CurrentArtist}' AND Title = N'{SettingsManager.CurrentTitle}'", Database.cnn);
                cmdUpdateSong.ExecuteNonQuery();
            }

            // Return the values how many times the song has played.

            using (SqlCommand cmdReadSpins = new SqlCommand($"SELECT {CurrentMonth},Total FROM [dbo].[{TableName}] WHERE Artist = N'{SettingsManager.CurrentArtist}' AND Title = N'{SettingsManager.CurrentTitle}'", Database.cnn))
            {
                using (SqlDataReader dr = cmdReadSpins.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        SettingsManager.Spins_Y_Song = (int)dr["Total"];
                        SettingsManager.Spins_M_Song = (int)dr[CurrentMonth];
                    }
                }
            }
        }
    }
}
