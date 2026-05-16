using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace FitTrack.UI.Controls
{
    public partial class AnimatedCounter : UserControl
    {
        public static readonly DependencyProperty TargetValueProperty =
            DependencyProperty.Register("TargetValue", typeof(double), typeof(AnimatedCounter), new PropertyMetadata(0.0, OnTargetValueChanged));

        public static readonly DependencyProperty FormatProperty =
            DependencyProperty.Register("Format", typeof(string), typeof(AnimatedCounter), new PropertyMetadata("0.0"));

        public double TargetValue
        {
            get { return (double)GetValue(TargetValueProperty); }
            set { SetValue(TargetValueProperty, value); }
        }

        public string Format
        {
            get { return (string)GetValue(FormatProperty); }
            set { SetValue(FormatProperty, value); }
        }

        public AnimatedCounter()
        {
            InitializeComponent();
            Loaded += (s, e) => AnimateValue();
        }

        private static void OnTargetValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is AnimatedCounter counter && counter.IsLoaded)
            {
                counter.AnimateValue();
            }
        }

        private void AnimateValue()
        {
            var anim = new DoubleAnimation(0, TargetValue, TimeSpan.FromMilliseconds(800));
            anim.EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut };
            anim.FillBehavior = FillBehavior.Stop;

            double current = 0;
            anim.CurrentTimeInvalidated += (s, ev) =>
            {
                if (s is AnimationClock clock && clock.CurrentProgress.HasValue)
                {
                    current = TargetValue * clock.CurrentProgress.Value;
                    ValueText.Text = current.ToString(Format);
                }
            };
            anim.Completed += (s, ev) => ValueText.Text = TargetValue.ToString(Format);

            var dummyAnim = new DoubleAnimation(1, 1, TimeSpan.FromMilliseconds(800));
            dummyAnim.CurrentTimeInvalidated += (s, ev) =>
            {
                if (s is AnimationClock clock && clock.CurrentProgress.HasValue)
                {
                    current = TargetValue * clock.CurrentProgress.Value;
                    ValueText.Text = current.ToString(Format);
                }
            };
            dummyAnim.Completed += (s, ev) => ValueText.Text = TargetValue.ToString(Format);
            ValueText.BeginAnimation(OpacityProperty, dummyAnim);
        }
    }
}
