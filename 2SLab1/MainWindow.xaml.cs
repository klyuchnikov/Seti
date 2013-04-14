using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Klyuchnikov.Seti.TwoSemestr.CommonLibrary;

namespace Klyuchnikov.Seti.TwoSemestr.Lab1
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        public class StateObject
        {
            // Client socket.
            public Socket workSocket = null;
            // Size of receive buffer.
            public const int BufferSize = 1024 * 3200;
            // Receive buffer.
            public byte[] buffer = new byte[BufferSize];
            // Received data string.
            public StringBuilder sb = new StringBuilder();

            public string URL;
        }
        private byte[] buff;
        const string proxyAuth = "Proxy-Authorization:Basic ZC5rbHl1Y2huaWtvdjozNzc0MDc=\r\n";
        private void button1_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!textBox2.Text.Contains("http://") && !textBox2.Text.Contains("https://"))
                {
                    MessageBox.Show("Отсутствует префикс http://");
                    return;
                }
                buff = new byte[1024];
                Sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                var uri = new Uri(textBox2.Text);
                //     Sock.Connect("proxy.ustu", 3128);
                Sock.Connect(uri.Host, 80);
                //    Sock.Send(Encoding.Default.GetBytes("CONNECT users.ulstu.ru HTTP/1.0\r\n"));
                Sock.Send(Encoding.Default.GetBytes(string.Format("GET {0} HTTP/1.1\r\nHost:{1}\r\n{2}\r\n", uri.AbsolutePath, uri.Host, proxyAuth)), SocketFlags.Partial);
                //  Sock.Send(Encoding.Default.GetBytes("GET / HTTP/1.0\r\nHost:users.ulstu.ru\r\n\r\n" + proxyAuth));
                Sock.ReceiveBufferSize = 5000000;
                byte[] buffer = new byte[Sock.ReceiveBufferSize];
                var bytesRead = 0;
                var contentLenght = 0;
                byte[] arr;
                var str = "";
                do
                {
                    if (bytesRead == 0)
                        bytesRead += Sock.Receive(buffer);
                    else
                        //   if (contentLenght - bytesRead ==)
                        if (Sock.Available > 0)
                            bytesRead += Sock.Receive(buffer, bytesRead, Sock.Available, SocketFlags.None);
                    // else
                    //      if (contentLenght - bytesRead < 20)
                    //          break;
                    arr = buffer.TakeWhile(q => q != 0).ToArray();
                    str = Encoding.Default.GetString(arr);
                } while (!str.Contains("</html>"));

                arr = buffer.TakeWhile(q => q != 0).ToArray();
                var ss = Encoding.Default.GetString(arr);
                Parser.ParseDocument(arr, textBox2.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + Environment.NewLine + ex.StackTrace, "Error");
                Console.WriteLine(e.ToString());
            }
        }

        public void ReceiveCallback(IAsyncResult ar)
        {
            try
            {
                var state = (StateObject)ar.AsyncState;
                var client = state.workSocket;
                var cc = client.Available;
                int bytesRead = client.EndReceive(ar);
                if (cc != 0)
                {
                    var uri = new Uri(state.URL);
                    Sock.BeginReceive(state.buffer, bytesRead, client.Available, 0, new AsyncCallback(ReceiveCallback), state);
                    return;
                }
                var arr = state.buffer.TakeWhile(q => q != 0).ToArray();

            }
            catch (Exception exception)
            {
            }
        }

        private Socket Sock;
    }
}
