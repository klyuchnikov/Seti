using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace Klyuchnikov.Seti.OneSemestr.Lab2CheckersClient
{
    public class Checker
    {
        public Point Position { get; set; }

        public Image ImageFigure { get; private set; }

        public Checker(Image image, Point position, bool isSelf)
        {
            this.ImageFigure = image;
            this.Position = position;
            this.IsSelf = isSelf;
            if (this.IsSelf)
            {
                ImageFigure.MouseDown += delegate(object sender, System.Windows.Input.MouseButtonEventArgs e)
                                             {
                                                 if (!GameProcess.Inctance.IsRunStroke)
                                                 {
                                                     ImageFigure.Opacity = ImageFigure.Opacity == 1.0 ? 0.5 : 1.0;
                                                     GameProcess.Inctance.IsRunStroke = !GameProcess.Inctance.IsRunStroke;
                                                     GameProcess.Inctance.SetCursorChecker(sender as Image);
                                                     GameProcess.Inctance.RunStrokeChecker = this;
                                                 }
                                                 else
                                                 {
                                                     if (ImageFigure.Opacity == 0.5)
                                                     {
                                                         ImageFigure.Opacity = ImageFigure.Opacity == 1.0 ? 0.5 : 1.0;
                                                         GameProcess.Inctance.IsRunStroke = !GameProcess.Inctance.IsRunStroke;
                                                         GameProcess.Inctance.SetCursorChecker(sender as Image);
                                                     }
                                                 }
                                             };
            }
        }

        private bool isKing;
        public bool IsKing
        {
            get { return isKing; }
            set
            {
                isKing = value;
                if (value)
                {
                    if (!this.ImageFigure.Source.ToString().Contains("King"))
                        this.ImageFigure.Source = new BitmapImage(new Uri(this.ImageFigure.Source.ToString().Replace(".png", "King.png")));
                }

            }
        }


        public bool IsSelf { get; private set; }
    }
}
