using Arc_Station_Logger.ServiceUnits;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Threading;
using System.Timers;

namespace Arc_Station_Logger
{
    public partial class SvcCore : ServiceBase
    {

        //Create FileSystemWatcher object
        FileSystemWatcher fsw;

        //Path of Service binary (.exe file)
        string bindir = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);

        private static System.Timers.Timer WaitForNextReading;

        public SvcCore()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {

            // Read app.config file for all user defined settings.
            SettingsManager.LoadAppConfig();

            // If the logs folder does not exist in the desired location, then create a new folder.
            if(!Directory.Exists(SettingsManager.logdir))
            {
                Directory.CreateDirectory(SettingsManager.logdir);
            }

            // Create a timer to prevent double insertion to database.
            SetTimer();

            #region File Watcher
            // Set the file and folder path where the watcher must be looking for.
            try
            {
                fsw = new FileSystemWatcher(SettingsManager.ListenDirectory)
                {
                    EnableRaisingEvents = true,         // Allow the FileSystemWatcher to run.. events i guess???
                    IncludeSubdirectories = false,      // No subdirectories this time..
                    NotifyFilter = NotifyFilters.LastWrite,
                    Filter = SettingsManager.ListenFile // Which file will be monitored?
                };
            }
            catch (ArgumentException e)
            {
                EventLog.WriteEntry("An error occured while loading the File Watcher object. Does the folder or file even exist?", EventLogEntryType.Error);
            }


            // What should be executed when the desired is changed?
            fsw.Changed += fswChangEvent;
            #endregion

            #region SQL
            // A small SQL connection test. If there is something wrong, it will output an error message.
            Database.Open();
            Database.Close();
            #endregion

            var l = Environment.NewLine;
            ServiceLog.WriteEvent($"Station Logger has been started.{l}{l}" +
                $"This file will be monitored: {SettingsManager.ListenDirectory}\\{SettingsManager.ListenFile}{l}" +
                $"Pointing to database: {SettingsManager.SQLdatabase} @ {SettingsManager.SQLserver}", "Service Start", EventLogEntryType.Information);
        }

        // Run code below when the text file is changed //
        private void fswChangEvent(object sender, FileSystemEventArgs e)
        {
            // Used for testing //
            #region Test code
            //EventLog.WriteEntry("File has been changed!");
            //File.AppendAllText($"{bindir}\\log.txt", "File has been changed!");
            #endregion
            // The real code is here!
            #region Read text file
            if(SettingsManager.Wait == false)
            {
                SettingsManager.Wait = true;
                WaitForNextReading.Start();

                Thread.Sleep(400);

                var line = File.ReadLines($"{SettingsManager.ListenDirectory}\\{SettingsManager.ListenFile}").First();

                if (line == "")
                {
                    ServiceLog.Write("File changed but no song data is found.", "Now Playing: None", EventLogEntryType.Warning);
                }
                else
                {
                    string[] songinfo = line.Split(new string[] { " - " }, StringSplitOptions.None);

                    SettingsManager.CurrentArtist = songinfo[0];
                    SettingsManager.CurrentTitle = songinfo[1];

                    //ServiceLog.WriteFile($"Now Playing: {SettingsManager.CurrentArtist} - {SettingsManager.CurrentTitle}");

                    Database.InsertToDB();
                }
            }
            #endregion
        }
        // end //

        // Timer code
        private static void SetTimer()
        {
            WaitForNextReading = new System.Timers.Timer(1000);
            WaitForNextReading.Elapsed += new ElapsedEventHandler(OnTimedEvent);
            WaitForNextReading.Enabled = true;
        }

        private static void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            SettingsManager.Wait = false;
            WaitForNextReading.Stop();
        }
        //end

        protected override void OnStop()
        {
            ServiceLog.WriteEvent("Station Logger has been stopped.", "Service Stop", EventLogEntryType.Information);    // Just a simple log message to say goodbye.
        }
    }
}
