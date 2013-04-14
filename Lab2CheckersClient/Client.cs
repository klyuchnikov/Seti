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
using System.Web.Script.Serialization;

namespace Klyuchnikov.Seti.OneSemestr.Lab2CheckersClient
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
            this.Address = _address;
            this.Port = _port;
            SockAsyncArgs = new SocketAsyncEventArgs { RemoteEndPoint = new DnsEndPoint(this.Address, this.Port) };
            SockAsyncArgs.Completed += SockAsyncArgs_Completed;
            var userName = Encoding.UTF8.GetBytes(GameProcess.Inctance.UserName);
            var buffer = new List<byte> { (byte)Operation.UserName };
            buffer.AddRange(Encoding.UTF8.GetBytes(guid.ToString()));
            buffer.AddRange(userName);
            SockAsyncArgs.SetBuffer(buffer.ToArray(), 0, buffer.Count);
            ConnectAsync(SockAsyncArgs);

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
                StateObject state = (StateObject)ar.AsyncState;
                Socket client = state.workSocket;

                int bytesRead = client.EndReceive(ar);

                if (bytesRead > 0)
                {
                    Console.WriteLine("operation - {0}", buff[0]);
                    client.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                     new AsyncCallback(ReceiveCallback), state);

                    if (bytesRead == 1) // запросы сервера
                    {
                        switch ((Operation)state.buffer[0])
                        {
                            case Operation.Assert:
                                break;
                            case Operation.UserName:
                                Console.WriteLine("send username");
                                var userName = Encoding.UTF8.GetBytes(GameProcess.Inctance.UserName);
                                var buffer = new List<byte> { (byte)Operation.UserName };
                                buffer.AddRange(userName);
                                SendBytes(buffer.ToArray());
                                break;
                            case Operation.AbortOpponentConnection:
                                GameProcess.Inctance.MainWindow.AbortOpponentConnection();
                                break;
                            case Operation.OfferDraw:
                                GameProcess.Inctance.MainWindow.OfferDraw();
                                break;
                            case Operation.GiveUp:
                                GameProcess.Inctance.MainWindow.GiveUpOpponent();
                                break;
                        }
                    }
                    else // ответы на свои запросы
                    {
                        switch ((Operation)state.buffer[0])
                        {
                            case Operation.ListUsers: // получение списка пользователей на сервере
                                var str = Encoding.UTF8.GetString(state.buffer.Skip(1).Take(bytesRead - 1).ToArray());
                                JavaScriptSerializer c = new JavaScriptSerializer();
                                var users = c.Deserialize<User[]>(str);
                                GameProcess.Inctance.Users = users;
                                Console.WriteLine("set users");
                                break;
                            case Operation.SubmitGame: // получение приглашения на игру

                                Console.WriteLine("get TakeGame");
                                var guid = new Guid(state.buffer.Skip(1).Take(16).ToArray());
                                var user = GameProcess.Inctance.Users.Single(a => a.Guid == guid);
                                GameProcess.Inctance.MainWindow.TakeGame(user);
                                break;
                            case Operation.TakeGame: // ответ на приглашение на игру
                                Console.WriteLine("get StartGame");
                                var res = state.buffer[1] == 1;
                                var guidOwer = new Guid(state.buffer.Skip(2).Take(16).ToArray());
                                var guidTake = new Guid(state.buffer.Skip(18).Take(16).ToArray());
                                User userTake = null;
                                User userOwer = null;
                                try
                                {
                                    userTake = GameProcess.Inctance.Users.Single(a => a.Guid == guidTake);
                                    userOwer = GameProcess.Inctance.Users.Single(a => a.Guid == guidOwer);
                                }
                                catch (Exception exx)
                                { }
                                if (guidOwer == Guid)
                                {
                                    if (res)
                                        GameProcess.Inctance.MainWindow.StartGame(userTake, true);
                                    else
                                        GameProcess.Inctance.MainWindow.DenialGame(userTake);
                                }
                                else if (res)
                                    GameProcess.Inctance.MainWindow.StartGame(userOwer, false);
                                break;
                            case Operation.Stroke: // xod
                                var stroke = Encoding.UTF8.GetString(state.buffer.Skip(1).Take(bytesRead - 1).ToArray());
                                GameProcess.Inctance.RenderOpponentStroke(stroke);
                                break;
                            case Operation.AgreeToDraw:
                                var result = state.buffer[1] == 1;
                                GameProcess.Inctance.MainWindow.AgreeToDraw(result);
                                break;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                try
                {
                    File.AppendAllLines("log_" + GameProcess.Inctance.UserName + ".txt", new string[] { e.Message, e.StackTrace });
                }
                catch (Exception)
                {

                    //throw;
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

        public void Close()
        {
            if (Sock.Connected)
                SendBytes(new byte[] { (byte)Operation.Close });
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
            Console.WriteLine("Sock.Connected = {0}", Sock.Connected);
            Console.WriteLine("SocketAsyncOperation = {0}", e.LastOperation);
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
                Receive(Sock);
                Console.WriteLine("Connected to {0}...", e.RemoteEndPoint.ToString());
                SockAsyncArgs.SetBuffer(buff, 0, buff.Length);
            }
            else
            {
                Console.WriteLine("Dont connect to {0}", e.RemoteEndPoint.ToString());
            }
        }

        public void SubmitGame(User selectedUser)
        {
            if (!Sock.Connected) return;
            var buffer = new List<byte> { (byte)Operation.SubmitGame };
            buffer.AddRange(selectedUser.Guid.ToByteArray());
            buffer.AddRange(buffer);
            SendBytes(buffer.ToArray());
        }

        internal void TakeGame(User user, bool p)
        {
            if (!Sock.Connected) return;
            var buffer = new List<byte> { (byte)Operation.TakeGame, (byte)(p ? 1 : 2) };
            buffer.AddRange(user.Guid.ToByteArray());
            SendBytes(buffer.ToArray());
        }


        internal void SendStroke(string p)
        {
            if (!Sock.Connected) return;
            var buffer = new List<byte> { (byte)Operation.Stroke };
            buffer.AddRange(Encoding.UTF8.GetBytes(p));
            SendBytes(buffer.ToArray());
            GameProcess.Inctance.IsSelfStroke = false;
        }

        internal void OfferDraw()
        {
            if (!Sock.Connected) return;
            SendBytes(new[] { (byte)Operation.OfferDraw });
        }

        internal void GiveUp()
        {
            if (!Sock.Connected) return;
            SendBytes(new[] { (byte)Operation.GiveUp });
        }

        internal void AgreeToDraw(bool res)
        {
            if (!Sock.Connected) return;
            SendBytes(new[] { (byte)Operation.AgreeToDraw, (byte)(res ? 1 : 0) });
        }
    }
}

