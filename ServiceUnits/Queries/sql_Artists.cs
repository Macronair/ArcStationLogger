using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arc_Station_Logger.ServiceUnits.Queries
{
    internal class sql_Artists
    {

        static string TableName = "01_Artists";

        public static void Run()
        {
            SqlCommand cmdCheckTable = new SqlCommand("SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = @TableName", Database.cnn);
            cmdCheckTable.Parameters.AddWithValue("@TableName", TableName);
            if ((int)cmdCheckTable.ExecuteScalar() == 0)
            {
                SqlCommand cmdCreateTable = new SqlCommand($"CREATE TABLE [dbo].[{TableName}] (" +
                    $"[Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY, " +
                    $"[Artist] NVARCHAR(MAX) NOT NULL, " +
                    $"[TotalSpins] INT NULL, " +
                    $"[LastPlayed] DATETIME NOT NULL, " +
                    $"[FirstPlayed] DATETIME NOT NULL)", Database.cnn);
                cmdCreateTable.ExecuteNonQuery();
            }
        }

        private static void InsertRecord()
        {
            SqlCommand cmdCheckSong = new SqlCommand($"SELECT COUNT(*) FROM [dbo].[{TableName}] WHERE Artist = @Artist;", Database.cnn);
            cmdCheckSong.Parameters.AddWithValue("@Artist", SettingsManager.CurrentArtist);
            int results = Convert.ToInt32(cmdCheckSong.ExecuteScalar());

            if (results == 0)        // Run if song doesn't exists.
            {
                SqlCommand cmdInsertSong = new SqlCommand($"INSERT INTO [dbo].[{TableName}] (Artist,TotalSpins,LastPlayed,FirstPlayed) VALUES (@Artist,@Total,@LastPlayed,@FirstPlayed)", Database.cnn);
                cmdInsertSong.Parameters.AddWithValue("@Artist", SettingsManager.CurrentArtist);
                cmdInsertSong.Parameters.AddWithValue("@Total", 1);
                cmdInsertSong.Parameters.AddWithValue("@LastPlayed", DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss"));
                cmdInsertSong.Parameters.AddWithValue("@FirstPlayed", DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss"));
                cmdInsertSong.ExecuteNonQuery();
            }
            else if (results > 0)    // Run if song is already in the datatable.
            {
                SqlCommand cmdUpdateSong = new SqlCommand($"UPDATE [dbo].[{TableName}] SET TotalSpins = TotalSpins + 1, LastPlayed = @LastPlayed WHERE Artist = @Artist", Database.cnn);
                cmdUpdateSong.Parameters.AddWithValue("@Artist", SettingsManager.CurrentArtist);
                cmdUpdateSong.Parameters.AddWithValue("@LastPlayed", DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss"));
                cmdUpdateSong.ExecuteNonQuery();
            }
        }

    }
}
