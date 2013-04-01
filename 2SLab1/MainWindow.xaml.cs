using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
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
        }
        private byte[] buff;
        private void button1_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                richTextBox1.Document.Blocks.Clear();
                buff = new byte[1024];
                Sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                Sock.Connect(textBox2.Text, 80);
                Sock.Send(Encoding.UTF8.GetBytes(textBox1.Text + "\r\nHost:" + textBox2.Text + "\r\n\r\n"));

                // Create the state object.
                StateObject state = new StateObject();
                state.workSocket = Sock;

                // Begin receiving the data from the remote device.
                Sock.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                  new AsyncCallback(ReceiveCallback), state);
            }
            catch (Exception ex)
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
                var ss = Encoding.ASCII.GetString(state.buffer.Take(bytesRead).ToArray());
                Dispatcher.Invoke(new Action(() => richTextBox1.Document.Blocks.Add(new Paragraph(new Run(ss)))), null);
            }
            catch (Exception exception)
            {
            }
        }

        private Socket Sock;
    }
}
