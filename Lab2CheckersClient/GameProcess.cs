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

        public MainWindow MainWindow { get; set; }

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

        private User opponent;

        public User Opponent
        {
            get { return opponent; }
            set
            {
                opponent = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("Opponent"));
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

        private bool isSelfStroke;

        /// <summary>
        /// Чей ход в данный момент, если true - ход мой, false - ход противника
        /// </summary>
        public bool IsSelfStroke
        {
            get { return isSelfStroke; }
            set
            {
                isSelfStroke = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("IsSelfStroke"));
            }
        }

        private void canvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!IsRunStroke) return;
            var pGame = GetPointGame(Mouse.GetPosition(CanvasGame));
            bool endStroke = (pGame == RunStrokeChecker.Position);
            if (endStroke)
            {
                if (beatenPointOpponentChekers.Count == 0)
                    return;
                foreach (
                    var checkerOpponent in
                        beatenPointOpponentChekers.Select(c => checkersOpponent.Single(q => q.Position == c)))
                    BeatOpponentChecker(checkerOpponent);
                beatenPointOpponentChekers = new List<Point>();
                Client.Current.SendStroke(SendStroke);
                SendStroke = "";
                return;
            }
            if (!IsFreePoint(pGame))
                return;
            if ((pGame.Y % 2 != 0 || pGame.X % 2 != 1) && (pGame.Y % 2 != 1 || pGame.X % 2 != 0))
                return; // кликнули по белым точкам

            if (SendStroke == "")
                SendStroke = RunStrokeChecker.Position + "|";
            if (!RunStrokeChecker.IsKing)
            {
                if (RunStrokeChecker.Position.Y == pGame.Y + 1) // если просто ход вперед
                {
                    if (beatenPointOpponentChekers.Count == 0)
                        if (RunStrokeChecker.Position.X == pGame.X - 1 || RunStrokeChecker.Position.X == pGame.X + 1)
                            MoveChecker(pGame);
                }
                else // если удар
                {
                    var points = new[] { new Point(1, -1), new Point(1, 1), new Point(-1, -1), new Point(-1, 1) };
                    foreach (
                        var point in
                            points.Where(point => IsCheckerOpponent(RunStrokeChecker.Position + (Vector)point) &&
                                                  IsFreePoint(RunStrokeChecker.Position + (Vector)point * 2) &&
                                                  RunStrokeChecker.Position + (Vector)point * 2 == pGame))
                    {
                        if (beatenPointOpponentChekers.Contains(RunStrokeChecker.Position + (Vector)point))
                            return;
                        beatenPointOpponentChekers.Add(RunStrokeChecker.Position + (Vector)point);
                        BeatenChecker(
                            checkersOpponent.Single(a => a.Position == RunStrokeChecker.Position + (Vector)point));
                        SendStroke += ":" + (RunStrokeChecker.Position + (Vector)point);
                        SendStroke += "|" + pGame + "|";
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
                            lastPoints.Add(new Point(RunStrokeChecker.Position.X + k * dxP / Math.Abs(dxP),
                                                     RunStrokeChecker.Position.Y + k * dyP / Math.Abs(dyP)));
                        if (checkersSelf.SingleOrDefault(q => lastPoints.Contains(q.Position)) != null)
                            return;
                        var beatenCheckers = checkersOpponent.Where(a => lastPoints.Contains(a.Position)).ToArray();
                        int countBeatenCurrent = 0;
                        foreach (
                            var beatenChecker in
                                beatenCheckers.Where(
                                    beatenChecker => !beatenPointOpponentChekers.Contains(beatenChecker.Position)))
                        {
                            countBeatenCurrent++;
                            beatenPointOpponentChekers.Add(beatenChecker.Position);
                            BeatenChecker(beatenChecker);
                            SendStroke += ":" + beatenChecker.Position;
                        }
                        if (beatenPointOpponentChekers.Count == 0)
                        {
                            RunStrokeChecker.ImageFigure.Opacity = RunStrokeChecker.ImageFigure.Opacity == 1.0
                                                                       ? 0.5
                                                                       : 1.0;
                            GameProcess.Inctance.IsRunStroke = !GameProcess.Inctance.IsRunStroke;
                            GameProcess.Inctance.SetCursorChecker(sender as Image);
                            SendStroke += "|" + pGame + "|";
                            Client.Current.SendStroke(SendStroke);
                            SendStroke = "";
                        }
                        else if (countBeatenCurrent == 0 && beatenPointOpponentChekers.Count != 0)
                        {
                            return;
                        }
                        else
                            SendStroke += "|" + pGame + "|";
                        RunStrokeChecker.Position = new Point(pGame.X, pGame.Y);
                        RenderChecker(RunStrokeChecker);
                    }
            }
        }

        /// <summary>
        /// Точки своего хога
        /// </summary>
        private string SendStroke = "";

        private Stack<Point> SelfStrokes = new Stack<Point>();

        /// <summary>
        /// убиваемые шашки противника за текущий ход
        /// </summary>
        private List<Point> beatenPointOpponentChekers = new List<Point>();

        private bool isGameOnline;

        #region IsGameOnline

        /// <summary>
        /// Играет ли сейчас пользователь
        /// </summary>
        public bool IsGameOnline
        {
            get { return isGameOnline; }
            set
            {
                isGameOnline = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("IsGameOnline"));
            }
        }

        #endregion


        #region Users

        private User[] users = new User[0];

        public User[] Users
        {
            get { return users; }
            set
            {
                users = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("Users"));
                    PropertyChanged(this, new PropertyChangedEventArgs("UserFree"));
                }
            }
        }

        #endregion

        public int UserFree
        {
            get { return users.Count(a => a.OpponentGuid == Guid.Empty); }
        }

        #region CheckerSelf

        private List<Checker> checkersSelf;

        /// <summary>
        /// Свои шашки 
        /// </summary>
        public Checker[] CheckersSelf
        {
            get { return checkersSelf.ToArray(); }
        }

        #endregion

        #region CheckerOpponent

        /// <summary>
        /// Шашки противника
        /// </summary>
        private List<Checker> checkersOpponent;

        public Checker[] CheckerOpponent
        {
            get { return checkersOpponent.ToArray(); }
        }

        #endregion

        #region CheckersBeaten

        /// <summary>
        /// убитые шашки противника
        /// </summary>
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

        private void BeatenChecker(Checker checker)
        {

            DoubleAnimation dbAscendingX =
                new DoubleAnimation(checker.ImageFigure.Opacity, 0.3,
                                    new Duration(TimeSpan.FromMilliseconds(500)));
            Storyboard storyboard = new Storyboard();
            storyboard.Children.Add(dbAscendingX);
            Storyboard.SetTarget(dbAscendingX, checker.ImageFigure);
            Storyboard.SetTargetProperty(dbAscendingX, new PropertyPath(Image.OpacityProperty));
            storyboard.Begin();
            storyboard.Completed += delegate
                {
                    checker.ImageFigure.Opacity = 0.3;
                };
        }

        public void SetCanvasGame(Canvas canvas)
        {
            this.CanvasGame = canvas;
            canvas.PreviewMouseDown += canvas_MouseDown;
        }

        private void MoveChecker(Point pGame)
        {
            if (!IsFreePoint(pGame)) return;
            if (pGame.Y == 0.0)
                RunStrokeChecker.IsKing = true;
            RunStrokeChecker.ImageFigure.Opacity = 1.0; // ? 0.5 : 1.0;
            GameProcess.Inctance.IsRunStroke = false;
            GameProcess.Inctance.SetCursorChecker(RunStrokeChecker.ImageFigure);

            var stroke = RunStrokeChecker.Position + "|" + pGame;
            Client.Current.SendStroke(stroke);
            RunStrokeChecker.Position = new Point(pGame.X, pGame.Y);
            RenderChecker(RunStrokeChecker);
            SendStroke = "";
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
            var res = new Point((int)((pountCanvas.X - 18) / 48), (int)((pountCanvas.Y - 18) / 48));
            return res;
        }

        public void StartGame(bool IsOwner)
        {
            string pngSelf, pngOpponent;
            if (IsOwner)
            {
                pngSelf = "pack://application:,,,/Lab2CheckersClient;component/Resources/userWhite.png";
                pngOpponent = "pack://application:,,,/Lab2CheckersClient;component/Resources/userBlack.png";
            }
            else
            {
                pngSelf = "pack://application:,,,/Lab2CheckersClient;component/Resources/userBlack.png";
                pngOpponent = "pack://application:,,,/Lab2CheckersClient;component/Resources/userWhite.png";
            }
            checkersSelf = new List<Checker>();
            checkersOpponent = new List<Checker>();
            checkersBeaten = new List<Checker>();
            for (int i = 0; i < 8; i += 2)
            {
                var image1 = new Image() { Cursor = Cursors.Hand, Source = new BitmapImage(new Uri(pngSelf)) };
                CanvasGame.Children.Add(image1);
                checkersSelf.Add(new Checker(image1, new Point(i, 5), true));
                var image2 = new Image() { Cursor = Cursors.Hand, Source = new BitmapImage(new Uri(pngSelf)) };
                CanvasGame.Children.Add(image2);
                checkersSelf.Add(new Checker(image2, new Point(i + 1, 6), true));
                var image3 = new Image() { Cursor = Cursors.Hand, Source = new BitmapImage(new Uri(pngSelf)) };
                CanvasGame.Children.Add(image3);
                checkersSelf.Add(new Checker(image3, new Point(i, 7), true));

                var image11 = new Image() { Source = new BitmapImage(new Uri(pngOpponent)) };
                CanvasGame.Children.Add(image11);
                checkersOpponent.Add(new Checker(image11, new Point(i + 1, 0), false));

                var image21 = new Image() { Source = new BitmapImage(new Uri(pngOpponent)) };
                CanvasGame.Children.Add(image21);
                checkersOpponent.Add(new Checker(image21, new Point(i, 1), false));

                var image31 = new Image() { Source = new BitmapImage(new Uri(pngOpponent)) };
                CanvasGame.Children.Add(image31);
                checkersOpponent.Add(new Checker(image31, new Point(i + 1, 2), false));
            }
            RenderCheckers();
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

        internal void RenderOpponentStroke(string stroke)
        {
            MainWindow.Dispatcher.Invoke(new Action(() =>
                {
                    Checker checker = null;
                    var list = new List<Tuple<bool, Point>>();
                    foreach (var block in stroke.Split(new[] { "|" }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        var beatenPoints = block.Split(new[] { ":" }, StringSplitOptions.RemoveEmptyEntries);
                        if (!block.Contains(":")) // точки хода
                        {
                            var point = Point.Parse(block.Replace(';', ',')).GetPointInvers();
                            if (checker == null)
                                checker = checkersOpponent.Single(a => a.Position == point);
                            else
                                list.Add(new Tuple<bool, Point>(false, point));
                        }
                        else // убитые свои шашки в полученных точках
                            list.AddRange(beatenPoints.Select(
                                beatenPoint => Point.Parse(beatenPoint.Replace(';', ',')).GetPointInvers())
                                                      .Select(point => new Tuple<bool, Point>(true, point)));
                    }
                    var duration = 0;
                    Point oldPoint = new Point(-1, -1);
                    Point newPoint = new Point(-1, -1);
                    foreach (var point in list)
                    {
                        if (!point.Item1)
                        {
                            oldPoint = oldPoint == new Point(-1, -1) ? checker.Position : newPoint;
                            newPoint = point.Item2;
                            DoubleAnimation dbAscendingX =
                                new DoubleAnimation(18 + oldPoint.X * 48, 18 + newPoint.X * 48,
                                                    new Duration(TimeSpan.FromMilliseconds(500)));
                            DoubleAnimation dbAscendingY =
                                new DoubleAnimation(18 + oldPoint.Y * 48, 18 + newPoint.Y * 48,
                                                    new Duration(TimeSpan.FromMilliseconds(500)));
                            Storyboard storyboard = new Storyboard();
                            storyboard.Children.Add(dbAscendingX);
                            storyboard.Children.Add(dbAscendingY);
                            Storyboard.SetTarget(dbAscendingX, checker.ImageFigure);
                            Storyboard.SetTarget(dbAscendingY, checker.ImageFigure);
                            Storyboard.SetTargetProperty(dbAscendingX, new PropertyPath(Canvas.LeftProperty));
                            Storyboard.SetTargetProperty(dbAscendingY, new PropertyPath(Canvas.TopProperty));
                            storyboard.Completed += delegate
                                {
                                    if (queue.Count != 0)
                                        queue.Dequeue().Begin();
                                };
                            queue.Enqueue(storyboard);
                            duration += 1000;
                            checker.Position = newPoint;
                        }
                        else
                        {
                            var checkerBeaten = checkersSelf.Single(a => a.Position == point.Item2);
                            DoubleAnimation dbAscendingX =
                                new DoubleAnimation(checkerBeaten.ImageFigure.Opacity, 0.3,
                                                    new Duration(TimeSpan.FromMilliseconds(500)));
                            Storyboard storyboardBeaten = new Storyboard();
                            storyboardBeaten.Children.Add(dbAscendingX);
                            Storyboard.SetTarget(dbAscendingX, checkerBeaten.ImageFigure);
                            Storyboard.SetTargetProperty(dbAscendingX, new PropertyPath(Image.OpacityProperty));
                            storyboardBeaten.Completed += delegate
                                {
                                    BeatSelfChecker(checkerBeaten);
                                };
                            storyboardBeaten.BeginTime = TimeSpan.FromMilliseconds(duration);
                            storyboardBeaten.Begin();
                        }
                    }
                    queue.Dequeue().Begin();
                }
                                             ), null);
            GameProcess.Inctance.IsSelfStroke = true;
        }

        private Queue<Storyboard> queue = new Queue<Storyboard>();
    }
}
