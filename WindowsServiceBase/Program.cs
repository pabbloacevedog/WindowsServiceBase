using System.Diagnostics;
using System.ServiceProcess;
using WindowsServiceBase.Sistema;

namespace WindowsServiceBase
{
    static class Program
    {
        /// <summary>
        /// Punto de entrada principal para la aplicación.
        /// </summary>
        static void Main()
       {
#if DEBUG
            Process[] servicio = Process.GetProcessesByName(Process.GetCurrentProcess().ProcessName);
            string ProcessName = servicio[0].ProcessName;
            int largo = servicio.Length;
            if (largo > 1)
            {
                CONFIG.PATH_LOG_ACTION = "E:/SERVICES/LOGS/WindowsServiceBase/LogAction-" + ProcessName + "-DDMMYYYY.log";
                LogEventos.EscribirLog("Main", "Ya existe un servicio \"" + ProcessName + "\" en ejecución, no se puede iniciar una segunda instancia", "","Action");
            }
            else
            {
                ServicioBase baseService = new ServicioBase();
                baseService.OnDebug();
                System.Threading.Thread.Sleep(System.Threading.Timeout.Infinite);
            }
#else
            Process[] servicio = Process.GetProcessesByName(Process.GetCurrentProcess().ProcessName);
            string ProcessName = servicio[0].ProcessName;
            int largo = servicio.Length;
            if (largo > 1)
            {
                CONFIG.PATH_LOG_ACTION = "E:/SERVICES/LOGS/" + ProcessName + "/LogAction-" + ProcessName + "-DDMMYYYY.log";
                LogEventos.EscribirLog("Main", "Ya existe un servicio \"" + ProcessName + "\" en ejecución, no se puede iniciar una segunda instancia", "","Action");
            }
            else
            {
                ServiceBase[] ServicesToRun;
                ServicesToRun = new ServiceBase[]
                {
                    new ServicioBase()
                };
                ServiceBase.Run(ServicesToRun);
            }
#endif            
        }
    }
}
