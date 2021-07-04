using System.Net.Sockets;
using System.Text;

namespace WindowsServiceBase.Objetos
{
    // Objeto de datos del servidor socket iniciado
    public class ObjetoSocket
    {
        public string IP { get; set; }
        public string Puerto { get; set; }
        public string Estado { get; set; }
    }

    // Objeto de estado de los clientes conectados al servidor socket
    public class EstadoSocket
    {
        // Client  socket.
        public Socket workSocket = null;
        // Tamaño del búfer de recepción.
        public const int BufferSize = 20480;
        // Búfer de recepción.
        public byte[] buffer = new byte[BufferSize];
        // Constructor de string de los datos recibidos.
        public StringBuilder sb = new StringBuilder();
    }

    // Objeto de datos de cada cliente conectado al servidor socket, los que se agregarán a la lista de clientes conectados
    public class ClienteConectado
    {
        public string idCliente { get; set; }
        public Socket socketCliente { get; set; }
    }
}
