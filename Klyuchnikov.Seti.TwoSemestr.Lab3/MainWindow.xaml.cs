using System;
using System.Collections.Generic;
using System.Linq;
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

namespace Klyuchnikov.Seti.TwoSemestr.Lab3
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

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            int port;
            if (portTB.Text == "")
            {
                MessageBox.Show("Введите порт!");
                return;
            }
            if (!int.TryParse(portTB.Text, out port))
            {
                MessageBox.Show("Порт введен некорректно:");
                return;
            }
            if (!Server.Current.IsOpen)
            {
                Server.Current.Start(port);
                button1.Content = "Стоп";
                portTB.IsEnabled = false;
            }
            else
            {
                Server.Current.Stop();
                button1.Content = "Старт";
                portTB.IsEnabled = true;
            }

        }
    }
}
