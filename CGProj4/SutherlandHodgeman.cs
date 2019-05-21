using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace CGProj4
{
    public static class SutherlandHodgman
    {
        private class Edge
        {
            public Edge(Point p1, Point p2)
            {
                this.p1 = p1;
                this.p2 = p2;
            }

            public Point p1;
            public Point p2;
        }

        //http://en.wikipedia.org/wiki/Sutherland%E2%80%93Hodgman
        public static Point[] GetIntersectedPolygon(Point[] subjectPoly, Point[] clipPoly)
        {
            if (subjectPoly.Length < 3 || clipPoly.Length < 3)
            {
                throw new ArgumentException("Error");
            }

            List<Point> outputList = subjectPoly.ToList();

            if (!IsClockwise(subjectPoly))
            {
                outputList.Reverse();
            }

            foreach (Edge clipEdge in IterateEdgesClockwise(clipPoly))
            {
                List<Point> inputList = outputList.ToList();
                outputList.Clear();

                if (inputList.Count == 0)
                {
                    break;
                }

                Point S = inputList[inputList.Count - 1];

                foreach (Point E in inputList)
                {
                    if (IsInside(clipEdge, E))
                    {
                        if (!IsInside(clipEdge, S))
                        {
                            Point? point = GetIntersect(S, E, clipEdge.p1, clipEdge.p2);
                            if (point == null)
                            {
                                throw new ApplicationException("no intersection");
                            }
                            else
                            {
                                outputList.Add(point.Value);
                            }
                        }

                        outputList.Add(E);
                    }
                    else if (IsInside(clipEdge, S))
                    {
                        Point? point = GetIntersect(S, E, clipEdge.p1, clipEdge.p2);
                        if (point == null)
                        {
                            throw new ApplicationException("no intersection");
                        }
                        else
                        {
                            outputList.Add(point.Value);
                        }
                    }

                    S = E;
                }
            }

            return outputList.ToArray();
        }

        //iterates through the edges clockwise.
        private static IEnumerable<Edge> IterateEdgesClockwise(Point[] polygon)
        {
            if (IsClockwise(polygon))
            {
                #region Already clockwise

                for (int cntr = 0; cntr < polygon.Length - 1; cntr++)
                {
                    yield return new Edge(polygon[cntr], polygon[cntr + 1]);
                }

                yield return new Edge(polygon[polygon.Length - 1], polygon[0]);

                #endregion Already clockwise
            }
            else
            {
                #region Reverse

                for (int cntr = polygon.Length - 1; cntr > 0; cntr--)
                {
                    yield return new Edge(polygon[cntr], polygon[cntr - 1]);
                }

                yield return new Edge(polygon[0], polygon[polygon.Length - 1]);

                #endregion Reverse
            }
        }

        //detects intersection
        private static Point? GetIntersect(Point line1From, Point line1To, Point line2From, Point line2To)
        {
            Vector direction1 = line1To - line1From;
            Vector direction2 = line2To - line2From;
            double dotPerp = (direction1.X * direction2.Y) - (direction1.Y * direction2.X);

            if (IsNearZero(dotPerp))
            {
                return null;
            }

            Vector c = line2From - line1From;
            double t = (c.X * direction2.Y - c.Y * direction2.X) / dotPerp;

            return line1From + (t * direction1);
        }

        //checks if its inside
        private static bool IsInside(Edge edge, Point test)
        {
            bool? isLeft = IsLeftOf(edge, test);
            if (isLeft == null)
            {
                return true;
            }

            return !isLeft.Value;
        }

        //checks if its clockwise
        private static bool IsClockwise(Point[] polygon)
        {
            for (int cntr = 2; cntr < polygon.Length; cntr++)
            {
                bool? isLeft = IsLeftOf(new Edge(polygon[0], polygon[1]), polygon[cntr]);
                if (isLeft != null)
                {
                    return !isLeft.Value;
                }
            }

            throw new ArgumentException("colinear");
        }

        //checks if it is on the left side
        private static bool? IsLeftOf(Edge edge, Point test)
        {
            Vector tmp1 = edge.p2 - edge.p1;
            Vector tmp2 = test - edge.p2;

            double x = (tmp1.X * tmp2.Y) - (tmp1.Y * tmp2.X);

            if (x < 0)
            {
                return false;
            }
            else if (x > 0)
            {
                return true;
            }
            else
            {
                return null;
            }
        }

        private static bool IsNearZero(double testValue)
        {
            return Math.Abs(testValue) <= .000000001d;
        }
    }
}