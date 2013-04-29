using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using Attribute = System.Attribute;

namespace Klyuchnikov.Seti.TwoSemestr.CommonLibrary
{
    /// <summary>
    /// Класс, производящий анализ сайтов
    /// </summary>
    public class Parser
    {
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
        private const string proxyAuth = "Proxy-Authorization:Basic ZC5rbHl1Y2huaWtvdjozNzc0MDc=\r\n";
        /// <summary>
        /// Метод обратного вызова, приемник ответов сервера
        /// </summary>
        /// <param name="ar"></param>
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
                    state.workSocket.BeginReceive(state.buffer, bytesRead, client.Available, 0, new AsyncCallback(ReceiveCallback), state);
                    return;
                }
                var arr = state.buffer.TakeWhile(q => q != 0).ToArray();

            }
            catch (Exception exception)
            {
            }
        }

        /// <summary>
        /// Старт анализа сайта
        /// </summary>
        /// <param name="state">url сайта</param>
        public static void Start(object state)
        {
            try
            {
                var url = (string)state;
                var Sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                var uri = new Uri(url);
                //     Sock.Connect("proxy.ustu", 3128);
                Sock.Connect(uri.Host, 80);
                //    Sock.Send(Encoding.Default.GetBytes("CONNECT users.ulstu.ru HTTP/1.0\r\n"));
                Sock.Send(Encoding.Default.GetBytes(String.Format("GET {0} HTTP/1.1\r\nHost:{1}\r\n{2}\r\n", uri.AbsolutePath, uri.Host, proxyAuth)), SocketFlags.Partial);
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
                ParseDocument(arr, url);
            }
            catch (Exception exception)
            {

            }
        }
        /// <summary>
        /// Разбор документа
        /// </summary>
        /// <param name="arr">массив байт документа</param>
        /// <param name="url">url документа</param>
        private static void ParseDocument(byte[] arr, string url)
        {
            var ss = Encoding.Default.GetString(arr);
            var charset = Regex.Match(ss, @"charset=(?<charset>[\w-]*)", RegexOptions.IgnoreCase).Groups["charset"].Value;
            if (!String.IsNullOrEmpty(charset))
                ss = Encoding.GetEncoding(charset).GetString(arr);
            var title = Regex.Match(ss, @"<title>(?<title>[\d\D]*?)</title>", RegexOptions.IgnoreCase).Groups["title"].Value;
            title = title.Replace("\n", "").Replace("\r", "");
            var doc = new Document(title, url);
            ParseTags(ss, doc, "h\\d");
            ParseTags(ss, doc, "p");
            ParseKeywords(ss, doc);
            Model.Current.documents.Add(doc);
            Model.Current.Documents = null;
        }

        /// <summary>
        /// Разбор клучевых слов в документе
        /// </summary>
        /// <param name="str">документ</param>
        /// <param name="doc">объектная модель докумнета</param>
        private static void ParseKeywords(string str, Document doc)
        {
            var match = Regex.Match(str, "<meta name=\"keywords\" content=\"(?<content>.*?)\"\\s*?/>", RegexOptions.IgnoreCase);
            var keywords = match.Groups["content"].Value.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var keyword in keywords)
            {
                doc.Keywords.Add(keyword.Trim());
            }
        }
        /// <summary>
        /// Разбор тегов
        /// </summary>
        /// <param name="str">документ</param>
        /// <param name="doc">объектная модель докумнета</param>
        /// <param name="pattern">шаблон разбора</param>
        private static void ParseTags(string str, Document doc, string pattern)
        {
            var matches = Regex.Matches(str, String.Format("<{0}\\s*(?<attr>.*?)>(?<value>[\\d\\D]*?)</(?<tag>{0})>", pattern), RegexOptions.IgnoreCase);
            foreach (Match h in matches)
            {
                var tagS = h.Groups["tag"].Value;
                var value = h.Groups["value"].Value.Replace("\n", "").Replace("\r", "");
                var tag = new Tag(doc, tagS, value);
                ParseAttributes(tag, h.Groups["attr"].Value);
                Model.Current.Tags.Add(tag);
            }
        }
        /// <summary>
        /// Разбор тегов
        /// </summary>
        /// <param name="str">документ</param>
        /// <param name="doc">объектная модель докумнета</param>
        /// <param name="pattern">шаблон разбора</param>
        private static void ParseTagsWhitoutBody(string str, Document doc, string pattern)
        {
            var matches = Regex.Matches(str, String.Format("<(?<tag>{0})\\s*(?<attr>.*?)/>", pattern), RegexOptions.IgnoreCase);
            foreach (Match h in matches)
            {
                var tagS = h.Groups["tag"].Value;
                var tag = new Tag(doc, tagS, null);
                ParseAttributes(tag, h.Groups["attr"].Value);
                Model.Current.Tags.Add(tag);
            }
        }
        /// <summary>
        /// Разбор аттрибутов
        /// </summary>
        /// <param name="tag">объектная модель тега</param>
        /// <param name="str">документ</param>
        private static void ParseAttributes(Tag tag, string str)
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
    }
}
