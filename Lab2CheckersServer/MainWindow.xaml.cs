using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Lab2CheckersServer
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

        private void StartB_Click(object sender, RoutedEventArgs e)
        {
            short port;
            if (!short.TryParse(portTB.Text, out port))
            {
                MessageBox.Show("Неправильно задан порт");
                return;
            }
            Server.Current.Start(port);
            StartB.IsEnabled = false;
        }

        private void StopB_Click(object sender, RoutedEventArgs e)
        {
            Server.Current.Stop();
            StartB.IsEnabled = true;
        }
    }
}
