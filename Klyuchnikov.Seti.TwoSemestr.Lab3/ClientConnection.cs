using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows;
using Klyuchnikov.Seti.TwoSemestr.CommonLibrary;
using Timer = System.Timers.Timer;

namespace Klyuchnikov.Seti.TwoSemestr.Lab3
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
            //  SendBytes(new byte[] { 244 });
            SendBytes(Encoding.Default.GetBytes("Enter login: "));
            lastCommand = Operation.Login;
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

                }
            };
            timer.Start();

        }

        private void AbortOpponentConnection()
        {
            SendBytes(new byte[] { 6 });
        }

        ~ClientConnection()
        {
            timer.Stop();
            Console.WriteLine("destructor");
            try { Sock.Close(); }
            catch (Exception) { }
        }

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

        private byte[] buf;
        private string lastString = "";
        private Operation lastCommand;
        private string login = "";
        private Queue<string> ConsoleOutput = new Queue<string>();
        private void ProcessReceive(SocketAsyncEventArgs e)
        {
            if (e.BytesTransferred == 2 && e.Buffer[0] == 13 && e.Buffer[1] == 10)
            {
                switch (lastCommand)
                {
                    case Operation.Login:
                        if (lastString == "")
                        {
                            SendBytes(Encoding.Default.GetBytes("Enter login: "));
                        }
                        else
                        {
                            login = lastString;
                            SendBytes(Encoding.Default.GetBytes("Enter password for " + lastString + ": "));
                            lastCommand = Operation.Password;
                        }
                        break;
                    case Operation.Password:
                        if (lastString == "admin" && login == "admin")
                        {
                            SendBytes(Encoding.Default.GetBytes("Authentication is successful!\r\n"));
                            SendBytes(Encoding.Default.GetBytes("Available commands:\r\n"));
                            SendBytes(Encoding.Default.GetBytes("dp  DislayProcessed     Request a list of the analyzed site pages.\r\n"));
                            SendBytes(Encoding.Default.GetBytes("r RequestInfoSite     Request for information about the text to the address.\r\n"));
                            SendBytes(Encoding.Default.GetBytes("d DeleteInfoSite      Deleting information on the address page.\r\n"));
                            SendBytes(Encoding.Default.GetBytes("l LaunchInfoSite      Launch site analysis at the specified address.\r\n"));
                            SendBytes(Encoding.Default.GetBytes("s StopInfoSite        Stopping the analysis of the site at the address.\r\n"));
                            SendBytes(Encoding.Default.GetBytes(login + ">"));
                            lastCommand = Operation.WaitOperation;
                        }
                        else
                        {
                            SendBytes(Encoding.Default.GetBytes("Authentication failed!\r\nEnter login: "));
                            lastCommand = Operation.Login;
                        }
                        break;
                    case Operation.WaitOperation:
                        if (lastString == "")
                            SendBytes(Encoding.Default.GetBytes(login + ">"));
                        else
                        {
                            var arr = lastString.Split(' ');
                            if (arr[0] == "dp" || arr[0] == "DislayProcessed")
                            {
                                ConsoleOutput.Enqueue("Sites Processed Count: " + Model.Current.Documents.Length + "\r\n");
                                foreach (var document in Model.Current.Documents)
                                {
                                    ConsoleOutput.Enqueue("Index database: " + document.ID + "\r\n");
                                    ConsoleOutput.Enqueue("url: " + document.URL + "\r\n");
                                    ConsoleOutput.Enqueue("title: " + document.Name + "\r\n");
                                    ConsoleOutput.Enqueue("Keywords count: " + document.Keywords.Count + "\r\n");
                                    ConsoleOutput.Enqueue("Tags count: " + document.Tags.Length + "\r\n");
                                }
                                var countConsoleOutput = ConsoleOutput.Count;
                                for (int i = 0; i < (countConsoleOutput > 23 ? 23 : countConsoleOutput); i++)
                                    SendBytes(Encoding.Default.GetBytes(ConsoleOutput.Dequeue()));
                                if (ConsoleOutput.Count > 0)
                                {
                                    SendBytes(Encoding.Default.GetBytes("Press enter any key to continue output..."));
                                    lastCommand = Operation.WaitOfContinued;
                                }
                                else
                                {
                                    SendBytes(Encoding.Default.GetBytes("\r\n" + login + ">"));
                                    lastCommand = Operation.WaitOperation;
                                }
                            }
                            else if (arr[0] == "r" || arr[0] == "RequestInfoSite")
                            {
                                if (arr.Length < 2)
                                { SendBytes(Encoding.Default.GetBytes("RequestInfoSite: argument invalid!\r\n" + login + ">")); break; }
                                Document document = null;
                                try
                                {
                                    var ind = int.Parse(arr[1]);
                                    document = Model.Current.Documents.Single(q => q.ID == ind);
                                }
                                catch (Exception)
                                { SendBytes(Encoding.Default.GetBytes("LaunchInfoSite: argument invalid!\r\n" + login + ">")); break; }
                                ConsoleOutput.Enqueue("Index database: " + document.ID + "\r\n");
                                ConsoleOutput.Enqueue("url: " + document.URL + "\r\n");
                                ConsoleOutput.Enqueue("title: " + document.Name + "\r\n");
                                ConsoleOutput.Enqueue("Keywords count: " + document.Keywords.Count + "\r\n");
                                foreach (var keyword in document.Keywords)
                                    ConsoleOutput.Enqueue("   keyword: " + keyword + "\r\n");
                                ConsoleOutput.Enqueue("Tags count: " + document.Tags.Length + "\r\n");
                                foreach (var tag in document.Tags)
                                {
                                    ConsoleOutput.Enqueue("   " + tag.Name + ": " + tag.Value + "\r\n");
                                    foreach (var att in tag.Attributes)
                                        ConsoleOutput.Enqueue("     " + att.Name + ": " + att.Value + "\r\n");
                                }
                                var countConsoleOutput = ConsoleOutput.Count;
                                for (int i = 0; i < (countConsoleOutput > 23 ? 23 : countConsoleOutput); i++)
                                    SendBytes(Encoding.Default.GetBytes(ConsoleOutput.Dequeue()));
                                if (ConsoleOutput.Count > 0)
                                {
                                    SendBytes(Encoding.Default.GetBytes("Press enter any key to continue output..."));
                                    lastCommand = Operation.WaitOfContinued;
                                }
                                else
                                {
                                    SendBytes(Encoding.Default.GetBytes("\r\n" + login + ">"));
                                    lastCommand = Operation.WaitOperation;
                                }

                            }
                            else if (arr[0] == "d" || arr[0] == "DeleteInfoSite")
                            {

                            }
                            else if (arr[0] == "l" || arr[0] == "LaunchInfoSite")
                            {
                                if (arr.Length < 2)
                                { SendBytes(Encoding.Default.GetBytes("LaunchInfoSite: argument invalid!\r\n" + login + ">")); break; }
                                if (!arr[1].Contains("http://") && !arr[1].Contains("https://"))
                                { SendBytes(Encoding.Default.GetBytes("LaunchInfoSite: no prefix http or https !\r\n" + login + ">")); break; }
                                else
                                {
                                    ThreadPool.QueueUserWorkItem(Model2.Current.Delegate, arr[1].Trim());
                                    SendBytes(Encoding.Default.GetBytes("Launch site analysis " + arr[1] + "...\r\n" + login + ">"));
                                }
                            }
                            else if (arr[0] == "s" || arr[0] == "StopInfoSite")
                            {

                            }
                            else
                            {
                                SendBytes(Encoding.Default.GetBytes("Operation invalid!\r\n"));
                                SendBytes(Encoding.Default.GetBytes(login + ">"));
                            }
                            // SendBytes(Encoding.Default.GetBytes(login + ">"));
                        }
                        break;
                    case Operation.WaitOfContinued:
                        SendBytes(new byte[] { 13, 32, 32, 32, 32, 32, 32, 32, 32, 32, 32, 32, 32, 32, 32, 32, 32, 32, 32, 32, 32, 32, 32, 32, 32, 32, 32, 32, 32, 32, 32, 32, 32, 32, 32, 32, 32, 32, 32, 32, 32, 32, 32, 32, 32, 13 });
                        var count = ConsoleOutput.Count;
                        for (int i = 0; i < (count > 23 ? 23 : count); i++)
                            SendBytes(Encoding.Default.GetBytes(ConsoleOutput.Dequeue()));
                        if (ConsoleOutput.Count == 0)
                        { lastCommand = Operation.WaitOperation; SendBytes(Encoding.Default.GetBytes(login + ">")); }
                        break;
                }
                lastString = "";
            }
            else if (e.BytesTransferred == 1 && e.Buffer[0] == 8)
            {
                if (lastString.Length > 0)
                    lastString = lastString.Substring(0, lastString.Length - 1);
                SendBytes(new byte[] { 32, 8 });
            }
            else
            {
                var arr = e.Buffer.Take(e.BytesTransferred).TakeWhile(a => a != 0).ToArray();
                var str = Encoding.Default.GetString(arr);
                lastString += str;
                Console.WriteLine(str); SendBytes(new byte[] { 0x00 });
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
            byte[] buff = Encoding.Default.GetBytes(data);
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
