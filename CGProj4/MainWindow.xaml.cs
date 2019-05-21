using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Drawing;
using System.Runtime.InteropServices;

namespace CGProj4
{
    public partial class MainWindow : Window
    {
        #region Declarations

        private List<Point> polygonVertices = new List<Point>();

        private Random _rand = new Random();

        #region brush

        private Brush _subjectBack = new SolidColorBrush(ColorFromHex("30427FCF"));
        private Brush _subjectBorder = new SolidColorBrush(ColorFromHex("427FCF"));
        private Brush _polysubjectBack = new SolidColorBrush(ColorFromHex("30427FCF"));
        private Brush _polysubjectBorder = new SolidColorBrush(ColorFromHex("427FCF"));
        private Brush _clipBack = new SolidColorBrush(ColorFromHex("30D65151"));
        private Brush _clipBorder = new SolidColorBrush(ColorFromHex("D65151"));
        private Brush _polyclipBack = new SolidColorBrush(ColorFromHex("30D65151"));
        private Brush _polyclipBorder = new SolidColorBrush(ColorFromHex("D65151"));
        private Brush _intersectBack = new SolidColorBrush(ColorFromHex("609F18CC"));
        private Brush _intersectBorder = new SolidColorBrush(ColorFromHex("9F18CC"));
        private Brush _polyintersectBack = new SolidColorBrush(ColorFromHex("609F18CC"));
        private Brush _polyintersectBorder = new SolidColorBrush(ColorFromHex("9F18CC"));

        #endregion brush

        private Point[] triangle;
        private Point[] rectangle;
        private Point[] concave;
        private Point[] convex;

        private bool TriangleEnabled = false;
        private bool RectEnabled = false;
        private bool ConcaveEnabled = false;
        private bool ConvexEnabled = false;

        #endregion Declarations

        public MainWindow()
        {
            InitializeComponent();
        }

        #region Functions

        private void drawLine(Point p1, Point p2, Color color, Canvas Canvas)
        {
            Line l = new Line();
            l.X1 = p1.X;
            l.Y1 = p1.Y;
            l.X2 = p2.X;
            l.Y2 = p2.Y;
            SolidColorBrush brush = new SolidColorBrush(color);
            l.StrokeThickness = 2;
            l.Stroke = brush;
            Canvas.Children.Add(l);
        }

        private void ShowPolygon(Point[] points, Brush background, Brush border, double thickness)
        {
            if (points == null || points.Length == 0)
            {
                return;
            }

            Polygon polygon = new Polygon();
            polygon.Fill = background;
            polygon.Stroke = border;
            polygon.StrokeThickness = thickness;

            foreach (Point point in points)
            {
                polygon.Points.Add(point);
            }

            ClippingCanvas.Children.Add(polygon);
        }

        private static Color ColorFromHex(string hexValue)
        {
            if (hexValue.StartsWith("#"))
            {
                return (Color)ColorConverter.ConvertFromString(hexValue);
            }
            else
            {
                return (Color)ColorConverter.ConvertFromString("#" + hexValue);
            }
        }

        [DllImport("gdi32")]
        private static extern int GetPixel(int hdc, int nXPos, int nYPos);

        [DllImport("user32")]
        private static extern int GetWindowDC(int hwnd);

        [DllImport("user32")]
        private static extern int ReleaseDC(int hWnd, int hDC);

        private static SolidColorBrush GetPixelColor(Point point)
        {
            int lDC = GetWindowDC(0);
            int intColor = GetPixel(lDC, (int)point.X, (int)point.Y);

            // Release the DC after getting the Color.
            ReleaseDC(0, lDC);

            byte a = (byte)((intColor >> 0x18) & 0xffL);
            byte b = (byte)((intColor >> 0x10) & 0xffL);
            byte g = (byte)((intColor >> 8) & 0xffL);
            byte r = (byte)(intColor & 0xffL);
            Color color = Color.FromRgb(r, g, b);
            return new SolidColorBrush(color);
        }

        #endregion Functions

        #region Clipping

        private void ClippingCanvasClick(object sender, MouseButtonEventArgs e)
        {
        }

        private void ClippingReset(object sender, MouseButtonEventArgs e)
        {
        }

        private void ClippingButton_Click(object sender, RoutedEventArgs e)
        {
            if (TriangleEnabled && RectEnabled)
            {
                Point[] intersect1 = SutherlandHodgman.GetIntersectedPolygon(triangle, rectangle);
                ShowPolygon(intersect1, _intersectBack, _intersectBorder, 3d);
            }
            else if (ConcaveEnabled && ConvexEnabled)
            {
                Point[] intersect2 = SutherlandHodgman.GetIntersectedPolygon(concave, convex);
                ShowPolygon(intersect2, _polyintersectBack, _polyintersectBorder, 3d);
            }

            ClippingButton.IsEnabled = false;
        }

        private void ClippedOnlyButton_Click(object sender, RoutedEventArgs e)
        {
            if (TriangleEnabled && RectEnabled)
            {
                Point[] intersect = SutherlandHodgman.GetIntersectedPolygon(triangle, rectangle);
                ClippingCanvas.Children.Clear();
                ShowPolygon(intersect, _intersectBack, _intersectBorder, 3d);
            }
            else if (ConcaveEnabled && ConvexEnabled)
            {
                Point[] intersect2 = SutherlandHodgman.GetIntersectedPolygon(concave, convex);
                ClippingCanvas.Children.Clear();
                ShowPolygon(intersect2, _polyintersectBack, _polyintersectBorder, 3d);
            }

            ClippedOnlyButton.IsEnabled = false;
            ClippedOnlyButton.IsEnabled = false;
        }

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            ClippingCanvas.Children.Clear();
            triangle = null;
            rectangle = null;
            convex = null;
            concave = null;
        }

        private void AddTriangle_Click(object sender, RoutedEventArgs e)
        {
            ClippingCanvas.Children.Clear();

            double width = ClippingCanvas.ActualWidth;
            double height = ClippingCanvas.ActualHeight;

            triangle = new Point[] {
                    new Point(_rand.NextDouble() * width, _rand.NextDouble() * height),
                    new Point(_rand.NextDouble() * width, _rand.NextDouble() * height),
                    new Point(_rand.NextDouble() * width, _rand.NextDouble() * height) };

            ShowPolygon(triangle, _subjectBack, _subjectBorder, 1d);
            TriangleEnabled = true;

            isClipEnabled();
        }

        private void AddRectangle_Click(object sender, RoutedEventArgs e)
        {
            double width = ClippingCanvas.ActualWidth;
            double height = ClippingCanvas.ActualHeight;

            Point rectPoint = new Point(_rand.NextDouble() * (width * .75d), _rand.NextDouble() * (height * .75d));   
            Rect rect = new Rect(rectPoint, new Size(_rand.NextDouble() * (width - rectPoint.X),
                _rand.NextDouble() * (height - rectPoint.Y)));

            rectangle = new Point[] { rect.TopLeft, rect.TopRight, rect.BottomRight, rect.BottomLeft };

            ShowPolygon(rectangle, _clipBack, _clipBorder, 1d);
            RectEnabled = true;

            isClipEnabled();
        }

        private void isClipEnabled()
        {
            if ((TriangleEnabled && RectEnabled) || (ConcaveEnabled && ConvexEnabled))
            {
                ClippingButton.IsEnabled = true;
                ClippedOnlyButton.IsEnabled = true;
            }
        }

        private void AddConcave_Click(object sender, RoutedEventArgs e)
        {
            concave = new Point[]
            { new Point(randomGen(), randomGen()), new Point(randomGen(), randomGen()), new Point(randomGen(), randomGen()), new Point(randomGen(), randomGen()), new Point(randomGen(), randomGen()), new Point(randomGen(), randomGen()) };

            ShowPolygon(concave, _polysubjectBack, _polysubjectBorder, 1d);
            ConcaveEnabled = true;

            isClipEnabled();
        }

        private void AddConvex_Click(object sender, RoutedEventArgs e)
        {
            convex = new Point[]
            { new Point(100, 100), new Point(300, 100), new Point(300, 300), new Point(100, 300) };

            ShowPolygon(convex, _polyclipBack, _polyclipBorder, 1d);
            ConvexEnabled = true;

            isClipEnabled();
        }

        private int randomGen()
        {
            return _rand.Next(50, 350);
        }

        #endregion Clipping

        #region Filling

        //filling
        private void FillButton_Click(object sender, RoutedEventArgs e)
        {
            Scanline filler = new Scanline(polygonVertices, FillingCanvas);
            filler.fillPolygon();
            FillButton.IsEnabled = false;
        }

        private void FillingCanvas_Click(object sender, MouseButtonEventArgs e)
        {
            if (polygonVertices.Count < FillVertexSlider.Value)
            {
                Point temp = e.GetPosition(FillingCanvas);
                polygonVertices.Add(new Point((int)temp.X, (int)temp.Y));
                if (polygonVertices.Count > 1)
                {
                    drawLine(polygonVertices[polygonVertices.Count - 1], polygonVertices[polygonVertices.Count - 2], Colors.Purple, FillingCanvas);
                    if (polygonVertices.Count == FillVertexSlider.Value)
                    {
                        drawLine(polygonVertices[polygonVertices.Count - 1], polygonVertices[0], Colors.Purple, FillingCanvas);
                        showFillVerticesInListView();
                        FillButton.IsEnabled = true;
                    }
                }
            }
        }

        private void showFillVerticesInListView()
        {
            foreach (Point p in polygonVertices)
            {
                ListViewItem it = new ListViewItem { Content = "X: " + (int)p.X + " Y: " + (int)p.Y };
                FillVerticesListView.Items.Add(it);
            }
        }

        private void FillVerticesChanged(object sender, TextChangedEventArgs e)
        {
            polygonVertices.Clear();
            FillingCanvas.Children.Clear();
            FillVerticesListView.Items.Clear();
            FillButton.IsEnabled = false;
        }

        private void FillResetButton_Click(object sender, RoutedEventArgs e)
        {
            polygonVertices.Clear();
            FillingCanvas.Children.Clear();
            FillVerticesListView.Items.Clear();
            FillButton.IsEnabled = false;
        }

        #endregion Filling

        #region Boundary
        private void FillingCanvas_RightClick(object sender, MouseButtonEventArgs e)
        {
            //Point temp = e.GetPosition(FillingCanvas);
            //SolidColorBrush solidcolor = GetPixelColor(FillingCanvas.PointToScreen(temp));
            //Color color = Color.FromArgb(solidcolor.Color.A,
            //             solidcolor.Color.R,
            //             solidcolor.Color.G,
            //             solidcolor.Color.B);

            //BoundaryFill boundary = new BoundaryFill();
            //boundary.Fill(temp, color, FillingCanvas);
        }
        #endregion
    }
}