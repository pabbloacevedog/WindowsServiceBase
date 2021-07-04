using WindowsServiceBase.Objetos;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace WindowsServiceBase.Sistema
{
    public class CONFIG
    {
        //Sección LOG
        public static string DISCO_ORIGEN;
        public static string PATH_LOG_ACTION;
        public static string PATH_LOG_HB;
        public static string NOMBRE_SERVICIO;

        //Sección SOCKET
        public static string TIMER_HB;
        public static string CLIENTE_TCP_IP;
        public static string CLIENTE_TCP_PORT;
        public static string RECONECTAR;
        public static string TIMEOUT;
        public static string TIMER_COMUNICACION;
        public static string TIMER_LATENCIA_HB;
        public static string TIMER_SIN_RESPUESTA;
        public static string REINTENTOS;
        public static string BAJAR_CONEXION;

        //Sección MMF
        public static string ARCHIVO_MMF;
        public static string NOMBRE_MMF;
        public static string NOMBRE_MUTEX;
        public static string POSICION_ESTADO;
        public static string POSICION_FECHA;


        //Sección DATABASE
        public static string DB_SERVER;
        public static string DB_USER;
        public static string DB_PASS;
        public static string DB_NAME;

        //Sección ZPL
        public static string RUTA_ZPL_CAJA;
        

        public static string MB_IP;
        public static string MB_PORT;
        public static string MB_REINTENTOS;
        public static string MB_TIMEOUT;
        public static string MB_SLAVE;
        public static string MB_INTERVALO_TIMER;
        public static string MB_LECTURA;
        public static string MB_ESCRITURA;


        public static string ID_SERVICIO;

        public static bool CrearCONFIG()
        {
            try
            {
                Process[] servicio = Process.GetProcessesByName(Process.GetCurrentProcess().ProcessName);
                vg.ProcessName = servicio[0].ProcessName;

                string path_install = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
                //string directorioSERVICES = "C:\\SERVICES";
                string directorioSERVICES = ObtenerRutaConfig(path_install, "SERVICES");
                if (!string.IsNullOrWhiteSpace(directorioSERVICES))
                {                    
                    if (!File.Exists(@"" + directorioSERVICES + "/CONFIG/" + vg.ProcessName + ".ini"))
                    {
                        //Si no se encuentra el archivo, setear la ruta manualmente para escribir un mensaje de alerta.
                        PATH_LOG_ACTION = "E:/SERVICES/LOGS/" + vg.ProcessName + "/LOG-" + vg.ProcessName + "-DDMMYYYY.log";
                        LogEventos.EscribirLog("CrearCONFIG", "No se ha encontrado el archvo ini en la ruta especificada", "", "Action");
                        return false;
                    }
                    else
                    {
                        IniFile seccionLOG = new IniFile(@"" + directorioSERVICES + "/CONFIG/" + vg.ProcessName + ".ini", "LOG");
                        DISCO_ORIGEN = seccionLOG.Read("DISCO_ORIGEN");
                        PATH_LOG_ACTION = seccionLOG.Read("PATH_LOG_ACTION");
                        PATH_LOG_HB = seccionLOG.Read("PATH_LOG_HB");
                        NOMBRE_SERVICIO = seccionLOG.Read("NOMBRE_SERVICIO");

                        IniFile seccionSOCKET = new IniFile(@"" + directorioSERVICES + "/CONFIG/" + vg.ProcessName + ".ini", "SOCKET");
                        TIMER_HB = seccionSOCKET.Read("TIMER_HB");
                        CLIENTE_TCP_IP = seccionSOCKET.Read("CLIENTE_TCP_IP");
                        CLIENTE_TCP_PORT = seccionSOCKET.Read("CLIENTE_TCP_PORT");
                        RECONECTAR = seccionSOCKET.Read("RECONECTAR");
                        TIMEOUT = seccionSOCKET.Read("TIMEOUT");
                        TIMER_COMUNICACION = seccionSOCKET.Read("TIMER_COMUNICACION");
                        TIMER_LATENCIA_HB = seccionSOCKET.Read("TIMER_LATENCIA_HB");
                        TIMER_SIN_RESPUESTA = seccionSOCKET.Read("TIMER_SIN_RESPUESTA");
                        REINTENTOS = seccionSOCKET.Read("REINTENTOS");
                        BAJAR_CONEXION = seccionSOCKET.Read("BAJAR_CONEXION");

                        IniFile seccionMMF = new IniFile(@"" + directorioSERVICES + "/CONFIG/" + vg.ProcessName + ".ini", "MMF");
                        ARCHIVO_MMF = seccionMMF.Read("ARCHIVO_MMF");
                        NOMBRE_MMF = seccionMMF.Read("NOMBRE_MMF");
                        NOMBRE_MUTEX = seccionMMF.Read("NOMBRE_MUTEX");
                        POSICION_ESTADO = seccionMMF.Read("POSICION_ESTADO");
                        POSICION_FECHA = seccionMMF.Read("POSICION_FECHA");


                        IniFile seccionDATABASE = new IniFile(@"" + directorioSERVICES + "/CONFIG/" + vg.ProcessName + ".ini", "DATABASE");
                        DB_SERVER = seccionDATABASE.Read("DB_SERVER");
                        DB_USER = seccionDATABASE.Read("DB_USER");
                        DB_PASS = seccionDATABASE.Read("DB_PASS");
                        DB_NAME = seccionDATABASE.Read("DB_NAME");


                        IniFile seccionEtiqueta = new IniFile(@"" + directorioSERVICES + "/CONFIG/" + vg.ProcessName + ".ini", "ZPL");
                        RUTA_ZPL_CAJA = seccionEtiqueta.Read("RUTA_ZPL_CAJA");

                        IniFile seccionMODBUS = new IniFile(@"" + directorioSERVICES + "/CONFIG/" + vg.ProcessName + ".ini", "MODBUS");
                        MB_IP = seccionMODBUS.Read("MB_IP");
                        MB_PORT = seccionMODBUS.Read("MB_PORT");
                        MB_SLAVE = seccionMODBUS.Read("MB_SLAVE");
                        MB_TIMEOUT = seccionMODBUS.Read("MB_TIMEOUT");
                        MB_INTERVALO_TIMER = seccionMODBUS.Read("MB_INTERVALO_TIMER");
                        MB_REINTENTOS = seccionMODBUS.Read("MB_REINTENTOS");
                        MB_LECTURA = seccionMODBUS.Read("MB_LECTURA");
                        MB_ESCRITURA = seccionMODBUS.Read("MB_ESCRITURA");

                        return true;
                    }
                }
                else
                {
                    return false;
                }
            }
            catch (Exception error)
            {
                ControlExcepciones.Exception("CrearCONFIG", error);
                return false;
            }
        }
        public static string ObtenerRutaConfig(string path, string parentName)
        {
            string resultado = "";
            try
            {
                var dir = new DirectoryInfo(path);
                if (dir.Parent.Name == parentName)
                    resultado = dir.Parent.FullName;
                else
                    resultado = ObtenerRutaConfig(dir.Parent.FullName, parentName);
            }
            catch (Exception error)
            {
                ControlExcepciones.Exception("ObtenerRutaConfig", error);
            }
            return resultado;
        }

    }
}
