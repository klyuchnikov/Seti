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

namespace _2SLab1
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
                    // Sock.Poll(1000, SelectMode.SelectRead);
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
                    //   var contentLenghtS =
                    //       Regex.Match(str, @"Content-Length: (?<Length>\d*)", RegexOptions.IgnoreCase).Groups["Length"]
                    //           .Value;
                    //   contentLenght = int.Parse(contentLenghtS) + str.IndexOf("<");
                } while (!str.Contains("</html>"));

                arr = buffer.TakeWhile(q => q != 0).ToArray();
                var ss = Encoding.Default.GetString(arr);
                // Create the state object.
                // Begin receiving the data from the remote device.
            //    MessageBox.Show(ss.Length.ToString());
                Parser(arr, textBox2.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
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
                    //    Sock.Send(Encoding.Default.GetBytes(string.Format(
                    //        "GET {0} HTTP/1.1\r\nHost:{1}\r\n{2}\r\nProxy-Connection:keep-alive\r\n", uri.AbsolutePath, uri.Host, proxyAuth)));
                    Sock.BeginReceive(state.buffer, bytesRead, client.Available, 0, new AsyncCallback(ReceiveCallback), state);
                    return;
                }
                var arr = state.buffer.TakeWhile(q => q != 0).ToArray();

            }
            catch (Exception exception)
            {
            }
        }

        void Parser(byte[] arr, string url)
        {
            var ss = Encoding.Default.GetString(arr);
            /* if (ss.Contains("HTTP/1.0 200 Connection established"))
             {
                 Dispatcher.Invoke(new Action(() =>
                 {
                     var uri = new Uri(textBox2.Text);
                     Sock.Send(Encoding.Default.GetBytes(string.Format(
                         "GET {0} HTTP/1.1\r\nHost:{1}\r\n{2}\r\nProxy-Connection:keep-alive\r\n", uri.AbsolutePath, uri.Host, proxyAuth)));
                     Sock.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReceiveCallback), state);
                 }), null);
                 return;
             }*/
            var charset = Regex.Match(ss, @"charset=(?<charset>[\w-]*)", RegexOptions.IgnoreCase).Groups["charset"].Value;
            if (!string.IsNullOrEmpty(charset))
                ss = Encoding.GetEncoding(charset).GetString(arr);
         //   Dispatcher.Invoke(new Action(() => richTextBox1.Document.Blocks.Add(new Paragraph(new Run(ss)))), null);
            //Dispatcher.Invoke(new Action(() => richTextBox1.Document.Blocks.Add(new Paragraph(new Run(charset)))), null);

            var title = Regex.Match(ss, @"<title>(?<title>[\d\D]*)</title>", RegexOptions.IgnoreCase).Groups["title"].Value;
            var doc = new Document(title, url);

            Model.Current.Document = doc;
            var matches = Regex.Matches(ss,
                "<h\\d\\s*(?<attr>.*?)>(?<value>[\\d\\D]*?)</(?<tag>h\\d)>", RegexOptions.IgnoreCase);
            foreach (Match h in matches)
            {
                var tagS = h.Groups["tag"].Value;
                var value = h.Groups["value"].Value.Replace("\n", "").Replace("\r", "");
                var tag = new Tag(doc, tagS, value);
                var attrsS = h.Groups["attr"].Value.Replace("\n", "").Replace("\r", "");
                var ms = Regex.Matches(attrsS, "(?<name>.*)=\"(?<value>.*)\"", RegexOptions.IgnoreCase);
                foreach (Match m in ms)
                {
                    var name = m.Groups["name"].Value;
                    var valueA = m.Groups["value"].Value;
                    var newatt = new Attribute(tag, name, valueA);
                    Model.Current.Attributes.Add(newatt);
                  //  Model.Current.Attributes = null;
                }
                Model.Current.Tags.Add(tag);
              //  Model.Current.Tags = null;
            }
            Model.Current.documents.Add(doc);
            Model.Current.Documents = null;
            Dispatcher.Invoke(new Action(() =>
            {
                listBox1.ItemsSource = Model.Current.Documents;
             //   MessageBox.Show(ss.Length.ToString());
            }), null);
        }
        private Socket Sock;
    }
}
