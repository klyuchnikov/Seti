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
using System.Windows.Media.Animation;

namespace Lab2CheckersClient
{
    public sealed class GameProcess : INotifyPropertyChanged
    {
        #region Статическая часть

        static GameProcess()
        {
            Inctance = new GameProcess();
        }

        public static GameProcess Inctance { get; private set; }

        #endregion

        private GameProcess()
        {
            checkersSelf = new List<Checker>();
            checkersOpponent = new List<Checker>();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public Canvas CanvasGame { get; private set; }

        private string userName;
        public string UserName
        {
            get { return userName; }
            set
            {
                userName = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("UserName"));
            }
        }

        /// <summary>
        /// В данный момент выполняется ход
        /// </summary>
        public bool IsRunStroke { get; set; }

        /// <summary>
        /// Выделенная шашка, которой производится ход
        /// </summary>
        public Checker RunStrokeChecker { get; set; }

        /// <summary>
        /// Чей ход в данный момент, если true - ход мой, false - ход противника
        /// </summary>
        public bool IsSelfStroke { get; set; }

        void canvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!IsRunStroke) return;
            var pGame = GetPointGame(Mouse.GetPosition(CanvasGame));
            bool endStroke = (pGame == RunStrokeChecker.Position);
            if (endStroke)
            {
                foreach (var beatenPointOpponentCheker in beatenPointOpponentChekers)
                {
                    var checkerOpponent = checkersOpponent.Single(q => q.Position == beatenPointOpponentCheker);
                    BeatOpponentChecker(checkerOpponent);
                }
                beatenPointOpponentChekers = new List<Point>();
                return;
            }
            if (!IsFreePoint(pGame))
                return;
            if ((pGame.Y % 2 != 0 || pGame.X % 2 != 1) && (pGame.Y % 2 != 1 || pGame.X % 2 != 0)) return; // кликнули по белым точкам
            if (!RunStrokeChecker.IsKing)
            {
                if (RunStrokeChecker.Position.Y == pGame.Y + 1) // если просто ход вперед
                {
                    if (RunStrokeChecker.Position.X == pGame.X - 1 || RunStrokeChecker.Position.X == pGame.X + 1)
                        MoveChecker(pGame);
                }
                else // если удар
                {
                    var points = new[] { new Point(1, -1), new Point(1, 1), new Point(-1, -1), new Point(-1, 1) };
                    foreach (var point in points)
                        if (IsCheckerOpponent(RunStrokeChecker.Position + (Vector)point) &&
                            IsFreePoint(RunStrokeChecker.Position + (Vector)point * 2) &&
                            RunStrokeChecker.Position + (Vector)point * 2 == pGame)
                        {
                            beatenPointOpponentChekers.Add(RunStrokeChecker.Position + (Vector)point);
                            RunStrokeChecker.Position = new Point(pGame.X, pGame.Y);
                            RenderChecker(RunStrokeChecker);
                        }
                }
            }
            else
            {
                var points = new List<Point>();
                int i = 1;
                do
                {
                    var oldCount = points.Count;
                    var t1 = RunStrokeChecker.Position + (Vector)new Point(-i, -i);
                    var t2 = RunStrokeChecker.Position + (Vector)new Point(i, -i);
                    var t3 = RunStrokeChecker.Position + (Vector)new Point(-i, i);
                    var t4 = RunStrokeChecker.Position + (Vector)new Point(i, i);
                    if (t1.X > -1.0 && t1.Y > -1.0)
                        points.Add(t1);
                    if (t2.X < 8.0 && t2.Y > -1.0)
                        points.Add(t2);
                    if (t3.X > -1.0 && t3.Y < 8.0)
                        points.Add(t3);
                    if (t4.X < 8.0 && t4.Y < 8.0)
                        points.Add(t4);
                    if (oldCount == points.Count)
                        break;
                    else
                        i++;
                } while (true);


                foreach (var point in points)
                    if (point == pGame)
                    {
                        var dxP = point.X - RunStrokeChecker.Position.X;
                        var dyP = point.Y - RunStrokeChecker.Position.Y;
                        var lastPoints = new List<Point>();
                        for (int k = 1; k <= Math.Abs(dxP) - 1; k++)
                            lastPoints.Add(new Point(RunStrokeChecker.Position.X + k * dxP / Math.Abs(dxP), RunStrokeChecker.Position.Y + k * dyP / Math.Abs(dyP)));
                        if (checkersSelf.SingleOrDefault(q => lastPoints.Contains(q.Position)) != null)
                            return;
                        var beatenCheckers = checkersOpponent.Where(a => lastPoints.Contains(a.Position));
                        foreach (var beatenChecker in beatenCheckers)
                            beatenPointOpponentChekers.Add(beatenChecker.Position);
                        if (beatenPointOpponentChekers.Count == 0)
                        {
                            RunStrokeChecker.ImageFigure.Opacity = RunStrokeChecker.ImageFigure.Opacity == 1.0 ? 0.5 : 1.0;
                            GameProcess.Inctance.IsRunStroke = !GameProcess.Inctance.IsRunStroke;
                            GameProcess.Inctance.SetCursorChecker(sender as Image);
                        }
                        RunStrokeChecker.Position = new Point(pGame.X, pGame.Y);
                        RenderChecker(RunStrokeChecker);
                    }
            }
        }

        private List<Point> beatenPointOpponentChekers = new List<Point>();

        private bool isGameOnline;
        #region IsGameOnline
        public bool IsGameOnline
        {
            get { return isGameOnline; }
            private set
            {
                isGameOnline = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("IsGameOnline"));
            }
        }
        #endregion


        #region Users

        private User[] users;
        public User[] Users
        {
            get { return users; }
            set
            {
                users = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("Users"));
            }
        }
        #endregion

        #region CheckerSelf
        private List<Checker> checkersSelf;
        public Checker[] CheckerSelf
        {
            get { return checkersSelf.ToArray(); }
        }
        #endregion

        #region CheckerOpponent
        private List<Checker> checkersOpponent;
        public Checker[] CheckerOpponent
        {
            get { return checkersOpponent.ToArray(); }
        }
        #endregion

        #region CheckersBeaten
        private List<Checker> checkersBeaten;
        public Checker[] CheckersBeaten
        {
            get { return checkersBeaten.ToArray(); }
        }
        #endregion


        // Игровой процесс

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

            DoubleAnimation dbAscendingX =
                new DoubleAnimation(Canvas.GetLeft(checker.ImageFigure), 18 + checker.Position.X * 48,
                                    new Duration(TimeSpan.FromMilliseconds(500)));
            DoubleAnimation dbAscendingY =
                new DoubleAnimation(Canvas.GetTop(checker.ImageFigure), 18 + checker.Position.Y * 48,
                                    new Duration(TimeSpan.FromMilliseconds(500)));
            Storyboard storyboard = new Storyboard();
            storyboard.Children.Add(dbAscendingX);
            storyboard.Children.Add(dbAscendingY);
            Storyboard.SetTarget(dbAscendingX, checker.ImageFigure);
            Storyboard.SetTarget(dbAscendingY, checker.ImageFigure);
            Storyboard.SetTargetProperty(dbAscendingX, new PropertyPath(Canvas.LeftProperty));
            Storyboard.SetTargetProperty(dbAscendingY, new PropertyPath(Canvas.TopProperty));
            storyboard.Begin();
            storyboard.Completed += delegate
            {
                Canvas.SetLeft(checker.ImageFigure, 18 + checker.Position.X * 48);
                Canvas.SetTop(checker.ImageFigure, 18 + checker.Position.Y * 48);
            };
        }

        public void SetCanvasGame(Canvas canvas)
        {
            this.CanvasGame = canvas;
            GenChecker();
            RenderCheckers();
            canvas.PreviewMouseDown += canvas_MouseDown;
        }

        private void MoveChecker(Point pGame)
        {
            if (!IsFreePoint(pGame)) return;
            if (pGame.Y == 0.0)
                RunStrokeChecker.IsKing = true;
            RunStrokeChecker.Position = new Point(pGame.X, pGame.Y);
            RenderChecker(RunStrokeChecker);
            RunStrokeChecker.ImageFigure.Opacity = 1.0; // ? 0.5 : 1.0;
            GameProcess.Inctance.IsRunStroke = false;
            GameProcess.Inctance.SetCursorChecker(RunStrokeChecker.ImageFigure);
        }

        private bool IsFreePoint(Point p)
        {
            return (checkersSelf.SingleOrDefault(a => a.Position == p) == null &&
                    checkersOpponent.SingleOrDefault(a => a.Position == p) == null);
        }

        private bool IsCheckerOpponent(Point p)
        {
            return (checkersOpponent.SingleOrDefault(a => a.Position == p) != null);
        }

        private Point GetPointGame(Point pountCanvas)
        {
            var res = new Point(0, 0);
            res.X = (int)((pountCanvas.X - 18) / 48);
            res.Y = (int)((pountCanvas.Y - 18) / 48);
            return res;
        }

        private void GenChecker()
        {

            checkersSelf = new List<Checker>();
            checkersOpponent = new List<Checker>();
            checkersBeaten = new List<Checker>();
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

                /*     var image11 = new Image() { Source = new BitmapImage(new Uri("pack://application:,,,/Lab2CheckersClient;component/Resources/userOpponent.png")) };
                     CanvasGame.Children.Add(image11);
                     checkersOpponent.Add(new Checker(image11, new Point(i + 1, 0)));
                     var image21 = new Image() { Source = new BitmapImage(new Uri("pack://application:,,,/Lab2CheckersClient;component/Resources/userOpponent.png")) };
                     CanvasGame.Children.Add(image21);
                     checkersOpponent.Add(new Checker(image21, new Point(i, 1)));*/
                var image31 = new Image() { Source = new BitmapImage(new Uri("pack://application:,,,/Lab2CheckersClient;component/Resources/userOpponent.png")) };
                CanvasGame.Children.Add(image31);
                checkersOpponent.Add(new Checker(image31, new Point(i + 1, 2)));
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

        public void BeatSelfChecker(Checker checker)
        {
            CanvasGame.Children.Remove(checker.ImageFigure);
            checkersSelf.Remove(checker);
        }

        public void BeatOpponentChecker(Checker checker)
        {
            CanvasGame.Children.Remove(checker.ImageFigure);
            checkersBeaten.Add(checker);
            checkersOpponent.Remove(checker);
        }
    }
}
