using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Lab2CheckersServer
{
    internal class Server : INotifyPropertyChanged
    {
        private Socket Sock;
        private SocketAsyncEventArgs AcceptAsyncArgs;

        private Server()
        {
            Sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            AcceptAsyncArgs = new SocketAsyncEventArgs();
            AcceptAsyncArgs.Completed += AcceptCompleted;
        }

        public static Server Current = new Server();

        private List<ClientConnection> listUsers = new List<ClientConnection>();

        public ClientConnection[] ListUsers
        {
            get { return listUsers.ToArray(); }

        }
        public void SendPropertiesChanged()
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs("ListUsers"));
        }

        private void AcceptCompleted(object sender, SocketAsyncEventArgs e)
        {
            if (e.SocketError == SocketError.Success)
            {
                ClientConnection Client = new ClientConnection(e.AcceptSocket);
                listUsers.Add(Client);
                SendPropertiesChanged();
            }
            e.AcceptSocket = null;
            AcceptAsync(AcceptAsyncArgs);
        }

        private void AcceptAsync(SocketAsyncEventArgs e)
        {
            bool willRaiseEvent = Sock.AcceptAsync(e);
            if (!willRaiseEvent)
                AcceptCompleted(Sock, e);
        }

        public void Start(int Port)
        {
            Sock.Bind(new IPEndPoint(IPAddress.Any, Port));
            Sock.Listen(50);
            AcceptAsync(AcceptAsyncArgs);
        }

        public void Stop()
        {
            Sock.Close();
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
