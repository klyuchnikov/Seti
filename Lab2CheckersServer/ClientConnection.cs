using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Web.Script.Serialization;

namespace Lab2CheckersServer
{
    internal class ClientConnection : INotifyPropertyChanged
    {
        private static int ClientNumber = 0;

        private Socket Sock;
        private SocketAsyncEventArgs SockAsyncEventArgs;
        private byte[] buff;
        private Timer timer;

        public ClientConnection(Socket AcceptedSocket)
        {
            ClientNumber++;
            buff = new byte[1024];
            Sock = AcceptedSocket;
            SockAsyncEventArgs = new SocketAsyncEventArgs();
            SockAsyncEventArgs.Completed += SockAsyncEventArgs_Completed;
            SockAsyncEventArgs.SetBuffer(buff, 0, buff.Length);

            ReceiveAsync(SockAsyncEventArgs);
            timer = new Timer(100);
            timer.Elapsed += delegate
                {
                    try
                    {
                        if (Sock.Connected)
                            Sock.Send(new byte[] { 0 });
                    }
                    catch (Exception e)
                    {
                        if (UserBind != null)
                            if (UserBind.OpponentGuid != Guid.Empty)
                            {
                                var opponentConnection =
                                    Server.Current.ListConnection.SingleOrDefault(
                                        a => a.UserBind.OpponentGuid == UserBind.Guid);
                                if (opponentConnection != null)
                                    opponentConnection.AbortOpponentConnection();
                                UserBind.OpponentGuid = Guid.Empty;
                            }
                        Server.Current.ListUsers.Remove(UserBind);
                        Sock.Close();
                        Server.Current.ListConnection.Remove(this);
                        foreach (var connection in Server.Current.ListConnection)
                            connection.SendListUsers();
                        Server.Current.SendPropertiesChanged();
                    }
                };
            timer.Start();

        }

        private void AbortOpponentConnection()
        {
            UserBind.OpponentGuid = Guid.Empty;

            SendBytes(new byte[] { 6 });
        }

        ~ClientConnection()
        {
            timer.Stop();
            Console.WriteLine("destructor");
            try { Sock.Close(); }
            catch (Exception) { }
        }

        public User UserBind { get; set; }

        private void SockAsyncEventArgs_Completed(object sender, SocketAsyncEventArgs e)
        {
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
            if (e.SocketError != SocketError.Success) return;
            var sqea = new SocketAsyncEventArgs();
            sqea.Completed += SockAsyncEventArgs_Completed;
            sqea.SetBuffer(buff, 0, buff.Length);
            try
            {
                ReceiveAsync(sqea);
            }
            catch (Exception)
            {
                Console.WriteLine("error ProcessSend");
            }
        }

        private void ProcessReceive(SocketAsyncEventArgs e)
        {
            if (e.SocketError != SocketError.Success || e.Buffer.Length <= 0) return;
            switch ((Operation)e.Buffer[0])
            {
                case Operation.UserName:
                    var guidNewUser = Guid.Parse(Encoding.UTF8.GetString(e.Buffer, 1, 36));
                    var username = Encoding.UTF8.GetString(e.Buffer, 37, e.BytesTransferred - 37);
                    var userNew = Server.Current.ListUsers.SingleOrDefault(a => a.Guid == guidNewUser);
                    if (userNew == null)
                    {
                        userNew = new User()
                            {
                                Guid = guidNewUser,
                                IP = (Sock.LocalEndPoint as IPEndPoint).Address.ToString(),
                                UserName = username
                            };
                        Server.Current.ListUsers.Add(userNew);
                    }
                    UserBind = userNew;
                    foreach (var connection in Server.Current.ListConnection)
                        connection.SendListUsers();
                    Server.Current.SendPropertiesChanged();
                    Console.WriteLine("send userslist");
                    break;
                case Operation.ListUsers:
                    if (e.BytesTransferred == 1)
                        foreach (var connection in Server.Current.ListConnection)
                            connection.SendListUsers();
                    Server.Current.SendPropertiesChanged();
                    break;
                case Operation.Close:
                    if (e.BytesTransferred == 1)
                    {
                        if (UserBind != null)
                            if (UserBind.OpponentGuid != Guid.Empty)
                            {
                                var opponentConnection =
                                    Server.Current.ListConnection.SingleOrDefault(
                                        a => a.UserBind.OpponentGuid == UserBind.Guid);
                                if (opponentConnection != null)
                                    opponentConnection.AbortOpponentConnection();
                            }
                        Server.Current.ListUsers.Remove(UserBind);
                        Sock.Close();
                        Server.Current.ListConnection.Remove(this);
                        Console.WriteLine("close " + UserBind.Guid);
                        foreach (var connection in Server.Current.ListConnection)
                            connection.SendListUsers();
                        Server.Current.SendPropertiesChanged();
                    }
                    break;
                case Operation.SubmitGame: // предложение играть пользователю client
                    Console.WriteLine("send TakeGame");
                    var guidUser = new Guid(e.Buffer.Skip(1).Take(16).ToArray());
                    var client = Server.Current.ListConnection.Single(a => a.UserBind.Guid == guidUser);
                    client.SubmitGame(this.UserBind);
                    break;
                case Operation.TakeGame: // согласился играть с пользователем client
                    var guidOwner = new Guid(e.Buffer.Skip(2).Take(16).ToArray());
                    var result = e.Buffer[1] == 1;
                    var clientOwner = Server.Current.ListConnection.Single(a => a.UserBind.Guid == guidOwner);
                    clientOwner.TakeGame(this.UserBind, clientOwner.UserBind, result); // client - ower
                    this.TakeGame(clientOwner.UserBind, clientOwner.UserBind, result);
                    foreach (var connection in Server.Current.ListConnection)
                        connection.SendListUsers();
                    Console.WriteLine("send StartGame");
                    Server.Current.SendPropertiesChanged();
                    break;
                case Operation.Stroke:
                    Server.Current.ListConnection.Single(a => a.UserBind.OpponentGuid == this.UserBind.Guid).SendBytes(e.Buffer.Take(e.BytesTransferred).ToArray());
                    break;
                case Operation.OfferDraw:
                    var opponent = Server.Current.ListConnection.Single(a => a.UserBind.Guid == UserBind.OpponentGuid);
                    opponent.OfferDraw();
                    break;
                case Operation.GiveUp:
                    var clientOpp = Server.Current.ListConnection.Single(a => a.UserBind.Guid == UserBind.OpponentGuid);
                    clientOpp.GiveUp();
                    foreach (var connection in Server.Current.ListConnection)
                        connection.SendListUsers();
                    Server.Current.SendPropertiesChanged();
                    break;
                case Operation.AgreeToDraw:
                    var res = e.Buffer[1] == 1;
                    var clientOpponent = Server.Current.ListConnection.Single(a => a.UserBind.Guid == UserBind.OpponentGuid);
                    clientOpponent.AgreeToDraw(res);
                    foreach (var connection in Server.Current.ListConnection)
                        connection.SendListUsers();
                    Server.Current.SendPropertiesChanged();
                    break;
                default:
                    string str = Encoding.UTF8.GetString(e.Buffer, 0, e.BytesTransferred);
                    Console.WriteLine("Incoming msg from #{0}: {1}", ClientNumber, str);
                    SendAsync("You send " + str);
                    break;
            }
        }

        private void AgreeToDraw(bool res)
        {
            SendBytes(new[] { (byte)Operation.AgreeToDraw, (byte)(res ? 1 : 0) });
            if (!res) return;
            Server.Current.ListUsers.Single(a => a.Guid == this.UserBind.OpponentGuid).OpponentGuid = Guid.Empty;
            this.UserBind.OpponentGuid = Guid.Empty;
        }

        private void GiveUp()
        {
            SendBytes(new[] { (byte)Operation.GiveUp });
            Server.Current.ListUsers.Single(a => a.Guid == this.UserBind.OpponentGuid).OpponentGuid = Guid.Empty;
            this.UserBind.OpponentGuid = Guid.Empty;
        }

        private void OfferDraw()
        {
            SendBytes(new[] { (byte)Operation.OfferDraw });
        }

        public void SendListUsers()
        {
            JavaScriptSerializer c = new JavaScriptSerializer();
            var str = c.Serialize(Server.Current.ListUsers.Select(a => new { a.Guid, a.IP, a.OpponentGuid, a.UserName, a.UserID }).ToArray());
            var buffer = new List<byte> { 2 };
            buffer.AddRange(Encoding.UTF8.GetBytes(str));
            SendBytes(buffer.ToArray());
            Console.WriteLine("send userslist -> " + UserBind.UserName);
        }

        private void TakeGame(User user, User owner, bool result)
        {
            var buffer = new List<byte> { 4, (byte)(result ? 1 : 2) };

            buffer.AddRange(owner.Guid.ToByteArray());
            buffer.AddRange(user.Guid.ToByteArray());
            if (result)
            {
                this.UserBind.OpponentGuid = user.Guid;
                user.OpponentGuid = this.UserBind.Guid;
            }
            SendBytes(buffer.ToArray());
        }

        private void SubmitGame(User user)
        {
            var buffer = new List<byte> { 3 };
            buffer.AddRange(user.Guid.ToByteArray());
            SendBytes(buffer.ToArray());
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
