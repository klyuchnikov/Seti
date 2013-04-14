using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using System.Windows.Threading;
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
            ThreadPool.SetMinThreads(10, 10);
            ThreadPool.SetMaxThreads(10, 10);
            label2.Content = 0;
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

            var count = (int)label2.Content;
            count += arr.Length;
            label2.Content = count;
            if (arr.Any(s => !s.Contains("http://") && !s.Contains("https://")))
            {
                MessageBox.Show("Отсутствует префикс http://");
                return;
            }
            foreach (var s in arr)
            {
                ThreadPool.QueueUserWorkItem(Task.Delegate, s);
            }
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            foreach (Task task in Model2.Current.Tasks)
                task.ThisThread.Resume();
        }
        private void button4_Click(object sender, RoutedEventArgs e)
        {
            foreach (Task task in Model2.Current.Tasks)
                task.ThisThread.Suspend();
        }

        private void button3_Click(object sender, RoutedEventArgs e)
        {
            var form = new ViewResual();
            form.ShowDialog();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var button = (sender as Button);
            var task = (button.DataContext as Task);
            if (task.ThisThread.ThreadState.HasFlag(ThreadState.Suspended))
                task.ThisThread.Resume();
            else
                task.ThisThread.Suspend();
        }

    }
}
