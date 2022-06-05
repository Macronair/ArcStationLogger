using System;
using System.Diagnostics;
using System.IO;

namespace Arc_Station_Logger.ServiceUnits
{
    internal class ServiceLog
    {

        public static void Write(string message, string source, EventLogEntryType type)
        {
            using (EventLog eventLog = new EventLog("Application"))
            {
                eventLog.Source = "[ASL Service] " + source;
                eventLog.WriteEntry(message, type);
            }

            if (!Directory.Exists(SettingsManager.logdir))
            {
                Directory.CreateDirectory(SettingsManager.logdir);
            }
            File.AppendAllText($"{SettingsManager.logdir}\\{DateTime.Now.Year}_{DateTime.Now.Month}_{DateTime.Now.Day}-ASLlog.txt", $"{DateTime.Now} : {message}{Environment.NewLine}");

        }

        public static void WriteFile(string message)
        {
            if (!Directory.Exists(SettingsManager.logdir))
            {
                Directory.CreateDirectory(SettingsManager.logdir);
            }
            File.AppendAllText($"{SettingsManager.logdir}\\{DateTime.Now.Year}_{DateTime.Now.Month}_{DateTime.Now.Day}-ASLlog.txt", $"{DateTime.Now} : {message}{Environment.NewLine}");
        }

        public static void WriteEvent(string message, string source, EventLogEntryType type)
        {
            using (EventLog eventLog = new EventLog("Application"))
            {
                eventLog.Source = "[ASL Service] " + source;
                eventLog.WriteEntry(message, type);
            }
        }

    }
}
