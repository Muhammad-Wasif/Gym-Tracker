using FitTrack.Services;
using System;
using System.Windows;
using System.Windows.Media.Animation;

namespace FitTrack.UI.Dialogs
{
    public partial class LogProgressDialog : Window
    {
        private readonly ProgressService _progress = new ProgressService();

        public LogProgressDialog()
        {
            InitializeComponent();
            WeightBox.Text = App.CurrentUser?.WeightKg.ToString("0.0") ?? "";
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            ErrorText.Text = "";

            if (!double.TryParse(WeightBox.Text, out double weight) || weight <= 0)
            {
                ErrorText.Text = "Please enter a valid weight.";
                return;
            }

            double? bodyFat = null;
            if (!string.IsNullOrWhiteSpace(BodyFatBox.Text))
            {
                if (!double.TryParse(BodyFatBox.Text, out double bf))
                {
                    ErrorText.Text = "Please enter a valid body fat percentage.";
                    return;
                }
                bodyFat = bf;
            }

            var notes = string.IsNullOrWhiteSpace(NotesBox.Text) ? null : NotesBox.Text;
            var result = _progress.LogSnapshot(App.CurrentUser!.PersonId, weight, bodyFat, notes);

            if (result.success)
            {
                DialogResult = true;
                Close();
            }
            else
            {
                ErrorText.Text = result.message;
            }
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            FormContainer.Opacity = 0;
            var slideIn = new DoubleAnimation(20, 0, TimeSpan.FromMilliseconds(300))
            {
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
            };
            var fadeIn = new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(300))
            {
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
            };
            FormContainer.RenderTransform.BeginAnimation(System.Windows.Media.TranslateTransform.YProperty, slideIn);
            FormContainer.BeginAnimation(OpacityProperty, fadeIn);
        }
    }
}
