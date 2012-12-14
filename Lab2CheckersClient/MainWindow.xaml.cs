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
        private ContextMenu userListContextMenu = new ContextMenu();

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

        private void item1_Click(object sender, RoutedEventArgs e)
        {
            // Предложить игру
            if (UsersList.SelectedItem == null)
                return;

            var selectedUser = UsersList.SelectedItem as User;
            if (selectedUser.Guid != Client.Current.Guid)
                Client.Current.SubmitGame(selectedUser);
            //    else
            //       MessageBox.Show("")
        }

        private void userListContextMenu_Opened(object sender, RoutedEventArgs e)
        {
            if (UsersList.SelectedItem == null)
                return;
            var selectedUser = UsersList.SelectedItem as User;
            if (selectedUser.Guid == Client.Current.Guid)
                userListContextMenu.IsOpen = false;
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
            var res = MessageBox.Show("Вам предлагает игру пользователь: " + user.UserName + ". Согласиться?",
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
            var res = MessageBox.Show("Пользователь: " + userTake.UserName + " отказался с вами играть.",
                                      "Предложение игры");
        }

        internal void AbortOpponentConnection()
        {
            Dispatcher.Invoke(new Action(() =>
                {
                    MessageBox.Show("Противник потерял связь с сервером! Игра окончена");
                    GameProcess.Inctance.IsGameOnline = false;
                    GameProcess.Inctance.Opponent = null;
                    GameProcess.Inctance.IsSelfStroke = true;
                    GameProcess.Inctance.StopGame();
                }), null);
        }

        private void OfferDraw_OnClick(object sender, RoutedEventArgs e)
        {
            var res = MessageBox.Show("Вы хотите предложить противнику ничью?", "Внимание", MessageBoxButton.YesNo);
            if (res == MessageBoxResult.Yes)
            {
                Client.Current.OfferDraw();
            }
        }

        private void GiveUp_OnClick(object sender, RoutedEventArgs e)
        {
            Dispatcher.Invoke(new Action(() =>
                {
                    var res = MessageBox.Show("Вы хотите сдаться?", "Внимание", MessageBoxButton.YesNo);
                    if (res != MessageBoxResult.Yes) return;
                    Client.Current.GiveUp();
                    MessageBox.Show("Вы сдались! Вы проиграли!");

                    GameProcess.Inctance.IsGameOnline = false;
                    GameProcess.Inctance.Opponent = null;
                    GameProcess.Inctance.IsSelfStroke = true;
                    GameProcess.Inctance.StopGame();
                }), null);
        }

        internal void GiveUpOpponent()
        {
            Dispatcher.Invoke(new Action(() =>
            {
                MessageBox.Show("Противник сдался! Вы победили");
                GameProcess.Inctance.IsGameOnline = false;
                GameProcess.Inctance.Opponent = null;
                GameProcess.Inctance.IsSelfStroke = true;
                GameProcess.Inctance.StopGame();
            }), null);
        }

        internal void OfferDraw()
        {
            Dispatcher.Invoke(new Action(() =>
            {
                var res = MessageBox.Show("Противник предлагает вам ничью! Вы согласитесь?", "Ничья", MessageBoxButton.YesNo) == MessageBoxResult.Yes;
                if (res)
                {
                    MessageBox.Show("Игра закончена ничьей!");

                    GameProcess.Inctance.IsGameOnline = false;
                    GameProcess.Inctance.Opponent = null;
                    GameProcess.Inctance.IsSelfStroke = true;
                    GameProcess.Inctance.StopGame();
                }
                Client.Current.AgreeToDraw(res);
            }), null);
        }

        internal void AgreeToDraw(bool result)
        {
            Dispatcher.Invoke(new Action(() =>
            {
                if (result)
                {
                    MessageBox.Show("Противник согласился закончить игру ничьей.");

                    GameProcess.Inctance.IsGameOnline = false;
                    GameProcess.Inctance.Opponent = null;
                    GameProcess.Inctance.IsSelfStroke = true;
                    GameProcess.Inctance.StopGame();
                }
                else
                {
                    MessageBox.Show("Противник отказался от ничьи!");
                }
            }), null);
        }
    }
}
