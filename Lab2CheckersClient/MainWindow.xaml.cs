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
            GameProcess.Inctance.MainWindow = this;
            GameProcess.Inctance.SetCanvasGame(canvas1);
            MenuItem item1 = new MenuItem();
            item1.Click += new RoutedEventHandler(item1_Click);
            //I have about 10 items
            //...
            item1.Header = "Предложить игру";
            userListContextMenu.Items.Add(item1);

            UsersList.ContextMenu = userListContextMenu;
            userListContextMenu.Opened += new RoutedEventHandler(userListContextMenu_Opened);
        }

        void item1_Click(object sender, RoutedEventArgs e)
        {// Предложить игру
            if (UsersList.SelectedItem == null)
                return;

            var selectedUser = UsersList.SelectedItem as User;
            Client.Current.SubmitGame(selectedUser);
        }

        void userListContextMenu_Opened(object sender, RoutedEventArgs e)
        {
        }


        private void UsersList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

            userListContextMenu.IsOpen = true;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var form = new ConnectForm() { Owner = this };
            form.ShowDialog();
        }

        internal void TakeGame(User user)
        {
            var res = MessageBox.Show("Вам предлагает игру пользователь: " + user.Name + ". Согласиться?",
                                      "Предложение игры", MessageBoxButton.YesNo);
            Client.Current.TakeGame(user, res == MessageBoxResult.Yes);
        }

        internal void StartGame(User user, bool isOwner)
        {
            Dispatcher.Invoke(new Action(() =>
                {
                    GameProcess.Inctance.IsGameOnline = true;
                    GameProcess.Inctance.Opponent = user;
                    GameProcess.Inctance.IsSelfStroke = isOwner;
                    GameProcess.Inctance.StartGame(isOwner);
                }), null);
        }

        internal void DenialGame(User userTake)
        {
            var res = MessageBox.Show("Пользователь: " + userTake.Name + " отказался с вами играть.",
                                     "Предложение игры");
        }
    }
}
