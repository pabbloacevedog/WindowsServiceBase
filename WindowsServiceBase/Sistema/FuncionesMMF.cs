using System;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Threading;
using WindowsServiceBase.Objetos;

namespace WindowsServiceBase.Sistema
{
    public class FuncionesMMF
    {
        public static bool bandera = false;
        public static char[] datosFecha = new char[19];
        public static bool creado = false;
        public static string nombreMutex = CONFIG.NOMBRE_MUTEX;
        public static string mutexId = string.Format("Global\\{{{0}}}", nombreMutex);
        private static readonly object thisLock = new object();
        public static string archivo = CONFIG.ARCHIVO_MMF;
        public static Mutex mutexEscritura;

        public static void EscribirMMF(int estado)
        {
            int posicionEstado = 0;
            //if (scanner == 1)
            //{
            //    posicionEstado = Convert.ToInt32(CONFIG.POSICION_ESTADO_SCANNER1);
            //}
            //else if (scanner == 2)
            //{
            //    posicionEstado = Convert.ToInt32(CONFIG.POSICION_ESTADO_SCANNER2);
            //}
            //else if (scanner == 3)
            //{
            //    posicionEstado = Convert.ToInt32(CONFIG.POSICION_ESTADO_SCANNER3);
            //}
            //else
            //{
            //    posicionEstado = Convert.ToInt32(CONFIG.POSICION_ESTADO);
            //}
            posicionEstado = Convert.ToInt32(CONFIG.POSICION_ESTADO);
            int tamañoBufer = 1024;
            string nombreBuffer = CONFIG.NOMBRE_MMF;
            try
            {
                using (mutexEscritura = Mutex.OpenExisting(mutexId))
                {
                    try
                    {
                        using (MemoryMappedFile mmf = MemoryMappedFile.OpenExisting(nombreBuffer))
                        {
                            try
                            {
                                EscribirMemoria(mmf, posicionEstado, estado);
                            }
                            catch (Exception error)
                            {
                                ControlExcepciones.Exception("Publicar", error);
                            }
                            mmf.Dispose();
                        }
                    }
                    catch (FileNotFoundException)
                    {
                        try
                        {
                            using (MemoryMappedFile mmf = MemoryMappedFile.CreateFromFile(@"" + archivo, FileMode.Open, nombreBuffer, tamañoBufer))
                            {
                                try
                                {
                                    EscribirMemoria(mmf, posicionEstado, estado);
                                }
                                catch (Exception error)
                                {
                                    ControlExcepciones.Exception("Publicar", error);
                                }
                                mmf.Dispose();
                            }
                        }
                        catch (FileNotFoundException)
                        {
                            LogEventos.EscribirLog("Publicar", "El archivo (" + archivo + ") no se encuentra en la ruta indicada, por favor revise la caperta MMF", "", "Action");
                        }
                        catch (IOException)
                        {
                        }
                        catch (Exception error)
                        {
                            ControlExcepciones.Exception("Publicar", error);
                        }
                    }
                }
            }
            catch (WaitHandleCannotBeOpenedException)
            {
                using (mutexEscritura = new Mutex(true, mutexId, out bool mutexCreated))
                {
                    try
                    {
                        using (MemoryMappedFile mmf = MemoryMappedFile.OpenExisting(nombreBuffer))
                        {
                            try
                            {
                                EscribirMemoria(mmf, posicionEstado, estado);
                            }
                            catch (Exception error)
                            {
                                ControlExcepciones.Exception("Publicar", error);
                            }
                            mmf.Dispose();
                        }
                    }
                    catch (FileNotFoundException)
                    {
                        try
                        {
                            using (MemoryMappedFile mmf = MemoryMappedFile.CreateFromFile(@"" + archivo, FileMode.Open, nombreBuffer, tamañoBufer))
                            {
                                try
                                {
                                    EscribirMemoria(mmf, posicionEstado, estado);
                                }
                                catch (Exception error)
                                {
                                    ControlExcepciones.Exception("Publicar", error);
                                }
                                mmf.Dispose();
                            }
                        }
                        catch (FileNotFoundException)
                        {
                            LogEventos.EscribirLog("Publicar", "El archivo (" + archivo + ") no se encuentra en la ruta indicada, por favor revise la caperta MMF", "", "Action");
                        }
                        catch (IOException)
                        {
                        }
                        catch (Exception error)
                        {
                            ControlExcepciones.Exception("Publicar", error);
                        }
                    }
                }
            }
            catch (Exception error)
            {
                ControlExcepciones.Exception("Publicar", error);
            }
        }

        public static void EscribirMemoria(MemoryMappedFile mmf, int posicionEstado, Int64 estado)
        {
            //LogEventos.EscribirLog("Publicar", "MMF = " + archivo + "; POSICION = " + posicionEstado + "; ESTADO = " + estado, "", "Action");
            lock (thisLock)
            {
                using (MemoryMappedViewAccessor ACCESOR = mmf.CreateViewAccessor())
                {
                    DateTime localDate = DateTime.Now;
                    string fechaS = localDate.ToString();
                    char[] fecha = fechaS.ToCharArray();
                    int largoFecha = fecha.Length;
                    string arregloFecha = new string(fecha);
                    ACCESOR.Write(posicionEstado, estado);
                    int posicionFecha = Convert.ToInt32(CONFIG.POSICION_FECHA);
                    ACCESOR.WriteArray(posicionFecha, fecha, 0, largoFecha);
                }
            }
        }

        public static ObjetoEstadosMMF estado = new ObjetoEstadosMMF();
        public static Mutex mutexLectura;

        public static ObjetoEstadosMMF LeerMMF()
        {
            string nombreBuffer = CONFIG.NOMBRE_MMF;
            string nombreMutex = CONFIG.NOMBRE_MUTEX;
            string archivoMMF = CONFIG.ARCHIVO_MMF;
            string mutexId = string.Format("Global\\{{{0}}}", nombreMutex);

            try
            {
                using (mutexLectura = Mutex.OpenExisting(mutexId))
                {
                    try
                    {
                        using (MemoryMappedFile mmf = MemoryMappedFile.OpenExisting(nombreBuffer))
                        {
                            try
                            {
                                estado = LeerMemoria(mmf);
                            }
                            catch (Exception error)
                            {
                                ControlExcepciones.Exception("ObtenerEstado", error);
                            }
                        }
                    }
                    catch (FileNotFoundException)
                    {
                        try
                        {
                            using (MemoryMappedFile mmf = MemoryMappedFile.CreateFromFile(@"" + archivoMMF, FileMode.Open, nombreBuffer))
                            {
                                try
                                {
                                    estado = LeerMemoria(mmf);
                                }
                                catch (Exception error)
                                {
                                    ControlExcepciones.Exception("ObtenerEstado", error);
                                }
                            }
                        }
                        catch (FileNotFoundException)
                        {
                            LogEventos.EscribirLog("ObtenerEstado", "El archivo (" + archivoMMF + ") no se encuentra en la ruta indicada, por favor revise la caperta MMF", "", "Action");
                        }
                        catch (IOException)
                        {
                            //mutex.ReleaseMutex();
                        }
                        catch (Exception error)
                        {
                            ControlExcepciones.Exception("ObtenerEstado", error);
                        }
                    }
                }
            }
            catch (WaitHandleCannotBeOpenedException)
            {
                using (mutexLectura = new Mutex(true, mutexId, out bool mutexCreated))
                {
                    try
                    {
                        using (MemoryMappedFile mmf = MemoryMappedFile.OpenExisting(nombreBuffer))
                        {
                            //crea el nombre del proceso que accede al espacio en memoria
                            try
                            {
                                estado = LeerMemoria(mmf);
                            }
                            catch (Exception error)
                            {
                                ControlExcepciones.Exception("ObtenerEstado", error);
                            }
                        }
                    }
                    catch (FileNotFoundException)
                    {
                        try
                        {
                            using (MemoryMappedFile mmf = MemoryMappedFile.CreateFromFile(@"" + archivoMMF, FileMode.Open, nombreBuffer))
                            {
                                try
                                {
                                    estado = LeerMemoria(mmf);
                                }
                                catch (Exception error)
                                {
                                    ControlExcepciones.Exception("ObtenerEstado", error);
                                }
                            }
                        }
                        catch (FileNotFoundException)
                        {
                            LogEventos.EscribirLog("ObtenerEstado", "El archivo (" + archivoMMF + ") no se encuentra en la ruta indicada, por favor revise la caperta MMF", "", "Action");
                        }
                        catch (IOException)
                        {
                        }
                        catch (Exception error)
                        {
                            ControlExcepciones.Exception("ObtenerEstado", error);
                        }
                    }
                }
            }
            catch (Exception error)
            {
                ControlExcepciones.Exception("ObtenerEstado", error);
            }

            return estado;
        }

        public static ObjetoEstadosMMF LeerMemoria(MemoryMappedFile mmf)
        {
            using (MemoryMappedViewAccessor procesoLectura = mmf.CreateViewAccessor())
            {
                char[] datosFecha = new char[19];
                procesoLectura.ReadArray(Convert.ToInt32(CONFIG.POSICION_FECHA), datosFecha, 0, datosFecha.Length);
                string arregloFecha = new string(datosFecha);

                ObjetoEstadosMMF resultado = new ObjetoEstadosMMF
                {
                    estado = procesoLectura.ReadInt32(Convert.ToInt32(CONFIG.POSICION_ESTADO)),
                    fecha = arregloFecha
                };

                LogEventos.EscribirLog("LeerMemoria", "Se han leído los siguientes datos MMF", LogEventos.SerializarJSON(resultado), "Action");

                return resultado;
            }
        }
        ~FuncionesMMF()
        {
            mutexEscritura.Dispose();
        }
    }
}
