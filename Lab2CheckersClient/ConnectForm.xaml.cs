using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Lab2CheckersClient
{
    /// <summary>
    /// Логика взаимодействия для ConnectForm.xaml
    /// </summary>
    public partial class ConnectForm : Window
    {
        public ConnectForm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            if (loginTB.Text == "")
            {
                MessageBox.Show("Задайте имя пользователя");
                return;
            }
            if (addressTB.Text == "")
            {
                MessageBox.Show("Задайте адрес сервера");
                return;
            }
            DnsEndPoint endPoint;
            try
            {
                endPoint = new DnsEndPoint(addressTB.Text, int.Parse(portTB.Text));
            }
            catch
            {
                MessageBox.Show("Неверный адрес сервера");
                return;
            }
            GameProcess.Inctance.UserName = loginTB.Text;
            Client.Current.ConnectAsync(endPoint.Host, endPoint.Port);
            this.Close();

        }
    }
}
