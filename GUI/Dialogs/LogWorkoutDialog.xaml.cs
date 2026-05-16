using FitTrack.Models;
using FitTrack.Services;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media.Animation;

namespace FitTrack.UI.Dialogs
{
    public partial class LogWorkoutDialog : Window
    {
        private readonly WorkoutService _workouts = new WorkoutService();
        private readonly List<(int exerciseId, int setNumber, int? actualReps, int? actualSeconds, double? weightKg)> _sets = new();
        private int _setCount = 0;

        public LogWorkoutDialog()
        {
            InitializeComponent();
            ExerciseCombo.ItemsSource = _workouts.GetAllExercises();
        }

        private void AddSet_Click(object sender, RoutedEventArgs e)
        {
            ErrorText.Text = "";

            var exercise = ExerciseCombo.SelectedItem as Exercise;
            if (exercise == null)
            {
                ErrorText.Text = "Please select an exercise.";
                return;
            }

            int? reps = null;
            if (!string.IsNullOrWhiteSpace(RepsBox.Text) && int.TryParse(RepsBox.Text, out int r))
                reps = r;

            double? weight = null;
            if (!string.IsNullOrWhiteSpace(WeightBox.Text) && double.TryParse(WeightBox.Text, out double w))
                weight = w;

            _setCount++;
            _sets.Add((exercise.ExerciseId, _setCount, reps, null, weight));
            SetsList.Items.Add($"Set {_setCount}: {exercise.Name} — {reps ?? 0} reps @ {weight ?? 0} kg");
        }

        private void LogSession_Click(object sender, RoutedEventArgs e)
        {
            ErrorText.Text = "";

            if (!int.TryParse(DurationBox.Text, out int duration) || duration <= 0)
            {
                ErrorText.Text = "Please enter a valid duration.";
                return;
            }

            if (_sets.Count == 0)
            {
                ErrorText.Text = "Please add at least one set.";
                return;
            }

            var notes = string.IsNullOrWhiteSpace(NotesBox.Text) ? null : NotesBox.Text;
            var activePlan = _workouts.GetActivePlan(App.CurrentUser!.PersonId);
            
            var result = _workouts.LogSession(App.CurrentUser!.PersonId, activePlan?.PlanId, duration, notes, _sets);

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
