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
            public const int BufferSize = 1024 * 32;
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
                richTextBox1.Document.Blocks.Clear();
                buff = new byte[1024];
                Sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                var uri = new Uri(textBox2.Text);
                Sock.Connect("proxy.ustu", 3128);
                //Sock.Connect(uri.Host, 80);
                Sock.Send(Encoding.Default.GetBytes("CONNECT users.ulstu.ru HTTP/1.0\r\n"));
                Sock.Send(Encoding.Default.GetBytes(string.Format("GET {0} HTTP/1.1\r\nHost:{1}\r\n{2}\r\n", uri.AbsolutePath, uri.Host, proxyAuth)));
                //  Sock.Send(Encoding.Default.GetBytes("GET / HTTP/1.0\r\nHost:users.ulstu.ru\r\n\r\n" + proxyAuth));

                // Create the state object.
                var state = new StateObject { workSocket = Sock, URL = textBox2.Text };
                Sock.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                 new AsyncCallback(ReceiveCallback), state);

                // Begin receiving the data from the remote device.

            }
            catch (Exception ex)
            {
                Console.WriteLine(e.ToString());
            }
        }

        public void ReceiveCallback(IAsyncResult ar)
        {
            var state = (StateObject)ar.AsyncState;
            var client = state.workSocket;
            int bytesRead = client.EndReceive(ar);
            try
            {
                var ss = Encoding.Default.GetString(state.buffer.Take(bytesRead).ToArray());
                if (ss.Contains("HTTP/1.0 200 Connection established"))
                {
                    Dispatcher.Invoke(new Action(() =>
                                                     {
                                                         var uri = new Uri(textBox2.Text);
                                                         Sock.Send(Encoding.Default.GetBytes(string.Format("GET {0} HTTP/1.1\r\nHost:{1}\r\n{2}\r\nProxy-Connection:keep-alive\r\n", uri.AbsolutePath, uri.Host, proxyAuth)));
                                                         Sock.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReceiveCallback), state);
                                                     }), null);
                    return;
                }
                var charset = Regex.Match(ss, @"charset=(?<charset>[\w-]*)", RegexOptions.IgnoreCase).Groups["charset"].Value;
                if (!string.IsNullOrEmpty(charset))
                    ss = Encoding.GetEncoding(charset).GetString(state.buffer.Take(bytesRead).ToArray());
                Dispatcher.Invoke(new Action(() => richTextBox1.Document.Blocks.Add(new Paragraph(new Run(ss)))), null);
                //Dispatcher.Invoke(new Action(() => richTextBox1.Document.Blocks.Add(new Paragraph(new Run(charset)))), null);

                var title = Regex.Match(ss, @"<title>(?<title>.*)</title>", RegexOptions.IgnoreCase).Groups["title"].Value;
                var doc = new Document(title, state.URL);

                Model.Current.Document = doc;
                var matches = Regex.Matches(ss, "<h\\d\\s*(?<attr>.*)>(?<value>.*)</(?<tag>h\\d)>", RegexOptions.IgnoreCase);
                foreach (Match h in matches)
                {
                    var tagS = h.Groups["tag"].Value;
                    var value = h.Groups["value"].Value;
                    var tag = new Tag(doc, tagS, value);
                    var attrsS = h.Groups["attr"].Value;
                    var attrs = attrsS.Trim().Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
                    var attr = attrs.Select(a => new Attribute(tag, a.Split('=')[0], a.Split('=')[1].Replace("\"", ""))).ToArray();
                    Model.Current.Attributes.AddRange(attr);
                    Model.Current.Attributes = null;
                    Model.Current.Tags.Add(tag);
                    Model.Current.Tags = null;
                }
                Model.Current.documents.Add(doc);
                Model.Current.Documents = null;
                Dispatcher.Invoke(new Action(() =>
                {
                    listBox1.ItemsSource = Model.Current.Documents;
                }), null);
            }
            catch (Exception exception)
            {
            }
        }

        private Socket Sock;
    }
}
