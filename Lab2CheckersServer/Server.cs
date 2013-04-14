using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace Klyuchnikov.Seti.OneSemestr.Lab2CheckersServer
{
    internal class Server : INotifyPropertyChanged
    {
        private Socket Sock;
        private SocketAsyncEventArgs AcceptAsyncArgs;
        private bool IsOpen { get; set; }

        private Server()
        {
            ListUsers = new List<User>();
            ListConnection = new List<ClientConnection>();
        }

        public static Server Current = new Server();

        public List<User> ListUsers { get; set; }

        public List<ClientConnection> ListConnection { get; set; }

        public User[] ListUsersArray
        {
            get { return ListUsers.ToArray(); }
        }

        public void SendPropertiesChanged()
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs("ListUsersArray"));
        }

        private void AcceptCompleted(object sender, SocketAsyncEventArgs e)
        {
            if (IsOpen)
            {
                if (e.SocketError == SocketError.Success)
                {
                    ClientConnection Client = new ClientConnection(e.AcceptSocket);
                    ListConnection.Add(Client);
                    SendPropertiesChanged();
                }
                e.AcceptSocket = null;
                AcceptAsync(AcceptAsyncArgs);
            }
        }

        private void AcceptAsync(SocketAsyncEventArgs e)
        {
            bool willRaiseEvent = Sock.AcceptAsync(e);
            if (!willRaiseEvent)
                AcceptCompleted(Sock, e);
        }

        public void Start(int Port)
        {
            Sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            AcceptAsyncArgs = new SocketAsyncEventArgs();
            AcceptAsyncArgs.Completed += AcceptCompleted;
            Sock.Bind(new IPEndPoint(IPAddress.Any, Port));
            Sock.Listen(50);
            AcceptAsync(AcceptAsyncArgs);
            IsOpen = true;

            var timer = new Timer { Interval = 200 };
            timer.Elapsed += delegate
                {
                    foreach (var connection in Server.Current.ListConnection)
                        connection.SendListUsers();
                };
            //timer.Start();
        }

        public void Stop()
        {
            //    Sock.Shutdown(SocketShutdown.Both);
            Sock.Close();
            IsOpen = false;
            ListUsers.RemoveAll(a => a.Guid != Guid.Empty);
            ListConnection.RemoveAll(a => a.UserBind != null);
            ListUsers = new List<User>();
            ListConnection = new List<ClientConnection>();
            SendPropertiesChanged();
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
