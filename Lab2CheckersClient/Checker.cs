using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace Lab2CheckersClient
{
    public class Checker
    {
        public Point Position { get; set; }

        public Image ImageFigure { get; private set; }

        public Checker(Image image, Point position)
        {
            this.ImageFigure = image;
            this.Position = position;
            if (this.IsSelf)
            {
                ImageFigure.MouseDown += delegate(object sender, System.Windows.Input.MouseButtonEventArgs e)
                                             {
                                                 if (!GameProcess.Inctance.IsStroke)
                                                 {
                                                     ImageFigure.Opacity = ImageFigure.Opacity == 1.0 ? 0.5 : 1.0;
                                                     GameProcess.Inctance.IsStroke = !GameProcess.Inctance.IsStroke;
                                                     GameProcess.Inctance.SetCursorChecker(sender as Image);
                                                 }
                                                 else
                                                 {
                                                     if (ImageFigure.Opacity == 0.5)
                                                     {
                                                         ImageFigure.Opacity = ImageFigure.Opacity == 1.0 ? 0.5 : 1.0;
                                                         GameProcess.Inctance.IsStroke = !GameProcess.Inctance.IsStroke;
                                                         GameProcess.Inctance.SetCursorChecker(sender as Image);
                                                     }
                                                 }
                                             };
            }
        }

        public bool IsSelf
        {
            get
            {
                return !(this.ImageFigure.Source as BitmapImage).UriSource.AbsoluteUri.Contains("userOpponent");
            }
        }
    }
}
