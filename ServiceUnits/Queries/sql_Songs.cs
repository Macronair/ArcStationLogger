﻿using System;
using System.Collections.Generic;
using System.Data.SqlClient;
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
                    $"[LastPlayed] DATETIME NOT NULL, " +
                    $"[FirstPlayed] DATETIME NOT NULL)", Database.cnn);
                cmdCreateTable.ExecuteNonQuery();
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
                SqlCommand cmdInsertSong = new SqlCommand($"INSERT INTO [dbo].[{TableName}] (Artist,Title,TotalSpins,LastPlayed,FirstPlayed) VALUES (@Artist,@Title,@Total,@LastPlayed,@FirstPlayed)", Database.cnn);
                cmdInsertSong.Parameters.AddWithValue("@Artist", SettingsManager.CurrentArtist);
                cmdInsertSong.Parameters.AddWithValue("@Title", SettingsManager.CurrentTitle);
                cmdInsertSong.Parameters.AddWithValue("@Total", 1);
                cmdInsertSong.Parameters.AddWithValue("@LastPlayed", DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss"));
                cmdInsertSong.Parameters.AddWithValue("@FirstPlayed", DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss"));
                cmdInsertSong.ExecuteNonQuery();
                SettingsManager.NewSong = true;
            }
            else if (results > 0)    // Run if song is already in the datatable.
            {
                SqlCommand cmdUpdateSong = new SqlCommand($"UPDATE [dbo].[{TableName}] SET TotalSpins = TotalSpins + 1, LastPlayed = @LastPlayed WHERE Artist = @Artist AND Title = @Title", Database.cnn);
                cmdUpdateSong.Parameters.AddWithValue("@Artist", SettingsManager.CurrentArtist);
                cmdUpdateSong.Parameters.AddWithValue("@Title", SettingsManager.CurrentTitle);
                cmdUpdateSong.Parameters.AddWithValue("@LastPlayed", DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss"));
                cmdUpdateSong.ExecuteNonQuery();
                SettingsManager.NewSong = false;
            }

            // Return the values how many times the song has played.

            using (SqlCommand cmdReadSpins = new SqlCommand($"SELECT TotalSpins FROM [dbo].[{TableName}] WHERE Artist = @Artist AND Title = @Title", Database.cnn))
            {
                cmdReadSpins.Parameters.AddWithValue("@Artist", SettingsManager.CurrentArtist);
                cmdReadSpins.Parameters.AddWithValue("@Title", SettingsManager.CurrentTitle);
                using (SqlDataReader dr = cmdReadSpins.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        SettingsManager.Spins_T_Song = (int)dr["TotalSpins"];
                    }
                }
            }
        }

    }
}
