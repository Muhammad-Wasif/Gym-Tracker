using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace FitTrack.UI.Controls
{
    public partial class LineChart : UserControl
    {
        public static readonly DependencyProperty DataPointsProperty =
            DependencyProperty.Register("DataPoints", typeof(IEnumerable<(DateTime date, double value)>), typeof(LineChart), new PropertyMetadata(null, OnDataPointsChanged));

        public static readonly DependencyProperty LineColorProperty =
            DependencyProperty.Register("LineColor", typeof(Brush), typeof(LineChart), new PropertyMetadata(Brushes.DodgerBlue, OnDataPointsChanged));

        public IEnumerable<(DateTime date, double value)> DataPoints
        {
            get { return (IEnumerable<(DateTime date, double value)>)GetValue(DataPointsProperty); }
            set { SetValue(DataPointsProperty, value); }
        }

        public Brush LineColor
        {
            get { return (Brush)GetValue(LineColorProperty); }
            set { SetValue(LineColorProperty, value); }
        }

        public LineChart()
        {
            InitializeComponent();
        }

        private static void OnDataPointsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is LineChart chart && chart.IsLoaded)
            {
                chart.DrawChart();
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            DrawChart();
        }

        private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            DrawChart();
        }

        private void DrawChart()
        {
            ChartCanvas.Children.Clear();
            if (DataPoints == null || !DataPoints.Any()) return;

            var pointsList = DataPoints.ToList();
            double minX = pointsList.Min(p => p.date.Ticks);
            double maxX = pointsList.Max(p => p.date.Ticks);
            double minY = pointsList.Min(p => p.value);
            double maxY = pointsList.Max(p => p.value);

            if (minY == maxY) { minY -= 10; maxY += 10; }
            if (minX == maxX) { maxX += TimeSpan.FromDays(1).Ticks; }

            double width = ChartCanvas.ActualWidth - 40; // padding for labels
            double height = ChartCanvas.ActualHeight - 20;

            if (width <= 0 || height <= 0) return;

            // Draw Grid Lines (20% intervals)
            Brush borderBrush = Application.Current.Resources["BorderBrush"] as Brush ?? Brushes.Gray;
            Brush textBrush = Application.Current.Resources["TextSecBrush"] as Brush ?? Brushes.Gray;
            Style captionStyle = Application.Current.Resources["Caption"] as Style;

            for (int i = 0; i <= 5; i++)
            {
                double yPos = height - (i * height / 5);
                double val = minY + (i * (maxY - minY) / 5);

                var line = new Line
                {
                    X1 = 40, Y1 = yPos, X2 = width + 40, Y2 = yPos,
                    Stroke = borderBrush, StrokeThickness = 1, StrokeDashArray = new DoubleCollection(new[] { 4.0, 4.0 })
                };
                ChartCanvas.Children.Add(line);

                var label = new TextBlock
                {
                    Text = val.ToString("0.0"),
                    Style = captionStyle,
                    Foreground = textBrush,
                    Width = 35,
                    TextAlignment = TextAlignment.Right
                };
                Canvas.SetLeft(label, 0);
                Canvas.SetTop(label, yPos - 8);
                ChartCanvas.Children.Add(label);
            }

            var polyline = new Polyline
            {
                Stroke = LineColor,
                StrokeThickness = 2
            };

            double totalLength = 0;
            Point prevPoint = new Point(-1, -1);

            foreach (var pt in pointsList)
            {
                double px = 40 + ((pt.date.Ticks - minX) / (maxX - minX)) * width;
                double py = height - ((pt.value - minY) / (maxY - minY)) * height;
                Point currentPoint = new Point(px, py);
                polyline.Points.Add(currentPoint);

                if (prevPoint.X != -1)
                {
                    totalLength += Math.Sqrt(Math.Pow(currentPoint.X - prevPoint.X, 2) + Math.Pow(currentPoint.Y - prevPoint.Y, 2));
                }
                prevPoint = currentPoint;

                var ellipse = new Ellipse
                {
                    Width = 8, Height = 8, Fill = LineColor, Stroke = Brushes.White, StrokeThickness = 2,
                    ToolTip = $"{pt.date:MMM dd}\nValue: {pt.value:0.0}"
                };
                Canvas.SetLeft(ellipse, px - 4);
                Canvas.SetTop(ellipse, py - 4);
                
                // Add hover effect
                ellipse.MouseEnter += (s, e) => { ellipse.Width = 12; ellipse.Height = 12; Canvas.SetLeft(ellipse, px - 6); Canvas.SetTop(ellipse, py - 6); };
                ellipse.MouseLeave += (s, e) => { ellipse.Width = 8; ellipse.Height = 8; Canvas.SetLeft(ellipse, px - 4); Canvas.SetTop(ellipse, py - 4); };

                ChartCanvas.Children.Add(ellipse);
            }

            ChartCanvas.Children.Add(polyline);

            // Animate Polyline
            if (totalLength > 0)
            {
                polyline.StrokeDashArray = new DoubleCollection(new[] { totalLength, totalLength });
                polyline.StrokeDashOffset = totalLength;

                var anim = new DoubleAnimation(totalLength, 0, TimeSpan.FromMilliseconds(1200));
                anim.EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut };
                polyline.BeginAnimation(Shape.StrokeDashOffsetProperty, anim);
            }
        }
    }
}
