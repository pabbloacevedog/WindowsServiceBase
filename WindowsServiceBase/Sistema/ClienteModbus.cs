using System;
using FieldTalk.Modbus.Master;
using WindowsServiceBase.Modelo;
using System.Threading;
using WindowsServiceBase.Objetos;
using System.Linq;

namespace WindowsServiceBase.Sistema
{
    public class ClienteModbus
    {
        private  MbusTcpMasterProtocol Modbus = new MbusTcpMasterProtocol();
        //private  MbusMasterFunctions Modbus;
        public System.Timers.Timer monitor;
        public System.Timers.Timer reconexion;
        public  bool tieneConexion = false;
        public  bool conexion = false;
        public readonly char cero = '0';
        public const char LF = '\n';
        public const char STX = '\x0002'; //(char)0x02
        public bool monitoreando = false;
        


        public  int estadoConexion = 0;

        public int slave = 1;
        public  void Conectar()
        {
            //Modbus.configureCountFromZero();
            int puerto = int.Parse(CONFIG.MB_PORT);
            int reintentos = int.Parse(CONFIG.MB_REINTENTOS);
            int timeOut = int.Parse(CONFIG.MB_TIMEOUT);
            string ip = CONFIG.MB_IP;

            if ((Modbus == null))
            {
                try
                {
                    Modbus = new MbusTcpMasterProtocol();
                }
                catch (OutOfMemoryException ex)
                {
                    LogEventos.EscribirLog("Cliente Modbus - Conectar()", " No se ha podido crear una instancia de la clase protocolo Ethernet" + ex.Message, "", "Action");
                   
                    estadoConexion = 0;
                    FuncionesMMF.EscribirMMF(estadoConexion);
                    return;
                }
                catch (Exception e2)
                {
                    
                    ControlExcepciones.Exception("Cliente Modbus - Conectar", e2);
                }
            }
            else // already instantiated, close protocol and reinstantiate
            {
                if (Modbus.isOpen())
                    Modbus.closeProtocol();
                Modbus = null;
                try
                {
                    Modbus = new MbusTcpMasterProtocol();

                }
                catch (OutOfMemoryException ex)
                {
                    LogEventos.EscribirLog("Cliente Modbus - Conectar()", " No se ha podido crear una instancia de la clase protocolo Ethernet" + ex.Message, "","Action");
                    estadoConexion = 0;
                    FuncionesMMF.EscribirMMF(estadoConexion);
                    return;


                }
                catch (Exception e2)
                {
                    ControlExcepciones.Exception("Cliente Modbus - Conectar", e2);
                }
            }
            int res;
            Modbus.timeout = timeOut;
            Modbus.retryCnt = reintentos;
            // Note: The following cast is required as the Modbus object is declared
            // as the superclass of MbusTcpMasterProtocol. That way Modbus can
            // represent different protocol types.


            Modbus.port = (short)puerto;
            res = Modbus.openProtocol(ip);
            if ((res == BusProtocolErrors.FTALK_SUCCESS))
            {
                Modbus.configureCountFromZero();
                if (tieneConexion)
                {
                    reconexion.Stop();
                    reconexion.Dispose();
                    reconexion.EndInit();
                    tieneConexion = false;
                }
                LogEventos.EscribirLog("Cliente Modbus - Conectar()", " Modbus/TCP conectado con la IP:" + ip + " y el puerto " + puerto + " PLC1", "","Action");
                estadoConexion = 1;
                FuncionesMMF.EscribirMMF(estadoConexion);
                conexion = true;
                if (!monitoreando)
                {
                    IniciarTimerRegistros();
                    monitoreando = true;
                }
            }
            else
            {
                conexion = false;
                LogEventos.EscribirLog("Cliente Modbus - Conectar()", " No se ha podido conectar: " + BusProtocolErrors.getBusProtocolErrorText(res) + " PLC1", "","Action");
                if (tieneConexion)
                {
                    reconexion.Stop();
                    reconexion.Dispose();
                    reconexion.EndInit();
                    tieneConexion = false;
                }
                desconectado();
                estadoConexion = 0;
                FuncionesMMF.EscribirMMF(estadoConexion);
            }
        }
        public  void retardo()
        {
            Thread.Sleep(5000);
        }
        public  void Monitorear(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (vg.mutexMonitor.WaitOne(1000))
            {
                monitoreando = true;
                int totalPosiciones = 1;
                //Monitoreando READBARCODE
                try
                {
                    bool lecturaAntigua_LECTURA = vg.RegistrosB_LECTURA[0];
                    int inicioPosiciones_LECTURA = int.Parse(CONFIG.MB_LECTURA);
                    if (!vg.bandera_LECTURA)
                    {
                        vg.LeerRegistros_LECTURA = Modbus.readInputDiscretes(slave, inicioPosiciones_LECTURA, vg.Registros_LECTURA, totalPosiciones);
                        if (vg.LeerRegistros_LECTURA == BusProtocolErrors.FTALK_SUCCESS)
                        {
                            if (vg.Registros_LECTURA[0] == true)
                            {
                                //EL DISPOSITIVO MARCA VERDADERO EN LA POSICION DE LECTURA CUANDO HAY UN BULTO
                                LogEventos.EscribirLog("Cliente Modbus - Monitorear()", "Señal detectada en posición " + inicioPosiciones_LECTURA, "", "Action");
                            }
                            if (vg.Registros_LECTURA[0] == false)
                            {
                                //EL DISPOSITIVO MARCA FALSO EN LA POSICION DE LECTURA CUANDO NO HAY UN BULTO
                                LogEventos.EscribirLog("Cliente Modbus - Monitorear()", "Posición " + inicioPosiciones_LECTURA + " vacía", "", "Action");
                            }
                        }
                        else
                        {
                            LogEventos.EscribirLog("Cliente Modbus - Monitorear()", " PLC desconectado, registro: " + inicioPosiciones_LECTURA + ", error: " + vg.LeerRegistros_LECTURA + ", descripción: " + (BusProtocolErrors.getBusProtocolErrorText(vg.LeerRegistros_LECTURA)), "","Action");
                            estadoConexion = 0;
                            FuncionesMMF.EscribirMMF(estadoConexion);
                            conexion = false;
                            detenerTimer();
                            if (vg.LeerRegistros_LECTURA == BusProtocolErrors.FTALK_CONNECTION_WAS_CLOSED || vg.LeerRegistros_LECTURA == 134)
                            {
                                if (!tieneConexion)
                                {
                                    desconectado();
                                }
                            }
                        }
                        vg.bandera_LECTURA = true;
                    }
                    else
                    {
                        vg.LeerRegistros_LECTURA = Modbus.readInputDiscretes(slave, inicioPosiciones_LECTURA, vg.RegistrosA_LECTURA, totalPosiciones);
                        if (vg.LeerRegistros_LECTURA == BusProtocolErrors.FTALK_SUCCESS)
                        {
                            bool lecturaNueva_LECTURA = vg.RegistrosA_LECTURA[0];

                            if (lecturaNueva_LECTURA != lecturaAntigua_LECTURA)
                            {
                                vg.RegistrosB_LECTURA = vg.RegistrosA_LECTURA;
                                if (lecturaNueva_LECTURA == true)
                                {
                                    //EL DISPOSITIVO MARCA VERDADERO EN LA POSICION DE LECTURA CUANDO HAY UN BULTO
                                    LogEventos.EscribirLog("Cliente Modbus - Monitorear()", "Señal detectada en posición " + inicioPosiciones_LECTURA, "", "Action");
                                }
                                if (lecturaNueva_LECTURA == false)
                                {
                                    //EL DISPOSITIVO MARCA FALSO EN LA POSICION DE LECTURA CUANDO NO HAY UN BULTO
                                    LogEventos.EscribirLog("Cliente Modbus - Monitorear()", "Posición " + inicioPosiciones_LECTURA + " vacía", "", "Action");
                                }
                            }
                        }
                        else
                        {
                            LogEventos.EscribirLog("Cliente Modbus - Monitorear()", " PLC desconectado, registro: " + inicioPosiciones_LECTURA + ", error: " + vg.LeerRegistros_LECTURA + ", descripción: " + (BusProtocolErrors.getBusProtocolErrorText(vg.LeerRegistros_LECTURA)), "","Action");
                            estadoConexion = 0;
                            FuncionesMMF.EscribirMMF(estadoConexion);
                            conexion = false;
                            detenerTimer();
                            if (vg.LeerRegistros_LECTURA == BusProtocolErrors.FTALK_CONNECTION_WAS_CLOSED || vg.LeerRegistros_LECTURA == 134)
                            {
                                if (!tieneConexion)
                                {
                                    desconectado();
                                }
                            }
                        }
                    }
                }
                catch (Exception e2)
                {
                    
                    ControlExcepciones.Exception("Cliente Modbus - Monitorear", e2);
                    estadoConexion = 0;
                    FuncionesMMF.EscribirMMF(estadoConexion);
                    conexion = false;
                    detenerTimer();
                }
                vg.mutexMonitor.ReleaseMutex();
            }
        }
        public void desconectado()
        {
            IniciarTimerReconexion();
            tieneConexion = true;
        }
        public void enviarDesdeCliente(string mensaje)
        {
            vg.ClienteCore.Send(vg.coreSocket, STX + mensaje + LF);
        }
        public void IniciarTimerRegistros()
        {
            monitor = new System.Timers.Timer();
            monitor.Interval = int.Parse(CONFIG.MB_INTERVALO_TIMER);
            monitor.AutoReset = true;
            monitor.Elapsed += new System.Timers.ElapsedEventHandler(Monitorear);
            monitor.Start();
        }
        public void IniciarTimerReconexion()
        {
            reconexion = new System.Timers.Timer();
            reconexion.Interval = 5000;
            reconexion.AutoReset = true;
            reconexion.Elapsed += new System.Timers.ElapsedEventHandler(reconectar);
            reconexion.Start();
        }
        public  void reconectar(object sender, System.Timers.ElapsedEventArgs e)
        {
            LogEventos.EscribirLog("Cliente Modbus - Reconectar()", "Reconectando PLC...", "","Action");
            Conectar();
        }

        public  void detenerTimer()
        {
            monitor.Stop();
            monitor.Dispose();
            monitor.EndInit();
            monitoreando = false;
        }
        ~ClienteModbus()
        {
            vg.mutexMonitor.Dispose();
        }
    }
}
