using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Klyuchnikov.Seti.TwoSemestr.Lab4
{
    public class Client : INotifyPropertyChanged
    {
        public class StateObject
        {
            // Client socket.
            public Socket workSocket = null;
            // Size of receive buffer.
            public const int BufferSize = 1024;
            // Receive buffer.
            public byte[] buffer = new byte[BufferSize];
            // Received data string.
            public StringBuilder sb = new StringBuilder();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public static Client Current = new Client();

        private bool isConnected;
        public bool IsConnected
        {
            get
            {
                return isConnected;
            }
            private set
            {
                isConnected = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("IsConnected"));
            }
        }

        private int port;
        public int Port
        {
            get { return port; }
            set
            {
                port = value;

                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("Port"));
            }
        }

        private string address;
        public string Address
        {
            get { return address; }
            set
            {
                address = value;

                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("Address"));
            }
        }

        public Socket Sock;
        public SocketAsyncEventArgs SockAsyncArgs;
        //  private Timer timer;
        private byte[] buff;

        private Client()
        {
            buff = new byte[1024];
            Sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        }

        private readonly Guid guid = Guid.NewGuid();
        public Guid Guid { get { return guid; } }

        public void ConnectAsync(string _address, int _port)
        {
            buff = new byte[1024];
            Sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            this.Address = _address;
            this.Port = _port;
            SockAsyncArgs = new SocketAsyncEventArgs { RemoteEndPoint = new DnsEndPoint(this.Address, this.Port) };
            SockAsyncArgs.Completed += SockAsyncArgs_Completed;
            ConnectAsync(SockAsyncArgs);
        }

        public void Close()
        {
            if (Sock.Connected)
                Sock.Close();
            IsConnected = Sock.Connected;
        }

        private void Receive(Socket Sock)
        {
            try
            {
                // Create the state object.
                StateObject state = new StateObject();
                state.workSocket = Sock;

                // Begin receiving the data from the remote device.
                Sock.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                  new AsyncCallback(ReceiveCallback), state);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        public void ReceiveCallback(IAsyncResult ar)
        {
            try
            {
                IsConnected = Sock.Connected;
                StateObject state = (StateObject)ar.AsyncState;
                Socket client = state.workSocket;
                if (Sock.Connected)
                {
                    int bytesRead = client.EndReceive(ar);
                    var arr = state.buffer.Take(bytesRead).ToArray();

                    if (bytesRead > 0)
                        if (arr[0] != 0)
                        {
                            var str = Encoding.Default.GetString(arr);
                            Console.Write(str);
                        }
                        else
                            Client.Current.SendAsync("\r\n");
                    client.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                                        new AsyncCallback(ReceiveCallback), state);
                }
            }
            catch (Exception e)
            {
                try
                {
                    File.AppendAllLines("log_" + ".txt", new string[] { e.Message, e.StackTrace });
                }
                catch (Exception)
                {
                }
                Console.WriteLine(e.ToString());
            }
        }

        private void ConnectAsync(SocketAsyncEventArgs e)
        {
            bool willRaiseEvent = Sock.ConnectAsync(e);
            if (!willRaiseEvent)
                ProcessConnect(e);
        }

        public void SendAsync(string data)
        {
            if (Sock.Connected && data.Length > 0)
            {
                byte[] buff = Encoding.UTF8.GetBytes(data);
                SocketAsyncEventArgs e = new SocketAsyncEventArgs();
                e.SetBuffer(buff, 0, buff.Length);
                e.Completed += SockAsyncArgs_Completed;
                SendAsync(e);
            }
        }
        private void SendAsync(SocketAsyncEventArgs e)
        {
            try
            {
                Sock.SendAsync(e);
            }
            catch (Exception exception)
            {
                Console.WriteLine("error SendAsync - " + exception.Message);
            }
        }

        void SockAsyncArgs_Completed(object sender, SocketAsyncEventArgs e)
        {
            IsConnected = Sock.Connected;
            switch (e.LastOperation)
            {
                case SocketAsyncOperation.Connect:
                    ProcessConnect(e);
                    break;
                case SocketAsyncOperation.Receive:
                    break;
                case SocketAsyncOperation.Send:
                    //    ProcessSend(e);
                    break;
            }
        }

        public void SendBytes(byte[] data)
        {
            SocketAsyncEventArgs newE = new SocketAsyncEventArgs();
            newE.SetBuffer(data, 0, data.Length);
            newE.Completed += SockAsyncArgs_Completed;
            SendAsync(newE);
            Console.WriteLine("send data");
        }

        private void ProcessConnect(SocketAsyncEventArgs e)
        {
            if (e.SocketError == SocketError.Success)
            {
                Receive(Sock);
                Console.WriteLine("Connected to {0}...", e.RemoteEndPoint.ToString());
                SockAsyncArgs.SetBuffer(buff, 0, buff.Length);
            }
            else
            {
                Console.WriteLine("Dont connect to {0}", e.RemoteEndPoint.ToString());
            }
        }
    }
}

