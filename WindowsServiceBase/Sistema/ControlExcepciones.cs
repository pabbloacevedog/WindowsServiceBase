using System;
using System.Data.SqlClient;
using System.IO;
using System.Messaging;
using System.Net.Sockets;
using System.Net.WebSockets;

namespace WindowsServiceBase.Sistema
{
    public class ControlExcepciones
    {
        public static void SocketException(string modulo, SocketException ex)
        {
            string error = "";
            switch (ex.SocketErrorCode)
            {
                case SocketError.AccessDenied:
                    error = "Se intentó obtener acceso a un Socket de una manera prohibida por sus permisos de acceso.";
                    break;
                case SocketError.AddressAlreadyInUse:
                    error = "Normalmente se permite un solo uso de una dirección.";
                    break;
                case SocketError.AddressFamilyNotSupported:
                    error = "No admite la familia de direcciones especificada. Se devuelve este error si se especificó la familia de direcciones IPv6 y la pila del IPv6 no está instalada en el equipo local. Se devuelve este error si se especificó la familia de direcciones IPv4 y la pila del IPv4 no está instalada en el equipo local.";
                    break;
                case SocketError.AddressNotAvailable:
                    error = "La dirección IP seleccionada no es válida en este contexto.";
                    break;
                case SocketError.AlreadyInProgress:
                    error = "El Socket de no bloqueo ya tiene una operación en curso.";
                    break;
                case SocketError.ConnectionAborted:
                    error = "NET Framework o el proveedor de sockets subyacentes anuló la conexión.";
                    break;
                case SocketError.ConnectionRefused:
                    error = "El host remoto rechaza activamente una conexión.";
                    break;
                case SocketError.ConnectionReset:
                    error = "El host remoto restableció la conexión.";
                    break;
                case SocketError.DestinationAddressRequired:
                    error = "Se ha omitido una dirección necesaria de una operación en un Socket.";
                    break;
                case SocketError.Disconnecting:
                    error = "Se está realizando correctamente una desconexión.";
                    break;
                case SocketError.Fault:
                    error = "El proveedor de sockets subyacentes detectó una dirección de puntero no válida.";
                    break;
                case SocketError.HostDown:
                    error = "Se ha generado un error en la operación porque el host remoto está inactivo.";
                    break;
                case SocketError.HostNotFound:
                    error = "Se desconoce el host.El nombre no es un nombre de host o alias oficial.";
                    break;
                case SocketError.HostUnreachable:
                    error = "No hay ninguna ruta de red al host especificado.";
                    break;
                case SocketError.IOPending:
                    error = "La aplicación ha iniciado una operación superpuesta que no se puede finalizar inmediatamente.";
                    break;
                case SocketError.InProgress:
                    error = "Hay una operación de bloqueo en curso.";
                    break;
                case SocketError.Interrupted:
                    error = "Se canceló una llamada Socket de bloqueo.";
                    break;
                case SocketError.InvalidArgument:
                    error = "Se ha proporcionado un argumento no válido a un miembro de Socket.";
                    break;
                case SocketError.IsConnected:
                    error = "El Socket ya está conectado.";
                    break;
                case SocketError.MessageSize:
                    error = "El datagrama es demasiado largo.";
                    break;
                case SocketError.NetworkDown:
                    error = "La red no está disponible.";
                    break;
                case SocketError.NetworkReset:
                    error = "La aplicación intentó establecer KeepAlive en una conexión cuyo tiempo de espera ya está agotado.";
                    break;
                case SocketError.NetworkUnreachable:
                    error = "No existe ninguna ruta al host remoto.";
                    break;
                case SocketError.NoBufferSpaceAvailable:
                    error = "No hay espacio en búfer disponible para una operación de Socket.";
                    break;
                case SocketError.NoData:
                    error = "No se encontró el nombre o la dirección IP solicitada en el servidor de nombres.";
                    break;
                case SocketError.NoRecovery:
                    error = "El error es irrecuperable o no se encuentra la base de datos solicitada.";
                    break;
                case SocketError.NotConnected:
                    error = "La aplicación intentó enviar o recibir datos y el Socket no está conectado.";
                    break;
                case SocketError.NotInitialized:
                    error = "No se ha inicializado el proveedor de sockets subyacentes.";
                    break;
                case SocketError.NotSocket:
                    error = "Se intentó realizar una operación de Socket en algo que no es un socket.";
                    break;
                case SocketError.OperationAborted:
                    error = "La operación superpuesta se anuló debido al cierre del Socket.";
                    break;
                case SocketError.OperationNotSupported:
                    error = "La familia de protocolos no admite la familia de direcciones.";
                    break;
                case SocketError.ProcessLimit:
                    error = "Demasiados procesos están utilizando el proveedor de sockets subyacentes.";
                    break;
                case SocketError.ProtocolFamilyNotSupported:
                    error = "La familia de protocolos no está implementada o no está configurada.";
                    break;
                case SocketError.ProtocolNotSupported:
                    error = "El protocolo no está implementado o no está configurado.";
                    break;
                case SocketError.ProtocolOption:
                    error = "Se ha utilizado una opción o un nivel desconocido, no válido o incompatible con un Socket.";
                    break;
                case SocketError.ProtocolType:
                    error = "El tipo de protocolo es incorrecto para este Socket.";
                    break;
                case SocketError.Shutdown:
                    error = "Se denegó una solicitud de envío o recepción de datos porque ya se ha cerrado el Socket.";
                    break;
                case SocketError.SocketError:
                    error = "Se ha producido un error de Socket no especificado.";
                    break;
                case SocketError.SocketNotSupported:
                    error = "Esta familia de direcciones no es compatible con el tipo de socket especificado.";
                    break;
                case SocketError.Success:
                    error = "La operación de Socket se ha realizado correctamente.";
                    break;
                case SocketError.SystemNotReady:
                    error = "El subsistema de red no está disponible.";
                    break;
                case SocketError.TimedOut:
                    error = "El intento de conexión ha sobrepasado el tiempo de espera o el host conectado no ha respondido.";
                    break;
                case SocketError.TooManyOpenSockets:
                    error = "Hay demasiados sockets abiertos en el proveedor de sockets subyacentes.";
                    break;
                case SocketError.TryAgain:
                    error = "No se pudo resolver el nombre del host.Vuelva a intentarlo más tarde.";
                    break;
                case SocketError.TypeNotFound:
                    error = "No se encontró la clase especificada.";
                    break;
                case SocketError.VersionNotSupported:
                    error = "La versión del proveedor de sockets subyacentes está fuera del intervalo.";
                    break;
                case SocketError.WouldBlock:
                    error = "No se puede finalizar inmediatamente una operación en un socket de no bloqueo.";
                    break;
            }
            LogEventos.EscribirLog(modulo, error, "", "Action");
        }

        public static void MessageQueueException(string modulo, MessageQueueException ex)
        {
            string error = "";
            switch (ex.MessageQueueErrorCode)
            {
                case MessageQueueErrorCode.AccessDenied:
                    error = "Acceso denegado";
                    break;
                case MessageQueueErrorCode.BadSecurityContext:
                    error = "Mal Contexto de Seguridad";
                    break;
                case MessageQueueErrorCode.Base:
                    error = "ERROR NO ESPECIFICADO";
                    break;
                case MessageQueueErrorCode.BufferOverflow:
                    error = "Esta API es compatible con la infraestructura de producto y no está destinado a ser utilizado desde el código. El mensaje: El buffer suministrado a MQReceiveMessage para la recuperación del cuerpo del mensaje era demasiado pequeño. El mensaje no se elimina de la cola y parte del cuerpo del mensaje que cabe en el búfer fue copiado.";
                    break;
                case MessageQueueErrorCode.CannotCreateCertificateStore:
                    error = "Esta API es compatible con la infraestructura de producto y no está destinado a ser utilizado desde el código. El mensaje: El buffer suministrado a MQReceiveMessage para la recuperación del cuerpo del mensaje era demasiado pequeño. El mensaje no se elimina de la cola y parte del cuerpo del mensaje que cabe en el búfer fue copiado.";
                    break;
                case MessageQueueErrorCode.CannotCreateHashEx:
                    error = "No se puede crear un objeto hash para un mensaje autenticado";
                    break;
                case MessageQueueErrorCode.CannotCreateOnGlobalCatalog:
                    error = "No se pudo crear un objeto en un servidor de catálogo global especificado";
                    break;
                case MessageQueueErrorCode.CannotGetDistinguishedName:
                    error = "No se puede recuperar el nombre distintivo del equipo local";
                    break;
                case MessageQueueErrorCode.CannotGrantAddGuid:
                    error = "No se pudo conceder el permiso Add Guid al usuario actual";
                    break;
                case MessageQueueErrorCode.CannotHashDataEx:
                    error = "No se puede hacer hash de los datos para un mensaje autenticado";
                    break;
                case MessageQueueErrorCode.CannotImpersonateClient:
                    error = "El servidor RPC no puede suplantar a la aplicacion cliente, dado que no se puede comprobar las credenciales de seguridad";
                    break;
                case MessageQueueErrorCode.CannotJoinDomain:
                    error = "No se pudo unir MSMQ Enterprise a un dominio de Windows 2000";
                    break;
                case MessageQueueErrorCode.CannotLoadMsmqOcm:
                    error = "No se puede cargar la biblioteca MSQMOCM.DLL";
                    break;
                case MessageQueueErrorCode.CannotOpenCertificateStore:
                    error = "No se puede abrir el almacén de certificados del sertificado interno";
                    break;
                case MessageQueueErrorCode.CannotSetCryptographicSecurityDescriptor:
                    error = "No se puede establecer el descriptor de seguridad para las claves criptográficas.";
                    break;
                case MessageQueueErrorCode.CannotSignDataEx:
                    error = "No se puede firmar los datos antes de enviar un mensaje autenticado.";
                    break;
                case MessageQueueErrorCode.CertificateNotProvided:
                    error = "Un usuario ha intentado enviar un mensaje autenticado sin un certificado.";
                    break;
                case MessageQueueErrorCode.ComputerDoesNotSupportEncryption:
                    error = "El equipo no admite operaciones de cifrado.";
                    break;
                case MessageQueueErrorCode.CorruptedInternalCertificate:
                    error = "El mensaje interno Queuing certificado está dañado";
                    break;
                case MessageQueueErrorCode.CorruptedPersonalCertStore:
                    error = "El almacén de certificados personales está dañado.";
                    break;
                case MessageQueueErrorCode.CorruptedQueueWasDeleted:
                    error = "archivo .ini para la cola en LQS se eliminó porque fue dañado.";
                    break;
                case MessageQueueErrorCode.CorruptedSecurityData:
                    error = "Una función criptográfica ha fallado.";
                    break;
                case MessageQueueErrorCode.CouldNotGetAccountInfo:
                    error = "No se pudo obtener la información de la cuenta para el usuario.";
                    break;
                case MessageQueueErrorCode.CouldNotGetUserSid:
                    error = "No se pudo obtener la información SID del token de hilo.";
                    break;
                case MessageQueueErrorCode.DeleteConnectedNetworkInUse:
                    error = "La red conectada no se puede eliminar; está en uso.";
                    break;
                case MessageQueueErrorCode.DependentClientLicenseOverflow:
                    error = "El número de clientes dependientes atendidas por este servidor de Message Queue Server alcanzó su límite superior.";
                    break;
                case MessageQueueErrorCode.DsError:
                    error = "error del servicio de directorio interno.";
                    break;
                case MessageQueueErrorCode.DsIsFull:
                    error = "El servicio de directorio está lleno";
                    break;
                case MessageQueueErrorCode.DtcConnect:
                    error = "No se puede conectar a MS DTC";
                    break;
                case MessageQueueErrorCode.EncryptionProviderNotSupported:
                    error = " El proveedor de servicios de cifrado no es apoyada por Message Queue Serve";
                    break;
                case MessageQueueErrorCode.FailVerifySignatureEx:
                    error = " Firma del mensaje recibido no es válido.";
                    break;
                case MessageQueueErrorCode.FormatNameBufferTooSmall:
                    error = "La memoria intermedia nombre de formato suministrado a la API era demasiado pequeño para poner el nombre de formato.";
                    break;
                case MessageQueueErrorCode.Generic:
                    error = "El mensaje: Error genérico.";
                    break;
                case MessageQueueErrorCode.GuidNotMatching:
                    error = "No se pudo crear Message Queue objeto de configuración con un GUID que coincide con la instalación del equipo. Debe desinstalar Message Queue Server y vuelva a instalarlo.";
                    break;
                case MessageQueueErrorCode.IOTimeout:
                    error = "El recibir o mensaje vistazo tiempo de espera ha expirado.";
                    break;
                case MessageQueueErrorCode.IllegalContext:
                    error = "parámetro de contexto no válido.";
                    break;
                case MessageQueueErrorCode.IllegalCriteriaColumns:
                    error = "Parámetro no válido MQCOLUMNS.";
                    break;
                case MessageQueueErrorCode.IllegalCursorAction:
                    error = "MQ_ACTION_PEEK_NEXT especificado a MQReceiveMessage no se puede utilizar con la posición actual del cursor.";
                    break;
                case MessageQueueErrorCode.IllegalEnterpriseOperation:
                    error = "La operación no es válida para un servicios objeto Message Queue Server.";
                    break;
                case MessageQueueErrorCode.IllegalFormatName:
                    error = "El nombre de formato no es válido.";
                    break;
                case MessageQueueErrorCode.IllegalMessageProperties:
                    error = "parámetro MQQMPROPS ilegales, ya sea nulo o con cero propiedades.";
                    break;
                case MessageQueueErrorCode.IllegalOperation:
                    error = " La operación no es válida en sistemas de colas de mensajes extranjera.";
                    break;
                case MessageQueueErrorCode.IllegalPrivateProperties:
                    error = " El valor del parámetro propiedades privadas no es válido. Esto puede ser debido a que tiene un valor nulo o tiene cero propiedades especificadas.";
                    break;
                case MessageQueueErrorCode.IllegalPropertyId:
                    error = "la propiedad no válido valor identificador.";
                    break;
                case MessageQueueErrorCode.IllegalPropertySize:
                    error = " la propiedad ilegal tamaño del búfer.";
                    break;
                case MessageQueueErrorCode.IllegalPropertyValue:
                    error = " valor de propiedad no válido";
                    break;
                case MessageQueueErrorCode.IllegalPropertyVt:
                    error = " El mensaje: valor VARTYPE válido.";
                    break;
                case MessageQueueErrorCode.IllegalQueuePathName:
                    error = "cola no válida nombre de ruta.";
                    break;
                case MessageQueueErrorCode.IllegalQueueProperties:
                    error = "parámetro MQQUEUEPROPS ilegales, ya sea nulo o con cero propiedades.";
                    break;
                case MessageQueueErrorCode.IllegalRelation:
                    error = "Valor de relación no válida en la restricción.";
                    break;
                case MessageQueueErrorCode.IllegalRestrictionPropertyId:
                    error = "valor PropID válida en el parámetro MQRESTRICTION.";
                    break;
                case MessageQueueErrorCode.IllegalSecurityDescriptor:
                    error = "El descriptor de seguridad especificado no es un descriptor de seguridad válido.";
                    break;
                case MessageQueueErrorCode.IllegalSort:
                    error = "tipo ilegal especificada (por ejemplo, duplicar columnas).";
                    break;
                case MessageQueueErrorCode.IllegalSortPropertyId:
                    error = "valor PropID válida en MQSORTSET.";
                    break;
                case MessageQueueErrorCode.IllegalUser:
                    error = "El usuario tiene un nombre de usuario vá";
                    break;
                case MessageQueueErrorCode.InsufficientProperties:
                    error = "No todas las propiedades requeridas para la operación se especifican en los parámetros de entrada.";
                    break;
                case MessageQueueErrorCode.InsufficientResources:
                    error = "Recursos insuficientes para realizar la operación.";
                    break;
                case MessageQueueErrorCode.InvalidCertificate:
                    error = "El certificado de usuario no es válido.";
                    break;
                case MessageQueueErrorCode.InvalidHandle:
                    error = "Un identificador no válido pasó a una función.";
                    break;
                case MessageQueueErrorCode.InvalidOwner:
                    error = "El mensaje publicado por un objeto no válido. Por ejemplo createQueue falló porque el objeto Queue Manager no es válido.";
                    break;
                case MessageQueueErrorCode.InvalidParameter:
                    error = "un parámetro no válido pasó a una función.";
                    break;
                case MessageQueueErrorCode.LabelBufferTooSmall:
                    error = "El tampón de etiqueta suministrada a la API era demasiado pequeño.";
                    break;
                case MessageQueueErrorCode.MachineExists:
                    error = " Ordenador con el mismo nombre ya existe en el sitio.";
                    break;
                case MessageQueueErrorCode.MachineNotFound:
                    error = "El equipo especificado no se pudo encontrar.";
                    break;
                case MessageQueueErrorCode.MessageAlreadyReceived:
                    error = "Un mensaje que actualmente está señalado por el cursor se ha eliminado de la cola por otro proceso o por otra llamada para recibir el mensaje sin el uso de este cursor.";
                    break;
                case MessageQueueErrorCode.MessageNotFound:
                    error = " El mensaje especificado no se pudo encontrar.";
                    break;
                case MessageQueueErrorCode.MessageStorageFailed:
                    error = "No se pudo almacenar un mensaje recuperable o diario. Mensaje no fue enviado.";
                    break;
                case MessageQueueErrorCode.MissingConnectorType:
                    error = "Tipo de conector es obligatorio cuando se envía un acuse o mensaje seguro.";
                    break;
                case MessageQueueErrorCode.MqisReadOnlyMode:
                    error = "la base de datos MQIS está en modo de sólo lectura.";
                    break;
                case MessageQueueErrorCode.MqisServerEmpty:
                    error = "La lista de servidores MQIS (en el registro) está vacía.";
                    break;
                case MessageQueueErrorCode.NoDs:
                    error = "No hay conexión con el controlador (s) de este sitio.";
                    break;
                case MessageQueueErrorCode.NoEntryPointMsmqOcm:
                    error = "No se puede encontrar un punto de entrada en la biblioteca Msmqocm.dll.";
                    break;
                case MessageQueueErrorCode.NoGlobalCatalogInDomain:
                    error = "No se puede encontrar servidores de catálogo global en el dominio especificado.";
                    break;
                case MessageQueueErrorCode.NoInternalUserCertificate:
                    error = "El certificado de Message Queue Server interno para el usuario no existe.";
                    break;
                case MessageQueueErrorCode.NoMsmqServersOnDc:
                    error = "No se pudo encontrar servidores de Message Queue Server en controladores de dominio.";
                    break;
                case MessageQueueErrorCode.NoMsmqServersOnGlobalCatalog:
                    error = "No se pudo encontrar servidores de Message Queue Server en controladores de dominio de catálogo global.";
                    break;
                case MessageQueueErrorCode.NoResponseFromObjectServer:
                    error = "No hay respuesta del propietario del objeto.";
                    break;
                case MessageQueueErrorCode.ObjectServerNotAvailable:
                    error = " El mensaje publicado por un objeto no es alcanzable.";
                    break;
                case MessageQueueErrorCode.OperationCanceled:
                    error = "La operación fue cancelada antes de que se pudo completar.";
                    break;
                case MessageQueueErrorCode.PrivilegeNotHeld:
                    error = "El cliente no tiene los privilegios necesarios para realizar la operación";
                    break;
                case MessageQueueErrorCode.Property:
                    error = "Una o más de las propiedades pasadas no son válidos.";
                    break;
                case MessageQueueErrorCode.PropertyNotAllowed:
                    error = "la propiedad no válido para la operación solicitada";
                    break;
                case MessageQueueErrorCode.ProviderNameBufferTooSmall:
                    error = "El buffer pasado para la propiedad Nombre del proveedor es demasiado pequeño.";
                    break;
                case MessageQueueErrorCode.PublicKeyDoesNotExist:
                    error = "La clave pública para que el equipo no existe.";
                    break;
                case MessageQueueErrorCode.PublicKeyNotFound:
                    error = "No se puede encontrar la clave pública para la computadora.";
                    break;
                case MessageQueueErrorCode.QDnsPropertyNotSupported:
                    error = " Propiedad DNS no se admite como criterio para localizar las colas.";
                    break;
                case MessageQueueErrorCode.QueueDeleted:
                    error = "Se eliminó la cola. Los mensajes no se pueden recibir más el uso de esta instancia de cola. La cola debe ser cerrado.";
                    break;
                case MessageQueueErrorCode.QueueExists:
                    error = "Una cola con el mismo nombre de ruta ya está registrado.";
                    break;
                case MessageQueueErrorCode.QueueNotAvailable:
                    error = "Error al leer de una cola que reside en un equipo remoto.";
                    break;
                case MessageQueueErrorCode.QueueNotFound:
                    error = " La cola no está registrada en el servicio de directorio.";
                    break;
                case MessageQueueErrorCode.RemoteMachineNotAvailable:
                    error = "El equipo remoto no está disponible.";
                    break;
                case MessageQueueErrorCode.ResultBufferTooSmall:
                    error = "El tampón resultado suministrado es demasiado pequeño.";
                    break;
                case MessageQueueErrorCode.SecurityDescriptorBufferTooSmall:
                    error = "El tamaño de la memoria intermedia se pasa a MQGetQueueSecurity es demasiado pequeño.";
                    break;
                case MessageQueueErrorCode.SenderCertificateBufferTooSmall:
                    error = "El buffer pasado para la propiedad certificado de usuario es demasiado pequeño.";
                    break;
                case MessageQueueErrorCode.SenderIdBufferTooSmall:
                    error = "El buffer pasado para la propiedad identificador de usuario es demasiado pequeño.";
                    break;
                case MessageQueueErrorCode.ServiceNotAvailable:
                    error = "El servicio de colas de mensajes no está disponible.";
                    break;
                case MessageQueueErrorCode.SharingViolation:
                    error = "Compartiendo violación. La cola ya está abierto para recibir en exclusiva.";
                    break;
                case MessageQueueErrorCode.SignatureBufferTooSmall:
                    error = "El buffer pasado por la propiedad de la firma es demasiado pequeño.";
                    break;
                case MessageQueueErrorCode.StaleHandle:
                    error = "El servicio Administrador de colas se ha reiniciado. El asa de cola es rancio y debe cerrarse.";
                    break;
                case MessageQueueErrorCode.SymmetricKeyBufferTooSmall:
                    error = "El buffer pasado para la propiedad de clave simétrica es demasiado pequeño.";
                    break;
                case MessageQueueErrorCode.TransactionEnlist:
                    error = "No se puede escribir en la transacción";
                    break;
                case MessageQueueErrorCode.TransactionImport:
                    error = "No se puede importar la transacción";
                    break;
                case MessageQueueErrorCode.TransactionSequence:
                    error = "La secuencia de operaciones de la transección es incorrecta";
                    break;
                case MessageQueueErrorCode.TransactionUsage:
                    error = "El uso de la transaccion es incorrecto";
                    break;
                case MessageQueueErrorCode.UnsupportedAccessMode:
                    error = "El modo de acceso especificado no es compatible";
                    break;
                case MessageQueueErrorCode.UnsupportedFormatNameOperation:
                    error = "La operación solicitada por el nombre del formato especificado no es compatible.";
                    break;
                case MessageQueueErrorCode.UnsupportedOperation:
                    error = "La operación no es compatible para un equipo de instalación GRUPO DE TRABAJO.";
                    break;
                case MessageQueueErrorCode.UserBufferTooSmall:
                    error = "Solicitar fracasado porque búfer de usuario es demasiado pequeño para contener la información devuelta.";
                    break;
                case MessageQueueErrorCode.WksCantServeClient:
                    error = "los clientes de Message Queue Server-independientes no pueden servir de mensajes clientes Queuing-dependientes.";
                    break;
                case MessageQueueErrorCode.WriteNotAllowed:
                    error = "se va a instalar otro servidor MQIS; operaciones de escritura a la base de datos no se permiten en este momento.";
                    break;
            }
            LogEventos.EscribirLog(modulo, error, "", "Action");
        }

        public static void WebSocketException(string modulo, WebSocketException ex, string logDestino)
        {
            string error = "";
            switch (ex.WebSocketErrorCode)
            {
                case WebSocketError.ConnectionClosedPrematurely:
                    error = "La conexión fue cerrada de forma inesperada";
                    break;
                case WebSocketError.Faulted:
                    error = "Error generalizado en el WebSocket";
                    break;
                case WebSocketError.HeaderError:
                    error = "Se ha producido un error al analizar los encabezados HTTP durante el protocolo de enlace o handshake";
                    break;
                case WebSocketError.InvalidMessageType:
                    error = "Se recibió una trama WebSocket con un código de operación desconocido";
                    break;
                case WebSocketError.InvalidState:
                    error = "El WebSocket se encuentra en un estado no válido para la operación dada (como estar cerrado o abortado)";
                    break;
                case WebSocketError.NativeError:
                    error = "Se produjo un error nativo desconocido";
                    break;
                case WebSocketError.NotAWebSocket:
                    error = "La solicitud entrante no era una solicitud de websocket válida";
                    break;
                case WebSocketError.Success:
                    error = "No hubo información de error nativa para la excepción";
                    break;
                case WebSocketError.UnsupportedProtocol:
                    error = "El cliente solicitó un subprotocolo de WebSocket no compatible";
                    break;
                case WebSocketError.UnsupportedVersion:
                    error = "El cliente solicitó una versión no compatible del protocolo WebSocket";
                    break;
            }
            LogEventos.EscribirLog(modulo, error, "", logDestino);
        }

        public static void SQLException(string modulo, SqlException error)
        {
            string errorMsj = error.Message.ToString();
            LogEventos.EscribirLog(modulo, errorMsj, "", "Action");
        }

        public static void IOException(string modulo, IOException error)
        {
            string errorMsj = error.Message.ToString();
            LogEventos.EscribirLog(modulo, errorMsj, "", "Action");
        }

        public static void Exception(string modulo, Exception error)
        {
            string errorMsj = error.Message.ToString();
            LogEventos.EscribirLog(modulo, errorMsj, "", "Action");
        }
    }
}
