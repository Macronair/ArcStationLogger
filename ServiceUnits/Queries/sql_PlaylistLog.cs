using Arc_Station_Logger.ServiceUnits;
using System;
using System.Data.SqlClient;
using System.Threading;

namespace ArcLogger.Scripts
{
    internal class sql_PlaylistLog
    {

        private static string tableName;

        public sql_PlaylistLog()
        {
        }

        public static void Run()
        {
            DateTime now = DateTime.Now;
            tableName = "99_PlayLog_" + now.ToString("yyyy") + "_" + now.ToString("MM");
            Console.ForegroundColor = ConsoleColor.Magenta;
            SqlCommand sqlCommand = new SqlCommand("SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = @TableName", Database.cnn);
            sqlCommand.Parameters.AddWithValue("@TableName", tableName);
            if ((int)sqlCommand.ExecuteScalar() == 0)
            {
                SqlCommand newTable = new SqlCommand(string.Concat("CREATE TABLE [dbo].[", tableName, "] ([Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY, [PlayedOn] DATETIME NULL, [Artist] NVARCHAR(MAX) NULL, [Title] NVARCHAR(MAX) NULL)"), Database.cnn);
                newTable.ExecuteNonQuery();
            }
            Thread.Sleep(20);

            int num;
            SqlCommand sqlCommand2 = new SqlCommand("SELECT MAX(Id) +1 FROM [dbo].[ " + tableName + " ]", Database.cnn);
            try
            {
                num = (int)sqlCommand.ExecuteScalar();
            }
            catch
            {
                num = 0;
            }
            SqlCommand sqlCommand1 = new SqlCommand("INSERT INTO [dbo].[" + tableName + "] (PlayedOn,Artist,Title) VALUES (@PlayedOn,@Artist,@Title)", Database.cnn);
            //sqlCommand1.Parameters.AddWithValue("@Id", num);
            sqlCommand1.Parameters.AddWithValue("@Artist", SettingsManager.CurrentArtist);
            sqlCommand1.Parameters.AddWithValue("@Title", SettingsManager.CurrentTitle);
            sqlCommand1.Parameters.AddWithValue("@PlayedOn", DateTime.Now);
            sqlCommand1.ExecuteNonQuery();
        }
    }
}