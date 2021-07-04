using System;
using System.Linq;
using System.ServiceProcess;
using System.Threading;
using WindowsServiceBase.Sistema;

namespace WindowsServiceBase
{
    public partial class ServicioBase : ServiceBase
    {
        Thread hiloClienteTCP;
        Thread hiloClienteModbus;
        public ServicioBase()
        {
            AppDomain currentDomain = AppDomain.CurrentDomain;
            currentDomain.UnhandledException += new UnhandledExceptionEventHandler(CaputarExcepcion);
            InitializeComponent();
        }

        public void CaputarExcepcion(object sender, UnhandledExceptionEventArgs args)
        {
            Exception e = (Exception)args.ExceptionObject;
            LogEventos.EscribirLog("CaputarExcepcion", "¡Excepcion no controlada detectada! Descripción: " + e.Message + ", Origen : " + e.Source + ", Datos : " + e.StackTrace.ToString(), "", "Action");
            Stop();
        }

        public void OnDebug()
        {
            OnStart(null);
        }

        protected override void OnStart(string[] args)
        {
            try
            {
                if (CONFIG.CrearCONFIG())
                {
                    LogEventos.EscribirLog("OnStart", "----------------------------------------------------", "", "Action");
                    LogEventos.EscribirLog("OnStart", "La configuración inicial se ha creado correctamente", "", "Action");
                    LogEventos.EscribirLog("OnStart", "----------------------------------------------------", "", "Action");
                    LogEventos.EscribirLog("OnStart", "DISCO_ORIGEN = \"" + CONFIG.DISCO_ORIGEN + "\"", "", "Action");
                    LogEventos.EscribirLog("OnStart", "PATH_LOG_ACTION = \"" + CONFIG.PATH_LOG_ACTION + "\"", "", "Action");
                    LogEventos.EscribirLog("OnStart", "PATH_LOG_HB = \"" + CONFIG.PATH_LOG_HB + "\"", "", "Action");
                    LogEventos.EscribirLog("OnStart", "TIMER_HB = \"" + CONFIG.TIMER_HB + "\"", "", "Action");
                    LogEventos.EscribirLog("OnStart", "CLIENTE_TCP_IP = \"" + CONFIG.CLIENTE_TCP_IP + "\"", "", "Action");
                    LogEventos.EscribirLog("OnStart", "CLIENTE_TCP_PORT = \"" + CONFIG.CLIENTE_TCP_PORT + "\"", "", "Action");
                    LogEventos.EscribirLog("OnStart", "RECONECTAR = \"" + CONFIG.RECONECTAR + "\"", "", "Action");
                    LogEventos.EscribirLog("OnStart", "TIMEOUT = \"" + CONFIG.TIMEOUT + "\"", "", "Action");
                    LogEventos.EscribirLog("OnStart", "TIMER_COMUNICACION = \"" + CONFIG.TIMER_COMUNICACION + "\"", "", "Action");
                    LogEventos.EscribirLog("OnStart", "TIMER_LATENCIA_HB = \"" + CONFIG.TIMER_LATENCIA_HB + "\"", "", "Action");
                    LogEventos.EscribirLog("OnStart", "TIMER_SIN_RESPUESTA = \"" + CONFIG.TIMER_SIN_RESPUESTA + "\"", "", "Action");
                    LogEventos.EscribirLog("OnStart", "REINTENTOS = \"" + CONFIG.REINTENTOS + "\"", "", "Action");
                    LogEventos.EscribirLog("OnStart", "BAJAR_CONEXION = \"" + CONFIG.BAJAR_CONEXION + "\"", "", "Action");
                    LogEventos.EscribirLog("OnStart", "NOMBRE_MMF = \"" + CONFIG.NOMBRE_MMF + "\"", "", "Action");
                    LogEventos.EscribirLog("OnStart", "NOMBRE_MUTEX = \"" + CONFIG.NOMBRE_MUTEX + "\"", "", "Action");
                    LogEventos.EscribirLog("OnStart", "POSICION_ESTADO = \"" + CONFIG.POSICION_ESTADO + "\"", "", "Action");
                    LogEventos.EscribirLog("OnStart", "DB_SERVER = \"" + CONFIG.DB_SERVER + "\"", "", "Action");
                    LogEventos.EscribirLog("OnStart", "DB_USER = \"" + CONFIG.DB_USER + "\"", "", "Action");
                    LogEventos.EscribirLog("OnStart", "DB_PASS = \"" + CONFIG.DB_PASS + "\"", "", "Action");
                    LogEventos.EscribirLog("OnStart", "DB_NAME = \"" + CONFIG.DB_NAME + "\"", "", "Action");
                    LogEventos.EscribirLog("OnStart", "----------------------------------------------------", "", "Action");

                    FuncionesMMF.EscribirMMF(1);

                    hiloClienteTCP = new Thread(vg.ClienteCore.IniciarConexion);
                    hiloClienteTCP.Start();

                }                
            }
            catch (Exception e2)
            {
                ControlExcepciones.Exception("OnStart()", e2);
                LogEventos.EscribirLog("OnStart", "LeerMMF: ", "Error : " + e2.StackTrace , "Action");
            }
        }

        protected override void OnStop()
        {
            LogEventos.EscribirLog("OnStop", "Desconectando WindowsServiceBase", "", "Action");
        }
    }
}
