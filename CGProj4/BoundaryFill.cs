using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Drawing;
using System.Runtime.InteropServices;

namespace CGProj4
{
    class BoundaryFill
    {
        private Line drawLine(Point currentPoint, Color current)
        {
            Line l = new Line();
            l.X1 = currentPoint.X;
            l.Y1 = currentPoint.Y;
            l.X2 = currentPoint.X;
            l.Y2 = currentPoint.Y;
            SolidColorBrush brush = new SolidColorBrush(current);
            l.StrokeThickness = 1;
            l.Stroke = brush;

            return l;
        }

        //boundary fill
        public void Fill(Point initial, Color pointColor, Canvas canvas)
        {
            Stack<Point> points = new Stack<Point>();
            points.Push(new Point(initial.X, initial.Y));

            while (!(points.Count == 0))
            {
                Point currentPoint = points.Pop();
                double x = currentPoint.X;
                double y = currentPoint.Y;

                //need to get the color value of the current point
                Color current = pointColor;

                if (current == pointColor)
                {
                    current = Colors.Red;
                    Line p = drawLine(currentPoint, current);
                    canvas.Children.Add(p);

                    points.Push(new Point(x + 1, y));
                    points.Push(new Point(x - 1, y));
                    points.Push(new Point(x, y + 1));
                    points.Push(new Point(x, y - 1));
                }
            }
        }

    }
}
