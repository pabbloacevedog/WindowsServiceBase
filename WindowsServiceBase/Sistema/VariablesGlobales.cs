using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WindowsServiceBase.Sistema
{
    class vg
    {
        public static ClienteTCP ClienteCore = new ClienteTCP();
        public static ClienteModbus modbus = new ClienteModbus();
        public static Socket coreSocket;
        public static Socket impresoraSocket;

        public static string mensajeCore = "";
        public static int contadorMsj = 1;
        public static Mutex mutexMonitor = new Mutex();

        public static string mensajecompleto = "";
        public static string mensaje = "";
        public static string ProcessName = "";


        public static int LeerRegistros_LECTURA;
        public static bool[] Registros_LECTURA = new bool[1];
        public static bool[] RegistrosA_LECTURA = new bool[1];
        public static bool[] RegistrosB_LECTURA = new bool[1];
        public static bool bandera_LECTURA = false;

        //variable que guardar el estado de conexión de la impresora
        public static bool ImpresoraConectada = false;
        public static bool EstadoImpresora = false;
        public static bool validarErrorSocket(SocketException ex)
        {
            bool respuesta = false;
            switch (ex.SocketErrorCode)
            {
                case SocketError.AccessDenied:
                    respuesta = true;
                    //error = "Se intentó obtener acceso a un Socket de una manera prohibida por sus permisos de acceso.";
                    break;
                case SocketError.ConnectionAborted:
                    respuesta = true;
                    //error = "NET Framework o el proveedor de sockets subyacentes anuló la conexión.";
                    break;
                case SocketError.ConnectionRefused:
                    respuesta = true;
                    //error = "El host remoto rechaza activamente una conexión.";
                    break;
                case SocketError.ConnectionReset:
                    respuesta = true;
                    //error = "El host remoto restableció la conexión.";
                    break;
                case SocketError.Disconnecting:
                    respuesta = true;
                    //error = "Se está realizando correctamente una desconexión.";
                    break;
                case SocketError.HostDown:
                    respuesta = true;
                    //error = "Se ha generado un error en la operación porque el host remoto está inactivo.";
                    break;
                case SocketError.Interrupted:
                    respuesta = true;
                    //error = "Se canceló una llamada Socket de bloqueo.";
                    break;
                case SocketError.NotConnected:
                    respuesta = true;
                    //error = "La aplicación intentó enviar o recibir datos y el Socket no está conectado.";
                    break;
                case SocketError.NotInitialized:
                    respuesta = true;
                    //error = "No se ha inicializado el proveedor de sockets subyacentes.";
                    break;
                case SocketError.OperationAborted:
                    respuesta = true;
                    //error = "La operación superpuesta se anuló debido al cierre del Socket.";
                    break;
                case SocketError.Shutdown:
                    respuesta = true;
                    //error = "Se denegó una solicitud de envío o recepción de datos porque ya se ha cerrado el Socket.";
                    break;
            }
            return respuesta;
        }
    }

}
