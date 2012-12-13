using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Lab2CheckersClient
{
    public static class PointExtention
    {
        public static Point GetPointInvers(this Point point)
        {
            return new Point(Math.Abs(point.X - 7), Math.Abs(point.Y - 7));
        }
    }
}
