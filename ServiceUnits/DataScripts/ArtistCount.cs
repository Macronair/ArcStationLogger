using System;
using System.Data.SqlClient;
using System.Threading;

namespace Arc_Station_Logger.ServiceUnits.DataScripts
{
    internal class ArtistCount
    {

        public static string tableName = "TB_ArtistCount_Total";
        public static string tableNameM = $"TB_ArtistCount_{DateTime.Now.Year}_{DateTime.Now.Month}";

        public static void Run()
        {
            CheckTotal();   // Check total artist spins
            CheckMonth();   // Check monthly artist spins
        }

        #region Total spins per artist
        private static void CheckTotal()
        {
            SqlCommand create = new SqlCommand("SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = @TableName", Database.cnn);
            create.Parameters.AddWithValue("@TableName", tableName);
            if ((int)create.ExecuteScalar() == 0)
            {
                SqlCommand sqlCommand1 = new SqlCommand(string.Concat("CREATE TABLE [dbo].[", tableName, "] ([Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY, [Artist] NVARCHAR(MAX) NOT NULL, [TotalPlayCount] INT NULL, [LastPlayed] DATETIME NULL, [FirstPlayed] DATETIME NULL)"), Database.cnn);
                sqlCommand1.ExecuteNonQuery();
            }
            Thread.Sleep(20);

            SqlCommand sqlCommand = new SqlCommand("SELECT COUNT(*) FROM [dbo].[TB_ArtistCount_Total] WHERE Artist = @Artist;", Database.cnn);
            sqlCommand.Parameters.AddWithValue("@Artist", SettingsManager.CurrentArtist);
            sqlCommand.Parameters.AddWithValue("@Title", SettingsManager.CurrentTitle);
            int num1 = Convert.ToInt32(sqlCommand.ExecuteScalar());

            if (num1 > 0)
            {
                UpdateEntryT();
            }
            else
            {
                CreateEntryT();
            }
        }

        private static void CreateEntryT()
        {
            int num = 0;
            SqlCommand sqlCommand7 = new SqlCommand("SELECT MAX (Id) +1 FROM [dbo].[TB_ArtistCount_Total]", Database.cnn);
            try
            {
                num = (int)sqlCommand7.ExecuteScalar();
            }
            catch
            {
                num = 0;
            }
            SqlCommand sqlCommand8 = new SqlCommand("INSERT INTO [dbo].[TB_ArtistCount_Total] (Artist,TotalPlayCount,LastPlayed,FirstPlayed) VALUES (@Artist,@TotalPlayCount,@LastPlayed,@FirstPlayed)", Database.cnn);
            //sqlCommand8.Parameters.AddWithValue("@IId", num);
            sqlCommand8.Parameters.AddWithValue("@Artist", SettingsManager.CurrentArtist);
            sqlCommand8.Parameters.AddWithValue("@TotalPlayCount", 1);
            sqlCommand8.Parameters.AddWithValue("@LastPlayed", DateTime.Now);
            sqlCommand8.Parameters.AddWithValue("@FirstPlayed", DateTime.Now);
            sqlCommand8.ExecuteNonQuery();
        }

        private static void UpdateEntryT()
        {
            SqlCommand sqlCommand1 = new SqlCommand("UPDATE [dbo].[TB_ArtistCount_Total] SET TotalPlayCount = TotalPlayCount + 1 WHERE Artist = @Artist", Database.cnn);
            sqlCommand1.Parameters.AddWithValue("@Artist", SettingsManager.CurrentArtist);

            SqlCommand sqlCommand2 = new SqlCommand("UPDATE [dbo].[TB_ArtistCount_Total] SET LastPlayed = @Date WHERE Artist = @Artist", Database.cnn);
            sqlCommand2.Parameters.AddWithValue("@Date", DateTime.Now);
            sqlCommand2.Parameters.AddWithValue("@Artist", SettingsManager.CurrentArtist);

            sqlCommand1.ExecuteNonQuery();
            sqlCommand2.ExecuteNonQuery();
        }
        #endregion

        #region Monthly spins per artist
        private static void CheckMonth()
        {
            SqlCommand create = new SqlCommand("SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = @TableName", Database.cnn);
            create.Parameters.AddWithValue("@TableName", tableNameM);
            if ((int)create.ExecuteScalar() == 0)
            {
                SqlCommand sqlCommand1 = new SqlCommand(string.Concat("CREATE TABLE [dbo].[", tableNameM, "] ([Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY, [Artist] NVARCHAR(MAX) NOT NULL, [TotalPlayCount] INT NULL, [LastPlayed] DATETIME NULL, [FirstPlayed] DATETIME NULL)"), Database.cnn);
                sqlCommand1.ExecuteNonQuery();
            }
            Thread.Sleep(20);

            SqlCommand sqlCommand = new SqlCommand(String.Concat("SELECT COUNT(*) FROM [dbo].[", tableNameM, "] WHERE Artist = @Artist;"), Database.cnn);
            sqlCommand.Parameters.AddWithValue("@Artist", SettingsManager.CurrentArtist);
            sqlCommand.Parameters.AddWithValue("@Title", SettingsManager.CurrentTitle);
            int num1 = Convert.ToInt32(sqlCommand.ExecuteScalar());

            if (num1 > 0)
            {
                UpdateEntryM();
            }
            else
            {
                CreateEntryM();
            }
        }

        private static void CreateEntryM()
        {
            int num = 0;
            SqlCommand sqlCommand7 = new SqlCommand($"SELECT MAX (Id) +1 FROM [dbo].[{tableNameM}]", Database.cnn);
            try
            {
                num = (int)sqlCommand7.ExecuteScalar();
            }
            catch
            {
                num = 0;
            }
            SqlCommand sqlCommand8 = new SqlCommand($"INSERT INTO [dbo].[{tableNameM}] (Artist,TotalPlayCount,LastPlayed,FirstPlayed) VALUES (@Artist,@TotalPlayCount,@LastPlayed,@FirstPlayed)", Database.cnn);
            //sqlCommand8.Parameters.AddWithValue("@IId", num);
            sqlCommand8.Parameters.AddWithValue("@Artist", SettingsManager.CurrentArtist);
            sqlCommand8.Parameters.AddWithValue("@TotalPlayCount", 1);
            sqlCommand8.Parameters.AddWithValue("@LastPlayed", DateTime.Now);
            sqlCommand8.Parameters.AddWithValue("@FirstPlayed", DateTime.Now);
            sqlCommand8.ExecuteNonQuery();
        }

        private static void UpdateEntryM()
        {
            SqlCommand sqlCommand1 = new SqlCommand($"UPDATE [dbo].[{tableNameM}] SET TotalPlayCount = TotalPlayCount + 1 WHERE Artist = @Artist", Database.cnn);
            sqlCommand1.Parameters.AddWithValue("@Artist", SettingsManager.CurrentArtist);

            SqlCommand sqlCommand2 = new SqlCommand($"UPDATE [dbo].[{tableNameM}] SET LastPlayed = @Date WHERE Artist = @Artist", Database.cnn);
            sqlCommand2.Parameters.AddWithValue("@Date", DateTime.Now);
            sqlCommand2.Parameters.AddWithValue("@Artist", SettingsManager.CurrentArtist);

            sqlCommand1.ExecuteNonQuery();
            sqlCommand2.ExecuteNonQuery();
        }
        #endregion
    }
}
