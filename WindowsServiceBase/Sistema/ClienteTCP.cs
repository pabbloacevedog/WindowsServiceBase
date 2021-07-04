using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using WindowsServiceBase.Objetos;
using WindowsServiceBase.Modelo;
using System.Threading.Tasks;

namespace WindowsServiceBase.Sistema
{
    public class ClienteTCP
    {
        //public Socket clienteSocket;
        IPAddress ipAddress;
        string port;

        public static Mutex mutexMensaje = new Mutex();
        public Mutex mutexMensajeOK = new Mutex();
        const char LF = '\n';
        const char STX = '\x0002'; //(char)0x02
        readonly string STXLog = "<STX>";
        readonly string LFLog = "<LF>";
        int estadoConexion = 0;
        bool reintentos = false;
        bool timerReintentos = false;
        bool estadoComunicacionTimer = false;
        bool timerLatenciaHB = false;
        bool conAuto = false;
        public static bool estadoSocket = false;
        public bool tBins = false;
        public System.Timers.Timer timerBins;
        public System.Timers.Timer estadoComunicacion;
        public System.Timers.Timer latenciaHB;
        public System.Timers.Timer reintentarConectar;
        public System.Timers.Timer timerRespuestaHB;
        public System.Timers.Timer tConexion;
        public ClienteTCP()
        {

        }
        public void IniciarConexion()
        {
            conAuto = true;
            tConexion = new System.Timers.Timer();
            tConexion.Interval = Convert.ToDouble(100);
            
            tConexion.AutoReset = true;
            tConexion.Elapsed += new System.Timers.ElapsedEventHandler(IniciarClienteSocket);
            tConexion.Start();
        }
        public void FinalizarInicioConexion()
        {
            conAuto = false;
            tConexion.Stop();
            tConexion.Dispose();
            tConexion.EndInit();
        }
        public void IniciarClienteSocket(object sender, System.Timers.ElapsedEventArgs e)
        {
            tConexion.Interval = Convert.ToDouble(CONFIG.RECONECTAR);
            try
            {
                // Establecer el EndPoint (Extremo) remoto para conectar el cliente.
                string ip = CONFIG.CLIENTE_TCP_IP;
                ipAddress = IPAddress.Parse(ip);
                port = CONFIG.CLIENTE_TCP_PORT;
                IPEndPoint remoteEP = new IPEndPoint(ipAddress, Convert.ToInt32(port));

                if (vg.coreSocket == null)
                {
                    // Crear un TCP/IP socket.
                    vg.coreSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                }
                // Conectar con el EndPoint (Extremo) remoto.
                try
                {
                    vg.coreSocket.BeginConnect(remoteEP, new AsyncCallback(ConnectCallback), vg.coreSocket);
                }
                catch (SocketException error)
                {
                    reintentos = false;
                    ControlExcepciones.SocketException("IniciarClienteSocket", error);
                }
            }
            catch (SocketException error)
            {
                ControlExcepciones.SocketException("IniciarClienteSocket", error);
            }
            catch (Exception error)
            {
                ControlExcepciones.Exception("IniciarClienteSocket", error);
            }
        }

        public void ConnectCallback(IAsyncResult ar)
        {
            try
            {
                // Recupera el socket desde el objeto de estados.
                Socket client = (Socket)ar.AsyncState;
                // Completa la conexión.
                client.EndConnect(ar);

                ObjetoSocket nuevoCliente = new ObjetoSocket
                {
                    IP = ipAddress.ToString(),
                    Puerto = port,
                    Estado = "Conectado"
                };

                LogEventos.EscribirLog("ClienteTCP - ConnectCallback", "Cliente iniciado con éxito", LogEventos.SerializarJSON(nuevoCliente), "Action");

                // Recibe el response desde el dispositivo remoto.
                try
                {
                    Receive(client);
                    FinalizarInicioConexion();
                }
                catch (SocketException error)
                {
                    ControlExcepciones.SocketException("ClienteTCP - ConnectCallback", error);
                }
                reintentos = true;
                if (timerReintentos)
                {
                    reintentarConectar.Stop();
                    reintentarConectar.Dispose();
                    //reintentarConectar.EndInit();
                }

                if (!timerLatenciaHB)
                {
                    conexionLatenciaHB();
                }
                if (!estadoComunicacionTimer)
                {
                    conexion();
                }
                estadoConexion = 2;
                FuncionesMMF.EscribirMMF(estadoConexion);
            }
            catch (SocketException error)
            {
                reintentos = false;
                estadoConexion = 0;
                FuncionesMMF.EscribirMMF(estadoConexion);
                ControlExcepciones.SocketException("ClienteTCP - ConnectCallback", error);
            }
            catch (Exception error)
            {
                ControlExcepciones.Exception("ClienteTCP - ConnectCallback", error);
            }
        }

        public void Receive(Socket client)
        {
            try
            {
                // Crea el objeto de estado.
                EstadoSocket state = new EstadoSocket
                {
                    workSocket = client
                };

                // Comienza a recibir los datos desde el dispositivo remoto.
                client.BeginReceive(state.buffer, 0, EstadoSocket.BufferSize, 0, new AsyncCallback(ReceiveCallback), state);
            }
            catch (SocketException error)
            {
                reintentos = false;
                estadoConexion = 0;
                FuncionesMMF.EscribirMMF(estadoConexion);
                ControlExcepciones.SocketException("ClienteTCP - Receive", error);
            }
            catch (Exception error)
            {
                ControlExcepciones.Exception("ClienteTCP - Receive", error);
            }
        }

        public void ReceiveCallback(IAsyncResult ar)
        {

            try
            {
                if (vg.coreSocket != null)
                {
                    // Recupera el objeto de estados y el socket del cliente desde el objeto de estado de conexión asíncrona.
                    EstadoSocket state = (EstadoSocket)ar.AsyncState;
                    Socket cliente = state.workSocket;

                    // Lee los datos desde el dispositivo remoto.
                    int bytesRead = cliente.EndReceive(ar);

                    if (bytesRead > 0)
                    {
                        //LogEventos.EscribirLog("ClienteTCP - ReceiveCallback", "Recibiendo " + bytesRead + " bytes del servidor " + cliente.RemoteEndPoint.ToString(), "", "Action");
                        // Es posible que hayan más datos que leer, entonces se almacenan los datos recibidos hasta el momento.
                        string response = Encoding.UTF8.GetString(state.buffer, 0, bytesRead);
                        if (mutexMensaje.WaitOne())
                        {
                            validarMensaje(response);
                            mutexMensaje.ReleaseMutex();
                        }
                        // Obtiene el resto de los datos.
                        cliente.BeginReceive(state.buffer, 0, EstadoSocket.BufferSize, 0, new AsyncCallback(ReceiveCallback), state);
                    }
                    else
                    {
                        cliente.BeginSend(state.buffer, 0, state.buffer.Length, 0, new AsyncCallback(SendCallback), cliente);
                        cliente.Close();
                        cliente.Dispose();
                        LogEventos.EscribirLog("ClienteTCP - ReceiveCallback", "Cliente solicita desconexión", "", "Action");
                    }
                }
                else
                {
                    LogEventos.EscribirLog("ClienteTCP - ReceiveCallback", "El socket no está conectado.", "", "Action");
                }
            }
            catch (SocketException error)
            {
                ControlExcepciones.SocketException("ClienteTCP - ReceiveCallback", error);
            }
            catch (Exception error)
            {
                ControlExcepciones.Exception("ClienteTCP - ReceiveCallback", error);
            }
        }

        public void Send(Socket client, string data)
        {
            // Convierte los datos string en byte usando la codificación ASCII
            byte[] byteData = Encoding.UTF8.GetBytes(data);
            try
            {
                // Comienza a enviar los datos al dispositivo remoto.
                client.BeginSend(byteData, 0, byteData.Length, 0, new AsyncCallback(SendCallback), client);
            }
            catch (SocketException error)
            {
                ControlExcepciones.SocketException("ClienteTCP - Send", error);
            }
            catch (Exception error)
            {
                ControlExcepciones.Exception("ClienteTCP - Send", error);
            }

        }
        public void SendCallback(IAsyncResult ar)
        {
            try
            {
                // Recupera el socket desde el objeto de estados
                Socket client = (Socket)ar.AsyncState;

                // Completa el envío de datos al dispositivo remoto.
                int bytesSent = client.EndSend(ar);

                //LogEventos.EscribirLog("ClienteTCP - SendCallback", "Enviando " + bytesSent + " bytes al servidor " + client.RemoteEndPoint.ToString(), "", "Action");

                // Señal de que todos los bytes han sido enviados.
                //sendDone.Set();
            }
            catch (SocketException error)
            {
                ControlExcepciones.SocketException("ClienteTCP - SendCallback", error);
            }
            catch (Exception error)
            {
                ControlExcepciones.Exception("ClienteTCP - SendCallback", error);
            }
        }
        public void validarMensaje(string str)
        {
            try
            {
                char[] charArray = str.ToCharArray();
                //str.Replace("     ", "-");
                int total = (from c in str where c == LF select c).Count();
                if (total > 1)
                {
                    int posicionActual = 0;
                    string mensajeSalida;
                    string resto;
                    int contador = 0;
                    while (contador < total)
                    {
                        if (charArray[0] == STX)
                        {
                            if (charArray[charArray.Length - 1] == LF)
                            {
                                int position = str.IndexOf(LF);
                                mensajeSalida = str.Substring(posicionActual, position + 1);
                                resto = str.Substring(position + 1);
                                str = resto;
                                string mensaje = mensajeSalida.Remove(0, 1);
                                mensaje = mensaje.Remove(mensaje.Length - 1, 1);
                                if (mensaje == "HB")
                                {
                                    if (estadoComunicacionTimer)
                                    {
                                        //detiene el timer que valida la comunicacion
                                        estadoComunicacion.Stop();
                                        estadoComunicacion.Dispose();
                                        //estadoComunicacion.EndInit();
                                        estadoComunicacionTimer = false;
                                    }
                                    if (timerLatenciaHB)
                                    {
                                        //detiene el timer que valida la comunicacion
                                        latenciaHB.Stop();
                                        latenciaHB.Dispose();
                                        //latenciaHB.EndInit();
                                        timerLatenciaHB = false;
                                    }
                                    Send(vg.coreSocket, STX + "AB" + LF);
                                    LogEventos.EscribirLog("ClienteTCP - ValidarMensajeRecibido", "R: " + STXLog + "HB" + LFLog, "", "HB");
                                    LogEventos.EscribirLog("ClienteTCP - ValidarMensajeRecibido", "W: " + STXLog + "AB" + LFLog, "", "HB");
                                    estadoConexion = 1;
                                    FuncionesMMF.EscribirMMF(estadoConexion);
                                    if (!timerLatenciaHB)
                                    {
                                        conexionLatenciaHB();
                                    }
                                    if (!estadoComunicacionTimer)
                                    {
                                        conexion();
                                    }
                                }
                                else
                                {
                                    if (mutexMensajeOK.WaitOne())
                                    {
                                        MensajeOK(mensaje);
                                        LogEventos.EscribirLog("ClienteTCP - ValidarMensajeRecibido", "R: " + STXLog + mensaje + LFLog, "", "HB");
                                        mutexMensajeOK.ReleaseMutex();
                                    }

                                }

                            }
                            else
                            {
                                LogEventos.EscribirLog("ClienteTCP - validarMensaje", "MENSAJE INCOMPLETO, no existe el caracter de control LF como término de mensaje. R: " + str, "", "Action");
                            }

                        }
                        else
                        {
                            LogEventos.EscribirLog("ClienteTCP - validarMensaje", "MENSAJE INCOMPLETO, no existe el caracter de control STX como inicio de mensaje. R: " + str, "", "Action");
                        }
                        contador++;
                    }
                }
                else
                {
                    if (str.Length == 4)
                    {
                        if (charArray[0] == STX)
                        {
                            if (charArray[charArray.Length - 1] == LF)
                            {
                                if (str == STX + "HB" + LF)
                                {
                                    if (estadoComunicacionTimer)
                                    {
                                        //detiene el timer que valida la comunicacion
                                        estadoComunicacion.Stop();
                                        estadoComunicacion.Dispose();
                                        //estadoComunicacion.EndInit();
                                        estadoComunicacionTimer = false;
                                    }
                                    if (timerLatenciaHB)
                                    {
                                        //detiene el timer que valida la comunicacion
                                        latenciaHB.Stop();
                                        latenciaHB.Dispose();
                                        //latenciaHB.EndInit();
                                        timerLatenciaHB = false;
                                    }
                                    Send(vg.coreSocket, STX + "AB" + LF);
                                    LogEventos.EscribirLog("ClienteTCP - ValidarMensajeRecibido", "R: " + STXLog + "HB" + LFLog, "", "HB");
                                    LogEventos.EscribirLog("ClienteTCP - ValidarMensajeRecibido", "W: " + STXLog + "AB" + LFLog, "", "HB");
                                    estadoConexion = 1;
                                    FuncionesMMF.EscribirMMF(estadoConexion);
                                    if (!timerLatenciaHB)
                                    {
                                        conexionLatenciaHB();
                                    }
                                    if (!estadoComunicacionTimer)
                                    {
                                        conexion();
                                    }
                                }
                                else
                                {
                                    if (mutexMensajeOK.WaitOne())
                                    {
                                        string objeto = str.Remove(0, 1);
                                        objeto = objeto.Remove(objeto.Length - 1, 1);
                                        MensajeOK(objeto);
                                        LogEventos.EscribirLog("ClienteTCP - ValidarMensajeRecibido", "R: " + STXLog + objeto + LFLog, "", "HB");
                                        mutexMensajeOK.ReleaseMutex();
                                    }

                                }
                            }
                            else
                            {
                                LogEventos.EscribirLog("ClienteTCP - validarMensaje", "MENSAJE INCOMPLETO, no existe el caracter de control LF como término de mensaje. R: " + str, "", "Action");
                            }
                        }
                        else
                        {
                            LogEventos.EscribirLog("ClienteTCP - validarMensaje", "MENSAJE INCOMPLETO, no existe el caracter de control STX como inicio de mensaje. R: " + str, "", "Action");
                        }
                    }
                    else
                    {
                        if (charArray[0] == STX)
                        {
                            if (charArray[charArray.Length - 1] == LF)
                            {
                                if (str == STX + "HB" + LF)
                                {
                                    if (estadoComunicacionTimer)
                                    {
                                        //detiene el timer que valida la comunicacion
                                        estadoComunicacion.Stop();
                                        estadoComunicacion.Dispose();
                                        //estadoComunicacion.EndInit();
                                        estadoComunicacionTimer = false;
                                    }
                                    if (timerLatenciaHB)
                                    {
                                        //detiene el timer que valida la comunicacion
                                        latenciaHB.Stop();
                                        latenciaHB.Dispose();
                                        //latenciaHB.EndInit();
                                        timerLatenciaHB = false;
                                    }
                                    Send(vg.coreSocket, STX + "AB" + LF);
                                    LogEventos.EscribirLog("ClienteTCP - ValidarMensajeRecibido", "R: " + STXLog + "HB" + LFLog, "", "HB");
                                    LogEventos.EscribirLog("ClienteTCP - ValidarMensajeRecibido", "W: " + STXLog + "AB" + LFLog, "", "HB");
                                    estadoConexion = 1;
                                    FuncionesMMF.EscribirMMF(estadoConexion);
                                    if (!timerLatenciaHB)
                                    {
                                        conexionLatenciaHB();
                                    }
                                    if (!estadoComunicacionTimer)
                                    {
                                        conexion();
                                    }
                                }
                                else
                                {
                                    if (mutexMensajeOK.WaitOne())
                                    {
                                        string objeto = str.Remove(0, 1);
                                        objeto = objeto.Remove(objeto.Length - 1, 1);
                                        MensajeOK(objeto);
                                        LogEventos.EscribirLog("ClienteTCP - ValidarMensajeRecibido", "R: " + STXLog + objeto + LFLog, "", "HB");
                                        mutexMensajeOK.ReleaseMutex();
                                    }

                                }
                            }
                            else
                            {
                                LogEventos.EscribirLog("ClienteTCP - validarMensaje", "MENSAJE INCOMPLETO, no existe el caracter de control LF como término de mensaje. R: " + str, "", "Action");
                            }
                        }
                        else
                        {
                            LogEventos.EscribirLog("ClienteTCP - validarMensaje", "MENSAJE INCOMPLETO, no existe el caracter de control STX como inicio de mensaje. R: " + str, "", "Action");
                        }
                    }
                }
            }
            catch (Exception e2)
            {
                ControlExcepciones.Exception("ClienteTCP - ValidarMensajeRecibido", e2);
            }
        }
        public void MensajeOK(string objeto)
        {
            LogEventos.EscribirLog("ClienteTCP - validarMensaje", "Mensaje reconocido. R: " + objeto, "", "Action");
            //en esta función se debe incluir logica para el mensaje recibido
        }
        public void conexionLatenciaHB()
        {
            timerLatenciaHB = true;
            latenciaHB = new System.Timers.Timer();
            latenciaHB.Interval = Convert.ToDouble(CONFIG.TIMER_LATENCIA_HB);
            latenciaHB.AutoReset = true;
            latenciaHB.Elapsed += new System.Timers.ElapsedEventHandler(informarLatenciaHB);
            latenciaHB.Start();
        }
        public void informarLatenciaHB(object sender, System.Timers.ElapsedEventArgs e)
        {
            //LogEventos.EscribirLog("informarLatenciaHB", "HeartBeat se esta demorando más de " + Convert.ToInt32(CONFIG.TIMER_LATENCIA_HB) / 1000 + " segundos", "", "Action");
        }

        public void conexion()
        {
            estadoComunicacionTimer = true;
            estadoComunicacion = new System.Timers.Timer();
            estadoComunicacion.Interval = Convert.ToDouble(CONFIG.TIMER_COMUNICACION);
            estadoComunicacion.AutoReset = false;
            estadoComunicacion.Elapsed += new System.Timers.ElapsedEventHandler(bajarConexion);
            estadoComunicacion.Start();
        }
        public void bajarConexion(object sender, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                LogEventos.EscribirLog("bajarConexion", "Han pasado " + Convert.ToInt32(CONFIG.TIMER_COMUNICACION) / 1000 + " segundos, y no hay HB, desconectar cliente durante " + Convert.ToInt32(CONFIG.BAJAR_CONEXION) / 1000 + " segundos", "", "Action");
                int time = int.Parse(CONFIG.BAJAR_CONEXION);
                reintentos = false;
                estadoConexion = 0;
                FuncionesMMF.EscribirMMF(estadoConexion);
                Desconectar();

                if (estadoComunicacionTimer)
                {
                    //detiene el timer que valida la comunicacion
                    estadoComunicacion.Stop();
                    estadoComunicacion.Dispose();
                    estadoComunicacionTimer = false;
                }
                if (timerLatenciaHB)
                {
                    //detiene el timer que valida la comunicacion
                    latenciaHB.Stop();
                    latenciaHB.Dispose();
                    timerLatenciaHB = false;
                }
                Thread.Sleep(time);
                if (!reintentos)
                {
                    reconectar();
                }
                else
                {
                    timerReintentos = false;
                }
            }
            catch (Exception e2)
            {
                ControlExcepciones.Exception("SendCallback", e2);
            }

        }
        public void reconectar()
        {
            reintentarConectar = new System.Timers.Timer();
            reintentarConectar.Interval = Convert.ToDouble(CONFIG.REINTENTOS);
            reintentarConectar.AutoReset = true;
            reintentarConectar.Elapsed += new System.Timers.ElapsedEventHandler(reintentarConexion);
            reintentarConectar.Start();
        }
        public void reintentarConexion(object sender, System.Timers.ElapsedEventArgs e)
        {
            timerReintentos = true;
            LogEventos.EscribirLog("reintentarConexion()", "Reconectando...", "", "Action");
            if (!conAuto)
            {
                IniciarConexion();
            }

        }
        public bool ValidarConexion()
        {
            if (vg.coreSocket == null)
            {
                return false;
            }
            else
            {
                //true cuando esta conectado
                bool conectado = vg.coreSocket.Connected;
                if (!conectado)
                {
                    //client = null;
                    return false;
                }
                else
                {
                    if (!IsConnected())
                    {
                        //client = null;
                        estadoConexion = 0;
                        FuncionesMMF.EscribirMMF(estadoConexion);
                        return false;
                    }
                    else
                    {
                        //solo publicara el ultimo estado de la variable estadoConexion
                        FuncionesMMF.EscribirMMF(estadoConexion);
                        return true;
                    }
                }
            }
        }
        public bool IsConnected()
        {
            //es false cuando tiene conexion
            bool estaDesconectado = vg.coreSocket.Poll(1000, SelectMode.SelectRead);
            //es true cuando es != 0 (tiene datos que leer)
            bool tieneDatosParaLeer = (vg.coreSocket.Available == 0);
            if (!estaDesconectado && tieneDatosParaLeer)
                return true;
            else
                return false;
        }
        public void Desconectar()
        {
            try
            {
                LogEventos.EscribirLog("Desconectar()", "Desconectando el Cliente", "", "Action");
                estadoConexion = 0;
                FuncionesMMF.EscribirMMF(estadoConexion);
                vg.coreSocket.Shutdown(SocketShutdown.Both);
                vg.coreSocket = null;

            }
            catch (SocketException e)
            {
                ControlExcepciones.SocketException("Desconectar", e);
            }
            catch (Exception e2)
            {
                ControlExcepciones.Exception("Desconectar", e2);
            }
        }
        ~ClienteTCP()
        {
            mutexMensaje.Dispose();
            mutexMensajeOK.Dispose();
        }
    }
}
