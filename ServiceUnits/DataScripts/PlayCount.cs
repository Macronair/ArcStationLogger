using Arc_Station_Logger.ServiceUnits;
using System;
using System.Data.SqlClient;
using System.Threading;

namespace ArcLogger.Scripts
{
    internal class PlayCount
    {
        public PlayCount()
        {
        }

        public static void Run()
        {

            DateTime now = DateTime.Now;
            string tableName = string.Concat("TB_PlayCount");
            SqlCommand create = new SqlCommand("SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = @TableName", Database.cnn);
            create.Parameters.AddWithValue("@TableName", tableName);
            if ((int)create.ExecuteScalar() == 0)
            {
                SqlCommand sqlCommand1 = new SqlCommand(string.Concat("CREATE TABLE [dbo].[", tableName, "] ([Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY, [Artist] NVARCHAR(MAX) NOT NULL, [Title] NVARCHAR(MAX) NOT NULL, [TotalPlayCount] INT NULL, [WeekPlayCount] INT NULL, [MonthPlayCount] INT NULL, [YearPlayCount] INT NULL, [LastPlayed] DATETIME NULL, [FirstPlayed] DATETIME NULL)"), Database.cnn);
                sqlCommand1.ExecuteNonQuery();
            }
            Thread.Sleep(20);

            int num;
            SqlCommand sqlCommand = new SqlCommand("SELECT COUNT(*) FROM [dbo].[TB_PlayCount] WHERE Artist = @Artist AND Title = @Title;", Database.cnn);
            sqlCommand.Parameters.AddWithValue("@Artist", SettingsManager.CurrentArtist);
            sqlCommand.Parameters.AddWithValue("@Title", SettingsManager.CurrentTitle);
            int num1 = Convert.ToInt32(sqlCommand.ExecuteScalar());
            if (num1 > 0)
            {
                SqlCommand sqlCommand1 = new SqlCommand("UPDATE [dbo].[TB_PlayCount] SET TotalPlayCount = TotalPlayCount + 1 WHERE Artist = @Artist AND Title = @Title", Database.cnn);
                sqlCommand1.Parameters.AddWithValue("@Artist", SettingsManager.CurrentArtist);
                sqlCommand1.Parameters.AddWithValue("@Title", SettingsManager.CurrentTitle);
                sqlCommand1.ExecuteNonQuery();
                SqlCommand sqlCommand2 = new SqlCommand("UPDATE [dbo].[TB_PlayCount] SET WeekPlayCount = WeekPlayCount + 1 WHERE Artist = @Artist AND Title = @Title", Database.cnn);
                sqlCommand2.Parameters.AddWithValue("@Artist", SettingsManager.CurrentArtist);
                sqlCommand2.Parameters.AddWithValue("@Title", SettingsManager.CurrentTitle);
                sqlCommand2.ExecuteNonQuery();
                SqlCommand sqlCommand3 = new SqlCommand("UPDATE [dbo].[TB_PlayCount] SET MonthPlayCount = MonthPlayCount + 1 WHERE Artist = @Artist AND Title = @Title", Database.cnn);
                sqlCommand3.Parameters.AddWithValue("@Artist", SettingsManager.CurrentArtist);
                sqlCommand3.Parameters.AddWithValue("@Title", SettingsManager.CurrentTitle);
                sqlCommand3.ExecuteNonQuery();
                SqlCommand sqlCommand4 = new SqlCommand("UPDATE [dbo].[TB_PlayCount] SET YearPlayCount = YearPlayCount + 1 WHERE Artist = @Artist AND Title = @Title", Database.cnn);
                sqlCommand4.Parameters.AddWithValue("@Artist", SettingsManager.CurrentArtist);
                sqlCommand4.Parameters.AddWithValue("@Title", SettingsManager.CurrentTitle);
                sqlCommand4.ExecuteNonQuery();
                SqlCommand sqlCommand5 = new SqlCommand("UPDATE [dbo].[TB_PlayCount] SET LastPlayed = @LastPlayed WHERE Artist = @Artist AND Title = @Title", Database.cnn);
                sqlCommand5.Parameters.AddWithValue("@Artist", SettingsManager.CurrentArtist);
                sqlCommand5.Parameters.AddWithValue("@Title", SettingsManager.CurrentTitle);
                sqlCommand5.Parameters.AddWithValue("@LastPlayed", DateTime.Now);
                sqlCommand5.ExecuteNonQuery();
                SqlCommand sqlCommand6 = new SqlCommand("SELECT Id, TotalPlayCount, WeekPlayCount, MonthPlayCount, YearPlayCount  FROM [dbo].[TB_PlayCount] WHERE Artist = @Artist AND Title = @Title;", Database.cnn);
                sqlCommand6.Parameters.AddWithValue("@Artist", SettingsManager.CurrentArtist);
                sqlCommand6.Parameters.AddWithValue("@Title", SettingsManager.CurrentTitle);
                SqlDataReader sqlDataReader = sqlCommand6.ExecuteReader();
                if (!sqlDataReader.HasRows)
                {
                }
                else
                {
                    while (sqlDataReader.Read())
                    {
                        SettingsManager.SpinsTotal = sqlDataReader.GetInt32(1);
                        SettingsManager.SpinsWeek = sqlDataReader.GetInt32(2);
                        SettingsManager.SpinsMonth = sqlDataReader.GetInt32(3);
                        SettingsManager.SpinsYear = sqlDataReader.GetInt32(4);
                    }
                }
                ServiceLog.Write($"Song Spins: '{SettingsManager.CurrentTitle}' by {SettingsManager.CurrentArtist} | Total: {SettingsManager.SpinsTotal} | Week: {SettingsManager.SpinsWeek} | Month: {SettingsManager.SpinsMonth} | Year: {SettingsManager.SpinsYear}", "Now Playing", System.Diagnostics.EventLogEntryType.Information);
            }
            else if (num1 == 0)
            {
                SqlCommand sqlCommand7 = new SqlCommand("SELECT MAX (Id) +1 FROM [dbo].[TB_PlayCount]", Database.cnn);
                try
                {
                    num = (int)sqlCommand7.ExecuteScalar();
                }
                catch
                {
                    num = 0;
                }
                SqlCommand sqlCommand8 = new SqlCommand("INSERT INTO [dbo].[TB_PlayCount] (Artist,Title,TotalPlayCount,WeekPlayCount,MonthPlayCount,YearPlayCount,LastPlayed,FirstPlayed) VALUES (@Artist,@Title,@TotalPlayCount,@WeekPlayCount,@MonthPlayCount,@YearPlayCount,@LastPlayed,@FirstPlayed)", Database.cnn);
                //sqlCommand8.Parameters.AddWithValue("@IId", num);
                sqlCommand8.Parameters.AddWithValue("@Artist", SettingsManager.CurrentArtist);
                sqlCommand8.Parameters.AddWithValue("@Title", SettingsManager.CurrentTitle);
                sqlCommand8.Parameters.AddWithValue("@TotalPlayCount", 1);
                sqlCommand8.Parameters.AddWithValue("@WeekPlayCount", 1);
                sqlCommand8.Parameters.AddWithValue("@MonthPlayCount", 1);
                sqlCommand8.Parameters.AddWithValue("@YearPlayCount", 1);
                sqlCommand8.Parameters.AddWithValue("@LastPlayed", DateTime.Now);
                sqlCommand8.Parameters.AddWithValue("@FirstPlayed", DateTime.Now);
                sqlCommand8.ExecuteNonQuery();
                ServiceLog.Write($"Song '{SettingsManager.CurrentTitle}' by {SettingsManager.CurrentArtist} has been added to the database.", "Now Playing: New Song", System.Diagnostics.EventLogEntryType.Information);
            }
        }
    }
}