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
using System.Timers;
using Klyuchnikov.Seti.TwoSemestr.CommonLibrary;

namespace Klyuchnikov.Seti.TwoSemestr.Lab2
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            var str = File.ReadAllText("urls.txt");
            textBox1.Text = str;
            ThreadPool.SetMinThreads(10, 10);
            ThreadPool.SetMaxThreads(10, 10);
            label2.Content = 0;
            Model2.Current.OnPropertyChanged("Tasks");
            var timer = new System.Timers.Timer(100);
            timer.Elapsed += delegate
            {
                foreach (var task in Model2.Current.Tasks)
                {
                    task.OnPropertyChanged("ThisThreadState");
                    task.OnPropertyChanged("ThreadIsAlive");
                    Model2.Current.OnPropertyChanged("Tasks");
                }
            };
            timer.Start();
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
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
                ThreadPool.QueueUserWorkItem(Model2.Current.Delegate, s);
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            foreach (Task task in Model2.Current.Tasks)
                if (task.ThisThread.ThreadState.HasFlag(ThreadState.Suspended))
                { task.ThisThread.Resume(); task.OnPropertyChanged("ThisThreadState"); }
        }
        private void button4_Click(object sender, RoutedEventArgs e)
        {
            foreach (Task task in Model2.Current.Tasks)
                if (!task.ThisThread.ThreadState.HasFlag(ThreadState.Stopped))
                { task.ThisThread.Suspend(); task.OnPropertyChanged("ThisThreadState"); }
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
            task.OnPropertyChanged("ThisThreadState");
        }

    }
}
