﻿using System;
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

        public ClientConnection(Socket AcceptedSocket)
        {
            ClientNumber++;
            buff = new byte[1024];
            Sock = AcceptedSocket;
            SockAsyncEventArgs = new SocketAsyncEventArgs();
            SockAsyncEventArgs.Completed += SockAsyncEventArgs_Completed;
            SockAsyncEventArgs.SetBuffer(buff, 0, buff.Length);

            ReceiveAsync(SockAsyncEventArgs);
        }

        ~ClientConnection()
        {
            Console.WriteLine("destructor");
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
            if (e.SocketError == SocketError.Success)
            {
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
        }

        private void ProcessReceive(SocketAsyncEventArgs e)
        {
            if (e.SocketError == SocketError.Success && e.Buffer.Length > 0)
            {
                if (e.Buffer[0] == 1)
                {
                    var guid = Guid.Parse(Encoding.UTF8.GetString(e.Buffer, 1, 36));
                    var username = Encoding.UTF8.GetString(e.Buffer, 37, e.BytesTransferred - 37);
                    var user = Server.Current.ListUsers.SingleOrDefault(a => a.Guid == guid);
                    if (user == null)
                    {
                        user = new User() { Guid = guid, IP = (Sock.LocalEndPoint as IPEndPoint).Address.ToString(), UserName = username };
                        Server.Current.ListUsers.Add(user);
                    }
                    UserBind = user;
                    foreach (var connection in Server.Current.ListConnection)
                        connection.SendListUsers();
                    Server.Current.SendPropertiesChanged();
                    Console.WriteLine("set username");

                    BinaryFormatter bf = new BinaryFormatter();
                    MemoryStream ms = new MemoryStream();
                    bf.Serialize(ms, Server.Current.ListUsers.ToArray());
                    var buffer = new List<byte> { 2 };
                    buffer.AddRange(ms.ToArray());
                    SendBytes(buffer.ToArray());
                    Console.WriteLine("send userslist");
                    return;
                }
                if (e.Buffer[0] == 2 && e.BytesTransferred == 1)
                {
                    foreach (var connection in Server.Current.ListConnection)
                        connection.SendListUsers();
                    return;
                }
                if (e.Buffer[0] == 200 && e.BytesTransferred == 1)
                {
                    Server.Current.ListUsers.Remove(UserBind);
                    Sock.Close();
                    Server.Current.ListConnection.Remove(this);
                    Console.WriteLine("close " + UserBind.Guid);
                    foreach (var connection in Server.Current.ListConnection)
                        connection.SendListUsers();
                    return;
                }
                if (e.Buffer[0] == 3) // предложение играть пользователю client
                {
                    Console.WriteLine("send TakeGame");
                    var guid = new Guid(e.Buffer.Skip(1).Take(16).ToArray());
                    var client = Server.Current.ListConnection.Single(a => a.UserBind.Guid == guid);
                    client.SubmitGame(this.UserBind);
                    return;
                }
                if (e.Buffer[0] == 4)// согласился играть с пользователем client
                {
                    var guidOwner = new Guid(e.Buffer.Skip(2).Take(16).ToArray());
                    var result = e.Buffer[1] == 1;
                    var clientOwner = Server.Current.ListConnection.Single(a => a.UserBind.Guid == guidOwner);
                    clientOwner.TakeGame(this.UserBind, clientOwner.UserBind, result); // client - ower
                    this.TakeGame(clientOwner.UserBind, clientOwner.UserBind, result);

                    Console.WriteLine("send StartGame");
                    return;
                }
                string str = Encoding.UTF8.GetString(e.Buffer, 0, e.BytesTransferred);
                Console.WriteLine("Incoming msg from #{0}: {1}", ClientNumber, str);
                SendAsync("You send " + str);
            }
        }

        public void SendListUsers()
        {
            BinaryFormatter bf = new BinaryFormatter();
            MemoryStream ms = new MemoryStream();
            bf.Serialize(ms, Server.Current.ListUsers.ToArray());
            var buffer = new List<byte> { 2 };
            buffer.AddRange(ms.ToArray());

            //   SocketAsyncEventArgs enew = new SocketAsyncEventArgs();
            //   enew.Completed += SockAsyncEventArgs_Completed;
            //   enew.SetBuffer(buffer, 0, buffer.Length);
            // var res = Sock.Send(buffer.ToArray());

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