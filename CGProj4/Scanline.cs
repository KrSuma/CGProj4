using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Drawing;

namespace CGProj4
{
    public class Scanline
    {
        private PList P;
        private List<int> indices;
        private List<Edge> AET = new List<Edge>();
        private Canvas Canvas;

        private class Edge
        {
            public Edge(Point p1, Point p2)
            {
                if (p1.Y > p2.Y)
                    ymax = (int)p1.Y;
                else
                    ymax = (int)p2.Y;
                x = p1.X;
                if (p1.X != p2.X && p2.Y != p1.Y)
                {
                    M = 1 / ((p2.Y - p1.Y) / (p2.X - p1.X));
                }
                else
                    M = 0;
            }

            public double M { get; set; }
            public double x { get; set; }
            public int ymax { get; set; }
        }

        private class PList
        {
            private List<Point> array;

            public PList(List<Point> arr)
            {
                array = arr;
            }

            public Point this[int pos]
            {
                get
                {
                    if (pos > -1 && pos < array.Count)
                        return array[pos];
                    else if (pos < 0)
                        return array[array.Count + pos];
                    else
                        return array[pos - array.Count];
                }
            }
        }

        //sorts the edges first
        public Scanline(List<Point> points, Canvas c)
        {
            Canvas = c;
            P = new PList(points);
            List<Point> temp = points.OrderBy(p => p.Y).ToList();
            this.indices = new List<int>();
            foreach (Point p in temp)
            {
                indices.Add(points.IndexOf(p));
            }
        }

        public void drawLine(Point p1, Point p2)
        {
            Line l = new Line();
            l.X1 = p1.X;
            l.Y1 = p1.Y;
            l.X2 = p2.X;
            l.Y2 = p2.Y;
            SolidColorBrush brush = new SolidColorBrush(Colors.Teal);
            l.StrokeThickness = 1;
            l.Stroke = brush;
            Canvas.Children.Add(l);
        }

        //from Lecture 7 - active edge sorting in vertices
        public void fillPolygon()
        {
            int k = 0;
            int i = indices[k];
            double y = P[i].Y;
            double ymax = P[indices[indices.Count - 1]].Y;

            while (y < ymax)
            {
                while (P[i].Y == y)
                {
                    if (P[i - 1].Y > P[i].Y)
                    {
                        AET.Add(new Edge(P[i], P[i - 1]));
                    }
                    if (P[i + 1].Y > P[i].Y)
                    {
                        AET.Add(new Edge(P[i], P[i + 1]));
                    }
                    k = k + 1;
                    i = indices[k];
                }

                //scanline happens here
                AET = AET.OrderBy(e => e.x).ToList();
                for (int index = 0; index < AET.Count; index += 2)
                {
                    drawLine(new Point(AET[index].x, y), new Point(AET[index + 1].x, y));
                }
                y = y + 1;

                AET = AET.Where(e => (e.ymax != y)).ToList();
                foreach (Edge e in AET)
                {
                    e.x += e.M;
                }
            }
        }

    }
    
}