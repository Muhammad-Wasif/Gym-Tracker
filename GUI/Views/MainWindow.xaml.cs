using FitTrack.UI.Views.Pages;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace FitTrack.UI.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (App.CurrentUser != null)
            {
                UserFullName.Text = App.CurrentUser.FullName;
                UserRole.Text = App.CurrentUser.Role;
                
                var parts = App.CurrentUser.FullName.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length > 0)
                {
                    UserInitials.Text = parts.Length == 1 ? parts[0].Substring(0, 1).ToUpper() : (parts[0].Substring(0, 1) + parts[parts.Length - 1].Substring(0, 1)).ToUpper();
                }

                if (App.CurrentUser.Role == "Trainee")
                {
                    TrainerNav.Visibility = Visibility.Collapsed;
                    AdminNav.Visibility = Visibility.Collapsed;
                    RoleSeparator.Visibility = Visibility.Collapsed;
                }
                else if (App.CurrentUser.Role == "Trainer")
                {
                    AdminNav.Visibility = Visibility.Collapsed;
                }
            }

            Navigate("Dashboard");
        }

        private void TitleBar_Drag(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                DragMove();
        }

        private void Minimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void NavItem_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is string tag)
            {
                foreach (var child in NavItemsPanel.Children)
                {
                    if (child is Button b) b.IsDefault = false;
                }
                btn.IsDefault = true;

                Navigate(tag);
            }
        }

        private void ProfileCard_Click(object sender, MouseButtonEventArgs e)
        {
            foreach (var child in NavItemsPanel.Children)
            {
                if (child is Button b) b.IsDefault = false;
            }
            Navigate("Profile");
        }

        private void SignOut_Click(object sender, RoutedEventArgs e)
        {
            App.Logout();
        }

        private void Navigate(string pageName)
        {
            Page page = pageName switch
            {
                "Dashboard" => new DashboardPage(),
                "Workouts" => new WorkoutsPage(),
                "Exercises" => new ExercisesPage(),
                "Nutrition" => new NutritionPage(),
                "Progress" => new ProgressPage(),
                "Trainer" => new TrainerPage(),
                "Admin" => new AdminPage(),
                "Profile" => new ProfilePage(),
                _ => new DashboardPage()
            };

            MainFrame.Navigate(page);

            // Animate page content
            page.Loaded += (s, e) =>
            {
                page.Opacity = 0;
                var tt = new System.Windows.Media.TranslateTransform(-20, 0);
                page.RenderTransform = tt;

                var slideIn = new DoubleAnimation(-20, 0, TimeSpan.FromMilliseconds(280))
                {
                    EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
                };
                var fadeIn = new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(280))
                {
                    EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
                };

                tt.BeginAnimation(System.Windows.Media.TranslateTransform.XProperty, slideIn);
                page.BeginAnimation(OpacityProperty, fadeIn);
            };
        }
    }
}
