using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Net;
using System.Timers;

namespace Lab2CheckersClient
{
    public class Client : INotifyPropertyChanged
    {
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

        public void ConnectAsync(string Address, int Port)
        {

            SockAsyncArgs = new SocketAsyncEventArgs();
            SockAsyncArgs.Completed += SockAsyncArgs_Completed;
            SockAsyncArgs.RemoteEndPoint = new DnsEndPoint(Address, Port);
            ConnectAsync(SockAsyncArgs);
            var timer = new Timer { Interval = 500 };
            timer.Elapsed += delegate
                                 {

                                     //    timer.Stop();
                                 };
            timer.Start();
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
                bool willRaiseEvent = Sock.SendAsync(e);
                if (!willRaiseEvent)
                    ProcessSend(e);
            }
            catch (Exception)
            {
                Console.WriteLine("error SendAsync");
            }
        }

        private void ReceiveAsync(SocketAsyncEventArgs e)
        {
            if (e.Buffer != null)
                try
                {
                    Console.WriteLine("ReceiveAsync");
                    bool willRaiseEvent = Sock.ReceiveAsync(e);
                    Console.WriteLine("ReceiveAsync2");
                    if (!willRaiseEvent)
                        ProcessReceive(e);
                }
                catch (Exception)
                {
                    Console.WriteLine("error ReceiveAsync");
                }
        }

        void SockAsyncArgs_Completed(object sender, SocketAsyncEventArgs e)
        {
            IsConnected = Sock.Connected;
            Console.WriteLine("Sock.Connected = {0}", Sock.Connected);
            Console.WriteLine("SocketAsyncOperation = {0}", e.LastOperation);
            switch (e.LastOperation)
            {
                case SocketAsyncOperation.Connect:
                    ProcessConnect(e);
                    break;
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
            {
                ReceiveAsync(SockAsyncArgs);
            }
            else
            {
                Console.WriteLine("Dont send");
            }
        }

        private void ProcessReceive(SocketAsyncEventArgs e)
        {
            try
            {
                if (e.SocketError == SocketError.Success)
                {
                    if (e.BytesTransferred == 1) // запросы сервера
                    {
                        if (e.Buffer[0] == 1)
                        {
                            Console.WriteLine("send username");
                            var userName = Encoding.UTF8.GetBytes(GameProcess.Inctance.UserName);
                            var buffer = new List<byte> { 1 };
                            buffer.AddRange(userName);
                            SendBytes(buffer.ToArray());
                        }
                    }
                    else // ответы на свои запросы
                    {
                        switch (e.Buffer[0])
                        {
                            case 2: // получение списка пользователей на сервере
                                BinaryFormatter bf = new BinaryFormatter();
                                MemoryStream ms = new MemoryStream(e.Buffer, 1, e.BytesTransferred - 1);
                                var ob = (User[])bf.Deserialize(ms);
                                GameProcess.Inctance.Users = ob;
                                Console.WriteLine("set users");
                                break;
                        }
                    }
                    SendBytes(new List<byte> { 2 }.ToArray());
                    Console.WriteLine("send get users");
                }
                else
                {
                    Console.WriteLine("Dont recieve");
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
            }
           
        }

        private void SendBytes(byte[] data)
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
                Console.WriteLine("Connected to {0}...", e.RemoteEndPoint.ToString());
                SockAsyncArgs.SetBuffer(buff, 0, buff.Length);
                var username = Encoding.UTF8.GetBytes(GameProcess.Inctance.UserName);
                var buffer = new List<byte> { 1 };
                buffer.AddRange(username);
                //   SendBytes(buffer.ToArray());
                Console.WriteLine("send get username");
                SendBytes(new List<byte> { 2 }.ToArray());
                Console.WriteLine("send get users");
            }
            else
            {
                Console.WriteLine("Dont connect to {0}", e.RemoteEndPoint.ToString());
            }
        }
    }
}

