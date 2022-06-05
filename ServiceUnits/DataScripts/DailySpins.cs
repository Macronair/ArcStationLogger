using Arc_Station_Logger.ServiceUnits;
using System;
using System.Data.SqlClient;
using System.Globalization;
using System.Threading;

namespace ArcLogger.Scripts
{
    internal class DailySpins
    {
        private static string tableName;

        public DailySpins()
        {
        }

        private static void ChangeRecord()
        {
            SqlCommand sqlCommand = new SqlCommand(string.Concat("UPDATE dbo.", DailySpins.tableName, " SET Spins = Spins+1 WHERE AirDay = @DateNow"), Database.cnn);
            string str = DateTime.Now.Date.ToString("yyyy-MM-dd");
            sqlCommand.Parameters.AddWithValue("@DateNow", str);
            try
            {
                sqlCommand.ExecuteNonQuery();
                Console.WriteLine("{0} : Daily Spins: +1", DateTime.Now);
            }
            catch (SqlException sqlException)
            {
                Console.WriteLine(sqlException.Message);
            }
            Console.ForegroundColor = ConsoleColor.Gray;
        }

        private static void InsertRecord()
        {
            SqlCommand sqlCommand = new SqlCommand(string.Concat("INSERT INTO dbo.", DailySpins.tableName, "(AirDay,Spins,WeekNumber,WeekDay) VALUES (@AirDay,@Spins,@WeekNumber,@WeekDay)"), Database.cnn);
            string str = DateTime.Now.Date.ToString("yyyy-MM-dd");
            string str1 = DateTime.Now.DayOfWeek.ToString();
            Console.Write("{0} : New date detected! Creating new record..", DateTime.Now);
            sqlCommand.Parameters.AddWithValue("@AirDay", str);
            sqlCommand.Parameters.AddWithValue("@Spins", 1);
            sqlCommand.Parameters.AddWithValue("@WeekNumber", DailySpins.WeekNr());
            sqlCommand.Parameters.AddWithValue("@WeekDay", str1);
            try
            {
                sqlCommand.ExecuteNonQuery();
                Console.WriteLine(" Done!");
            }
            catch (SqlException sqlException1)
            {
                SqlException sqlException = sqlException1;
                Console.WriteLine();
                Console.WriteLine(sqlException.Message);
            }
            Console.ForegroundColor = ConsoleColor.Gray;
        }

        public static void Run()
        {
            DateTime now = DateTime.Now;
            DailySpins.tableName = string.Concat("TB_Spins_", now.ToString("yyyy"));
            Console.ForegroundColor = ConsoleColor.Magenta;
            SqlCommand sqlCommand = new SqlCommand("SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = @TableName", Database.cnn);
            sqlCommand.Parameters.AddWithValue("@TableName", DailySpins.tableName);
            if ((int)sqlCommand.ExecuteScalar() == 0)
            {
                SqlCommand sqlCommand1 = new SqlCommand(string.Concat("CREATE TABLE [dbo].[", DailySpins.tableName, "] ([Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY, [AirDay] VARCHAR(50) NOT NULL, [Spins] INT NOT NULL, [WeekNumber] INT NULL, [WeekDay] TEXT NULL)"), Database.cnn);
                sqlCommand1.ExecuteNonQuery();
            }
            Thread.Sleep(20);
            SqlCommand sqlCommand2 = new SqlCommand(string.Concat("SELECT COUNT(*) FROM [dbo].[", DailySpins.tableName, "] WHERE AirDay = @DateNow"), Database.cnn);
            string str = DateTime.Now.ToString("yyyy-MM-dd");
            sqlCommand2.Parameters.AddWithValue("@DateNow", str);
            int num = (int)sqlCommand2.ExecuteScalar();
            if (num > 0)
            {
                DailySpins.ChangeRecord();
            }
            else if (num == 0)
            {
                DailySpins.InsertRecord();
            }
        }

        public static int WeekNr()
        {
            DateTimeFormatInfo currentInfo = DateTimeFormatInfo.CurrentInfo;
            Calendar calendar = currentInfo.Calendar;
            int weekOfYear = calendar.GetWeekOfYear(DateTime.Now, currentInfo.CalendarWeekRule, currentInfo.FirstDayOfWeek);
            return weekOfYear;
        }
    }
}