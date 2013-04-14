using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
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

namespace Klyuchnikov.Seti.TwoSemestr.Lab2
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        static ManualResetEvent starter = new ManualResetEvent(false);
        private RegisteredWaitHandle registeredWaitHandle;
        public MainWindow()
        {
            InitializeComponent();
            var str = File.ReadAllText("urls.txt");
            textBox1.Text = str;
        }

        private static Thread Th;
        /*    public static void Go(object data, bool timedOut)
            {
                Th = Thread.CurrentThread;
                int i = 0;
                while (true)
                {
                    Console.WriteLine(i++.ToString());
                }
                // Выполнение задачи...
            }*/

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            label2.DataContext = Th;
            //   registeredWaitHandle = ThreadPool.RegisterWaitForSingleObject(starter, Go, "привет", -1, true);
            //ThreadPool.
            //  for (int i = 0; i < 15; i++)
            //       ThreadPool.QueueUserWorkItem(JobForAThread, i);
            //   Thread.Sleep(3000);
            //  Console.ReadLine();
            //  starter.Set();
            var arr = textBox1.Text.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var s in arr)
            {
                ThreadPool.QueueUserWorkItem(Delegate, s);
            }
        }
        void Delegate(object state)
        {
            Dispatcher.Invoke(new Action(() =>
            {
                listBox1.Items.Add(state);
            }), null);
            Parser.Start(state);
        }


        private void button2_Click(object sender, RoutedEventArgs e)
        {
          /*  if (Th.ThreadState.HasFlag(ThreadState.WaitSleepJoin))
                Th.Interrupt();
            else
                Th.Abort();//Sleep(Timeout.Infinite);
            //   Th.*/
            int nWorkerThreads;
            int nCompletionThreads;
            ThreadPool.GetMaxThreads(out nWorkerThreads, out nCompletionThreads);
            MessageBox.Show("Максимальное количество потоков: " + nWorkerThreads
                + "\nПотоков ввода-вывода доступно: " + nCompletionThreads);
        }

        private void button3_Click(object sender, RoutedEventArgs e)
        {
            var form = new ViewResual();
            form.ShowDialog();
        }
    }
}
