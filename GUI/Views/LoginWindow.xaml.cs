using FitTrack.UI.ViewModels;
using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace FitTrack.UI.Views
{
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
        }

        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                DragMove();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is LoginViewModel vm)
            {
                vm.Password = PasswordBox.Password;
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Right half entrance animation
            FormContainer.Opacity = 0;
            var slideIn = new DoubleAnimation(40, 0, TimeSpan.FromMilliseconds(400))
            {
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
            };
            var fadeIn = new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(400))
            {
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
            };

            FormContainer.RenderTransform.BeginAnimation(System.Windows.Media.TranslateTransform.XProperty, slideIn);
            FormContainer.BeginAnimation(OpacityProperty, fadeIn);

            // Background circles animation
            AnimateCircle(Circle1, 20, 12000);
            AnimateCircle(Circle2, 30, 9000);
            AnimateCircle(Circle3, 25, 10000);
        }

        private void AnimateCircle(UIElement circle, double driftDist, int durationMs)
        {
            var transform = new System.Windows.Media.TranslateTransform();
            circle.RenderTransform = transform;

            var animX = new DoubleAnimation(0, driftDist, TimeSpan.FromMilliseconds(durationMs))
            {
                AutoReverse = true,
                RepeatBehavior = RepeatBehavior.Forever,
                EasingFunction = new SineEase { EasingMode = EasingMode.EaseInOut }
            };
            var animY = new DoubleAnimation(0, driftDist, TimeSpan.FromMilliseconds(durationMs + 2000))
            {
                AutoReverse = true,
                RepeatBehavior = RepeatBehavior.Forever,
                EasingFunction = new SineEase { EasingMode = EasingMode.EaseInOut }
            };

            transform.BeginAnimation(System.Windows.Media.TranslateTransform.XProperty, animX);
            transform.BeginAnimation(System.Windows.Media.TranslateTransform.YProperty, animY);
        }
    }
}
