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

namespace Lab2CheckersClient
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ContextMenu userListContextMenu = new ContextMenu();
        public MainWindow()
        {
            InitializeComponent();
            GameProcess.Inctance.SetCanvasGame(canvas1);
            MenuItem item1 = new MenuItem();
            MenuItem item2 = new MenuItem();

            //I have about 10 items
            //...
            item1.Header = "item1";
            userListContextMenu.Items.Add(item1);

            item2.Header = "item2";
            userListContextMenu.Items.Add(item2);
            UsersList.ContextMenu = userListContextMenu;

        }


        private void button1_Click(object sender, RoutedEventArgs e)
        {
            GameProcess.Inctance.IsConnected = !GameProcess.Inctance.IsConnected;
        }

        private void UsersList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

            userListContextMenu.IsOpen = true;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            GameProcess.Inctance.IsConnected = true;
        }
    }
}
