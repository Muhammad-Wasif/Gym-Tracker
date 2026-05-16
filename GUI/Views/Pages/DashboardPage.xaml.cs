using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace FitTrack.UI.Views.Pages
{
    public partial class DashboardPage : Page
    {
        public DashboardPage()
        {
            InitializeComponent();
            Loaded += DashboardPage_Loaded;
        }

        private void DashboardPage_Loaded(object sender, RoutedEventArgs e)
        {
            AnimateIn(HeaderRow, 20, 0);
            AnimateListItems(StatsRow, 80);
            AnimateIn(NutritionCard, 20, 200);
            AnimateIn(WorkoutCard, 20, 280);
        }

        private void AnimateIn(UIElement element, double fromY = 20, double delay = 0)
        {
            element.Opacity = 0;
            var tt = new System.Windows.Media.TranslateTransform(0, fromY);
            element.RenderTransform = tt;

            var sb = new Storyboard();

            var fadeIn = new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(350));
            fadeIn.EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut };
            fadeIn.BeginTime = TimeSpan.FromMilliseconds(delay);
            Storyboard.SetTarget(fadeIn, element);
            Storyboard.SetTargetProperty(fadeIn, new PropertyPath(UIElement.OpacityProperty));

            var slideIn = new DoubleAnimation(fromY, 0, TimeSpan.FromMilliseconds(350));
            slideIn.EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut };
            slideIn.BeginTime = TimeSpan.FromMilliseconds(delay);
            Storyboard.SetTarget(slideIn, tt);
            Storyboard.SetTargetProperty(slideIn, new PropertyPath(System.Windows.Media.TranslateTransform.YProperty));

            sb.Children.Add(fadeIn);
            sb.Children.Add(slideIn);
            sb.Begin();
        }

        private void AnimateListItems(Panel container, double delayMs = 60)
        {
            int i = 0;
            foreach (UIElement child in container.Children)
            {
                AnimateIn(child, 16, i * delayMs);
                i++;
            }
        }
    }
}
