using System.ServiceProcess;

namespace Arc_Station_Logger
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new SvcCore()
            };
            ServiceBase.Run(ServicesToRun);
        }
    }
}
