using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace FitTrack.UI.Controls
{
    public partial class MacroBar : UserControl
    {
        public static readonly DependencyProperty LabelProperty =
            DependencyProperty.Register("Label", typeof(string), typeof(MacroBar), new PropertyMetadata(string.Empty));

        public static readonly DependencyProperty ActualGramsProperty =
            DependencyProperty.Register("ActualGrams", typeof(double), typeof(MacroBar), new PropertyMetadata(0.0, OnValuesChanged));

        public static readonly DependencyProperty TargetGramsProperty =
            DependencyProperty.Register("TargetGrams", typeof(double), typeof(MacroBar), new PropertyMetadata(0.0, OnValuesChanged));

        public static readonly DependencyProperty BarColorProperty =
            DependencyProperty.Register("BarColor", typeof(Brush), typeof(MacroBar), new PropertyMetadata(Brushes.Transparent));

        public string Label
        {
            get { return (string)GetValue(LabelProperty); }
            set { SetValue(LabelProperty, value); }
        }

        public double ActualGrams
        {
            get { return (double)GetValue(ActualGramsProperty); }
            set { SetValue(ActualGramsProperty, value); }
        }

        public double TargetGrams
        {
            get { return (double)GetValue(TargetGramsProperty); }
            set { SetValue(TargetGramsProperty, value); }
        }

        public Brush BarColor
        {
            get { return (Brush)GetValue(BarColorProperty); }
            set { SetValue(BarColorProperty, value); }
        }

        public MacroBar()
        {
            InitializeComponent();
        }

        private static void OnValuesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is MacroBar bar && bar.IsLoaded)
            {
                bar.AnimateFill();
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            AnimateFill();
        }

        private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            AnimateFill();
        }

        private void AnimateFill()
        {
            double targetWidth = 0;
            if (TargetGrams > 0)
            {
                var pct = ActualGrams / TargetGrams;
                if (pct > 1) pct = 1;
                targetWidth = BgBorder.ActualWidth * pct;
            }

            var anim = new DoubleAnimation(targetWidth, TimeSpan.FromMilliseconds(600));
            anim.EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut };
            FillBorder.BeginAnimation(WidthProperty, anim);
        }
    }
}
