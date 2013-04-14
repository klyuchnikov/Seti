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
                Parser(arr, textBox2.Text);
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

        void Parser(byte[] arr, string url)
        {
            var ss = Encoding.Default.GetString(arr);
            var charset = Regex.Match(ss, @"charset=(?<charset>[\w-]*)", RegexOptions.IgnoreCase).Groups["charset"].Value;
            if (!string.IsNullOrEmpty(charset))
                ss = Encoding.GetEncoding(charset).GetString(arr);
            var title = Regex.Match(ss, @"<title>(?<title>[\d\D]*)</title>", RegexOptions.IgnoreCase).Groups["title"].Value;
            var doc = new Document(title, url);
            ParseTags(ss, doc, "h\\d");
            ParseTags(ss, doc, "p");
            ParseKeywords(ss, doc);
            Model.Current.documents.Add(doc);
            Model.Current.Documents = null;
        }
        private void ParseKeywords(string str, Document doc)
        {
            var match = Regex.Match(str, "<meta name=\"keywords\" content=\"(?<content>.*?)\"\\s*?/>", RegexOptions.IgnoreCase);
            var keywords = match.Groups["content"].Value.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries);
            foreach (var keyword in keywords)
            {
                doc.Keywords.Add(keyword.Trim());
            }
        }

        private void ParseTags(string str, Document doc, string pattern)
        {
            var matches = Regex.Matches(str, string.Format("<{0}\\s*(?<attr>.*?)>(?<value>[\\d\\D]*?)</(?<tag>{0})>", pattern), RegexOptions.IgnoreCase);
            foreach (Match h in matches)
            {
                var tagS = h.Groups["tag"].Value;
                var value = h.Groups["value"].Value.Replace("\n", "").Replace("\r", "");
                var tag = new Tag(doc, tagS, value);
                ParseAttributes(tag, h.Groups["attr"].Value);
                Model.Current.Tags.Add(tag);
            }
        }
        private void ParseTagsWhitoutBody(string str, Document doc, string pattern)
        {
            var matches = Regex.Matches(str, string.Format("<(?<tag>{0})\\s*(?<attr>.*?)/>", pattern), RegexOptions.IgnoreCase);
            foreach (Match h in matches)
            {
                var tagS = h.Groups["tag"].Value;
                var tag = new Tag(doc, tagS, null);
                ParseAttributes(tag, h.Groups["attr"].Value);
                Model.Current.Tags.Add(tag);
            }
        }

        private void ParseAttributes(Tag tag, string str)
        {
            var attrsS = str.Replace("\n", "").Replace("\r", "");
            var ms = Regex.Matches(attrsS, "\\s*?(?<name>\\S*?)=\"(?<value>.*?)\"", RegexOptions.IgnoreCase);
            foreach (var newatt in from Match m in ms
                                   let name = m.Groups["name"].Value
                                   let valueA = m.Groups["value"].Value
                                   select new Attribute(tag, name, valueA))
            {
                Model.Current.Attributes.Add(newatt);
            }
        }

        private Socket Sock;
    }
}
