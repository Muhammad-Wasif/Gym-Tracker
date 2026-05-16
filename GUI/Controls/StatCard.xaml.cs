using FontAwesome.WPF;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace FitTrack.UI.Controls
{
    public partial class StatCard : UserControl
    {
        public static readonly DependencyProperty IconGlyphProperty =
            DependencyProperty.Register("IconGlyph", typeof(string), typeof(StatCard), new PropertyMetadata("None", OnIconGlyphChanged));

        public static readonly DependencyProperty LabelProperty =
            DependencyProperty.Register("Label", typeof(string), typeof(StatCard), new PropertyMetadata(string.Empty));

        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(string), typeof(StatCard), new PropertyMetadata(string.Empty, OnValueChanged));

        public static readonly DependencyProperty SubtitleProperty =
            DependencyProperty.Register("Subtitle", typeof(string), typeof(StatCard), new PropertyMetadata(string.Empty));

        public static readonly DependencyProperty AccentColorProperty =
            DependencyProperty.Register("AccentColor", typeof(Brush), typeof(StatCard), new PropertyMetadata(Brushes.Transparent));

        public string IconGlyph
        {
            get { return (string)GetValue(IconGlyphProperty); }
            set { SetValue(IconGlyphProperty, value); }
        }

        public string Label
        {
            get { return (string)GetValue(LabelProperty); }
            set { SetValue(LabelProperty, value); }
        }

        public string Value
        {
            get { return (string)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        public string Subtitle
        {
            get { return (string)GetValue(SubtitleProperty); }
            set { SetValue(SubtitleProperty, value); }
        }

        public Brush AccentColor
        {
            get { return (Brush)GetValue(AccentColorProperty); }
            set { SetValue(AccentColorProperty, value); }
        }

        public StatCard()
        {
            InitializeComponent();
        }

        private static void OnIconGlyphChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is StatCard card)
            {
                var val = e.NewValue?.ToString();
                if (Enum.TryParse<FontAwesomeIcon>(val, true, out var icon))
                {
                    card.IconImage.Icon = icon;
                }
            }
        }

        private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is StatCard card && card.IsLoaded)
            {
                card.AnimateValue();
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            AnimateValue();
        }

        private void AnimateValue()
        {
            if (string.IsNullOrEmpty(Value)) return;

            string format = "0.0";
            double target = 0;
            string suffix = "";
            
            var str = Value;
            if (str.EndsWith(" kg"))
            {
                suffix = " kg";
                str = str.Replace(" kg", "");
            }

            if (!double.TryParse(str, out target))
            {
                // Can't parse as double, just display it
                ValueText.Text = Value;
                return;
            }

            if (target % 1 == 0) format = "0";

            var anim = new DoubleAnimation(0, target, TimeSpan.FromMilliseconds(800));
            anim.EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut };
            anim.FillBehavior = FillBehavior.Stop;

            double current = 0;
            anim.CurrentTimeInvalidated += (s, ev) =>
            {
                if (s is AnimationClock clock && clock.CurrentProgress.HasValue)
                {
                    current = target * clock.CurrentProgress.Value;
                    ValueText.Text = current.ToString(format) + suffix;
                }
            };
            anim.Completed += (s, ev) => ValueText.Text = target.ToString(format) + suffix;

            ValueText.BeginAnimation(OpacityProperty, null); // Clear prev
            var clock2 = anim.CreateClock();
            ValueText.ApplyAnimationClock(OpacityProperty, null); // Need a property to apply clock to, Opacity is just a dummy target to drive the clock. 
            // Better way is to use a dummy property or just rely on CurrentTimeInvalidated of a standalone clock.
            // Actually, we can use an empty DoubleAnimation on Opacity just to run the clock and hook CurrentTimeInvalidated.
            
            var dummyAnim = new DoubleAnimation(1, 1, TimeSpan.FromMilliseconds(800));
            dummyAnim.CurrentTimeInvalidated += (s, ev) =>
            {
                if (s is AnimationClock clock && clock.CurrentProgress.HasValue)
                {
                    current = target * clock.CurrentProgress.Value;
                    ValueText.Text = current.ToString(format) + suffix;
                }
            };
            dummyAnim.Completed += (s, ev) => ValueText.Text = target.ToString(format) + suffix;
            ValueText.BeginAnimation(OpacityProperty, dummyAnim);
        }
    }
}
