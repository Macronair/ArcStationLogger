using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arc_Station_Logger.ServiceUnits.Queries
{
    internal class TitleSpins
    {

        static string TableName;

        public static void Run()
        {
            string TableName = "01_TitleSpins_" + DateTime.Now.Year;
            SqlCommand cmdCheckTable = new SqlCommand("SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = @TableName", Database.cnn);
            cmdCheckTable.Parameters.AddWithValue("@TableName", TableName);
            if ((int)cmdCheckTable.ExecuteScalar() == 0)
            {
                SqlCommand cmdCreateTable = new SqlCommand($"CREATE TABLE [dbo].[] (" +
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

        public static void InsertRecord()
        {
            SqlCommand cmdCheckSong = new SqlCommand("SELECT COUNT(*) FROM [dbo].[TB_PlayCount] WHERE Artist = @Artist AND Title = @Title;", Database.cnn);
            cmdCheckSong.Parameters.AddWithValue("@Artist", SettingsManager.CurrentArtist);
            cmdCheckSong.Parameters.AddWithValue("@Title", SettingsManager.CurrentTitle);
            int num1 = Convert.ToInt32(sqlCommand.ExecuteScalar());

        }
    }
}
