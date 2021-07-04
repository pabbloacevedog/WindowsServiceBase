using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Web.Script.Serialization;
using WindowsServiceBase.Objetos;

namespace WindowsServiceBase.Sistema
{
    public class LogEventos
    {
        public static Mutex mutexEscribir = new Mutex();

        public static void EscribirLog(string modulo, string mensaje, string json, string destinoLog)
        {
            if (mutexEscribir.WaitOne())
            {
                try
                {
                    if (!Directory.Exists(@"" + CONFIG.DISCO_ORIGEN + "SERVICES"))
                    {
                        Directory.CreateDirectory(@"" + CONFIG.DISCO_ORIGEN + "SERVICES");
                    }

                    if (!Directory.Exists(@"" + CONFIG.DISCO_ORIGEN + "SERVICES/LOGS"))
                    {
                        Directory.CreateDirectory(@"" + CONFIG.DISCO_ORIGEN + "SERVICES/LOGS");
                    }

                    if (!Directory.Exists(@"" + CONFIG.DISCO_ORIGEN + "SERVICES/LOGS/" + CONFIG.NOMBRE_SERVICIO))
                    {
                        Directory.CreateDirectory(@"" + CONFIG.DISCO_ORIGEN + "SERVICES/LOGS/" + CONFIG.NOMBRE_SERVICIO);
                    }

                    string rutaLog = "";
                    string pathLog = "";

                    if (destinoLog == "HB")
                    {
                        rutaLog = CONFIG.PATH_LOG_HB;
                        pathLog = rutaLog.Replace("DDMMYYYY", DateTime.Now.ToString("ddMMyyyy"));
                    }
                    else if (destinoLog == "Action")
                    {
                        rutaLog = CONFIG.PATH_LOG_ACTION;
                        pathLog = rutaLog.Replace("DDMMYYYY", DateTime.Now.ToString("ddMMyyyy"));
                    }
                    else
                    {
                        rutaLog = CONFIG.PATH_LOG_ACTION;
                        pathLog = rutaLog.Replace("DDMMYYYY", DateTime.Now.ToString("ddMMyyyy"));
                    }

                    // Si no existe el archivo lo crea, si no, escribe en el 
                    if (!File.Exists(pathLog))
                    {
                        using (StreamWriter sw = File.CreateText(pathLog))
                        {
                            if (destinoLog == "HB")
                            {
                                sw.WriteLine(DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss.fff") + "; " + mensaje);
                                sw.Close();
                            }
                            else
                            {
                                sw.WriteLine(DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss.fff") + "; " + modulo + "; " + mensaje + "; " + json);
                                sw.Close();
                            }
                        }                            
                    }
                    else
                    {
                        using (StreamWriter sw = File.AppendText(pathLog))
                        {
                            if (destinoLog == "HB")
                            {
                                sw.WriteLine(DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss.fff") + "; " + mensaje);
                                sw.Close();
                            }
                            else
                            {
                                sw.WriteLine(DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss.fff") + "; " + modulo + "; " + mensaje + "; " + json);
                                sw.Close();
                            }
                        }                            
                    }

                }
                catch (IOException error)
                {
                    ControlExcepciones.IOException("EscribirLog", error);
                }
                mutexEscribir.ReleaseMutex();
            }
        }

        public static string SerializarJSON(object obj)
        {
            string json = "";
            try
            {
                List<object> lista = new List<object> { obj };

                JavaScriptSerializer jsonserializer = new JavaScriptSerializer();
                json = jsonserializer.Serialize(lista);
            }
            catch (Exception error)
            {
                ControlExcepciones.Exception("SerializarJSON", error);
            }
            return json;
        }
        public static object DeserializarJSON(string tipoObjeto, string data)
        {
            object resultado = null;
            try
            {
                switch (tipoObjeto)
                {
                    case "IMPRIMIR":
                        JavaScriptSerializer jsonserializer = new JavaScriptSerializer();
                        resultado = jsonserializer.Deserialize<ObjetoImprimir>(data);
                        break;
                }
            }
            catch (Exception error)
            {
                ControlExcepciones.Exception("DeserializarJSON", error);
            }
            return resultado;
        }
        ~LogEventos()
        {
            mutexEscribir.Dispose();
        }
    }
}
