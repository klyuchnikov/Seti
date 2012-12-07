using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows.Input;

namespace Lab2CheckersClient
{
    public sealed class GameProcess : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public Canvas CanvasGame { get; private set; }

        /// <summary>
        /// В данный момент выполняется ход
        /// </summary>
        public bool IsRunStroke { get; set; }

        public Checker RunStrokeChecker { get; set; }

        /// <summary>
        /// Чей ход в данный момент, если true - ход мой, false - ход противника
        /// </summary>
        public bool IsSelfStroke { get; set; }

        private GameProcess()
        {
            users = new List<User>();
            checkersSelf = new List<Checker>();
            checkersOpponent = new List<Checker>();
            users.Add(new User("qwe"));
            users.Add(new User("cvxc"));
            users.Add(new User("vxfg"));
        }

        private void RenderCheckers()
        {
            foreach (var checker in checkersSelf)
            {
                Canvas.SetLeft(checker.ImageFigure, 18 + checker.Position.X * 48);
                Canvas.SetTop(checker.ImageFigure, 18 + checker.Position.Y * 48);
            }
            foreach (var checker in checkersOpponent)
            {
                Canvas.SetLeft(checker.ImageFigure, 18 + checker.Position.X * 48);
                Canvas.SetTop(checker.ImageFigure, 18 + checker.Position.Y * 48);
            }
        }
        private void RenderChecker(Checker checker)
        {
            Canvas.SetLeft(checker.ImageFigure, 18 + checker.Position.X * 48);
            Canvas.SetTop(checker.ImageFigure, 18 + checker.Position.Y * 48);
        }

        public void SetCanvasGame(Canvas canvas)
        {
            this.CanvasGame = canvas;
            GenChecker();
            RenderCheckers();
            canvas.MouseDown += canvas_MouseDown;
        }

        void canvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (IsRunStroke)
            {
                var point = Mouse.GetPosition(CanvasGame);
                var pGame = GetPointGame(point);
                if (!RunStrokeChecker.IsKing)
                {
                    if (RunStrokeChecker.Position.X == 0)
                    {
                        if (pGame.X == 1 && RunStrokeChecker.Position.Y == pGame.Y + 1)
                        {
                            RunStrokeChecker.Position = new Point(1, pGame.Y);
                            RenderChecker(RunStrokeChecker);
                            RunStrokeChecker.ImageFigure.Opacity = 1.0;// ? 0.5 : 1.0;
                            GameProcess.Inctance.IsRunStroke = false;
                            GameProcess.Inctance.SetCursorChecker(sender as Image);
                        }
                    }
                    else if (RunStrokeChecker.Position.X == 7)
                    {
                        if (pGame.X == 6 && RunStrokeChecker.Position.Y == pGame.Y + 1)
                        {
                            RunStrokeChecker.Position = new Point(6, pGame.Y);
                            RenderChecker(RunStrokeChecker);
                            RunStrokeChecker.ImageFigure.Opacity = 1.0;// ? 0.5 : 1.0;
                            GameProcess.Inctance.IsRunStroke = false;
                            GameProcess.Inctance.SetCursorChecker(sender as Image);
                        }
                    }
                    else
                        if (RunStrokeChecker.Position.Y == pGame.Y + 1 && (RunStrokeChecker.Position.X == pGame.X - 1 || RunStrokeChecker.Position.X == pGame.X + 1))
                        {
                            RunStrokeChecker.Position = new Point(pGame.X, pGame.Y);
                            RenderChecker(RunStrokeChecker);
                            RunStrokeChecker.ImageFigure.Opacity = 1.0;// ? 0.5 : 1.0;
                            GameProcess.Inctance.IsRunStroke = false;
                            GameProcess.Inctance.SetCursorChecker(sender as Image);
                        }

                }
            }
        }
        private Point GetPointGame(Point pountCanvas)
        {
            var res = new Point(0, 0);
            res.X = (int)((pountCanvas.X - 18) / 48);
            res.Y = (int)((pountCanvas.Y - 18) / 48);
            return res;
        }

        #region Статическая часть

        static GameProcess()
        {
            Inctance = new GameProcess();
        }

        public static GameProcess Inctance { get; private set; }

        #endregion

        private bool isConnected;

        public bool IsConnected
        {
            get { return isConnected; }
            set
            {
                isConnected = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("IsConnected"));
            }
        }

        private List<User> users;

        public User[] Users
        {
            get { return users.ToArray(); }
        }

        private List<Checker> checkersSelf;
        private List<Checker> checkersOpponent;

        public Checker[] CheckerSelf
        {
            get { return checkersSelf.ToArray(); }
        }
        public Checker[] CheckerOpponent
        {
            get { return checkersOpponent.ToArray(); }
        }

        private void GenChecker()
        {

            checkersSelf = new List<Checker>();
            checkersOpponent = new List<Checker>();
            for (int i = 0; i < 8; i += 2)
            {
                var image1 = new Image() { Cursor = Cursors.Hand, Source = new BitmapImage(new Uri("pack://application:,,,/Lab2CheckersClient;component/Resources/user.png")) };
                CanvasGame.Children.Add(image1);
                checkersSelf.Add(new Checker(image1, new Point(i, 5)));
                var image2 = new Image() { Cursor = Cursors.Hand, Source = new BitmapImage(new Uri("pack://application:,,,/Lab2CheckersClient;component/Resources/user.png")) };
                CanvasGame.Children.Add(image2);
                checkersSelf.Add(new Checker(image2, new Point(i + 1, 6)));
                var image3 = new Image() { Cursor = Cursors.Hand, Source = new BitmapImage(new Uri("pack://application:,,,/Lab2CheckersClient;component/Resources/user.png")) };
                CanvasGame.Children.Add(image3);
                checkersSelf.Add(new Checker(image3, new Point(i, 7)));

                var image11 = new Image() { Source = new BitmapImage(new Uri("pack://application:,,,/Lab2CheckersClient;component/Resources/userOpponent.png")) };
                CanvasGame.Children.Add(image11);
                checkersSelf.Add(new Checker(image11, new Point(i + 1, 0)));
                var image21 = new Image() { Source = new BitmapImage(new Uri("pack://application:,,,/Lab2CheckersClient;component/Resources/userOpponent.png")) };
                CanvasGame.Children.Add(image21);
                checkersSelf.Add(new Checker(image21, new Point(i, 1)));
                var image31 = new Image() { Source = new BitmapImage(new Uri("pack://application:,,,/Lab2CheckersClient;component/Resources/userOpponent.png")) };
                CanvasGame.Children.Add(image31);
                checkersSelf.Add(new Checker(image31, new Point(i + 1, 2)));
            }
        }

        public void SetCursorChecker(Image imageTrue)
        {
            foreach (var checker in checkersSelf)
            {
                if (this.IsRunStroke)
                    checker.ImageFigure.Cursor = imageTrue == checker.ImageFigure ? Cursors.Hand : Cursors.Arrow;
                else
                    checker.ImageFigure.Cursor = Cursors.Hand;

            }
        }

        public void RemoveSelfChecker(Checker checker)
        {
            CanvasGame.Children.Remove(checker.ImageFigure);
            checkersSelf.Remove(checker);
        }
    }
}
