using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace Lab2CheckersServer
{

    public enum Operation
    {
        GetName
    }
    internal class ClientConnection : INotifyPropertyChanged
    {
        private static int ClientNumber = 0;

        private Socket Sock;
        private SocketAsyncEventArgs SockAsyncEventArgs;
        private byte[] buff;

        public User GetUser()
        {
            return new User() { ID = userID, IP = Sock.LocalEndPoint.AddressFamily.ToString(), Login = userName };
        }
        private string userName;
        public string UserName
        {
            get { return userName; }
            set
            {
                userName = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("UserName"));
            }
        }

        private int userID;
        public int UserID
        {
            get { return userID; }
            set
            {
                userID = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("UserID"));
            }
        }

        private string opponent;
        public string Opponent
        {
            get { return opponent; }
            set
            {
                opponent = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("Opponent"));
            }
        }

        public override string ToString()
        {
            return string.Format("#{0} {1}, IP:", this.UserID, UserName, Sock.LocalEndPoint.ToString());
        }


        public ClientConnection(Socket AcceptedSocket)
        {
            ClientNumber++;
            this.UserID = ClientNumber;
            buff = new byte[1024];
            Sock = AcceptedSocket;
            SockAsyncEventArgs = new SocketAsyncEventArgs();
            SockAsyncEventArgs.Completed += SockAsyncEventArgs_Completed;
            SockAsyncEventArgs.SetBuffer(buff, 0, buff.Length);

            ReceiveAsync(SockAsyncEventArgs);
        }

        private void SockAsyncEventArgs_Completed(object sender, SocketAsyncEventArgs e)
        {
            if (e.BytesTransferred == 500 && UserName == null)
            {
                SendBytes(new byte[] { 1 });
                Console.WriteLine("send get username");
                return;
            }
            switch (e.LastOperation)
            {
                case SocketAsyncOperation.Receive:
                    ProcessReceive(e);
                    break;
                case SocketAsyncOperation.Send:
                    ProcessSend(e);
                    break;
            }
        }

        private void ProcessSend(SocketAsyncEventArgs e)
        {
            if (e.SocketError == SocketError.Success)
                ReceiveAsync(SockAsyncEventArgs);
        }

        private void ProcessReceive(SocketAsyncEventArgs e)
        {
            if (e.SocketError == SocketError.Success && e.Buffer.Length > 0)
            {
                if (e.Buffer[0] == 1)
                {
                    this.UserName = Encoding.UTF8.GetString(e.Buffer, 1, e.BytesTransferred - 1);
                    Server.Current.SendPropertiesChanged();
                    Console.WriteLine("set username");
                    return;
                }
                if (e.Buffer[0] == 2 && e.BytesTransferred == 1)
                {
                    var users = Server.Current.ListUsers.Select(a => a.GetUser()).ToArray();
                    BinaryFormatter bf = new BinaryFormatter();
                    MemoryStream ms = new MemoryStream();
                    bf.Serialize(ms, users);
                    var buffer = new List<byte> { 2 };
                    buffer.AddRange(ms.ToArray());
                    SendBytes(buffer.ToArray());
                    Console.WriteLine("send userslist");
                    return;
                }
                string str = Encoding.UTF8.GetString(e.Buffer, 0, e.BytesTransferred);
                Console.WriteLine("Incoming msg from #{0}: {1}", ClientNumber, str);
                SendAsync("You send " + str);
            }
        }

        private void SendBytes(byte[] data)
        {
            SocketAsyncEventArgs enew = new SocketAsyncEventArgs();
            enew.Completed += SockAsyncEventArgs_Completed;
            enew.SetBuffer(data, 0, data.Length);
            SendAsync(enew);
        }

        private void ReceiveAsync(SocketAsyncEventArgs e)
        {
            bool willRaiseEvent = Sock.ReceiveAsync(e);
            if (!willRaiseEvent)
                ProcessReceive(e);
        }

        private void SendAsync(string data)
        {
            byte[] buff = Encoding.UTF8.GetBytes(data);
            SendBytes(buff);
        }

        private void SendAsync(SocketAsyncEventArgs e)
        {
            bool willRaiseEvent = Sock.SendAsync(e);
            if (!willRaiseEvent)
                ProcessSend(e);
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
