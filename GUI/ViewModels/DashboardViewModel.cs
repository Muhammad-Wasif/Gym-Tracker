using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FitTrack.Models;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace FitTrack.UI.ViewModels
{
    public partial class DashboardViewModel : BaseViewModel
    {
        [ObservableProperty] private string _greetingPrefix = string.Empty;
        [ObservableProperty] private string _firstName = string.Empty;
        [ObservableProperty] private string _todayDateString = string.Empty;
        [ObservableProperty] private string _goalName = string.Empty;
        
        [ObservableProperty] private double _bMI;
        [ObservableProperty] private string _bMICategory = string.Empty;
        
        [ObservableProperty] private double _todayCalories;
        [ObservableProperty] private double _targetCalories;
        
        [ObservableProperty] private double _currentWeight;
        [ObservableProperty] private string _weightLastUpdated = string.Empty;
        
        [ObservableProperty] private int _weekSessionCount;
        
        [ObservableProperty] private double _proteinActual;
        [ObservableProperty] private double _proteinTarget;
        [ObservableProperty] private double _carbsActual;
        [ObservableProperty] private double _carbsTarget;
        [ObservableProperty] private double _fatActual;
        [ObservableProperty] private double _fatTarget;

        [ObservableProperty] private WorkoutPlan? _activePlan;

        public ObservableCollection<NutritionLog> TodayMeals { get; } = new ObservableCollection<NutritionLog>();
        public ObservableCollection<WorkoutPlanExercise> TodayExercises { get; } = new ObservableCollection<WorkoutPlanExercise>();
        public ObservableCollection<ProgressSnapshot> RecentSnapshots { get; } = new ObservableCollection<ProgressSnapshot>();

        public DashboardViewModel()
        {
            if (App.CurrentUser != null)
                LoadData();
        }

        public void LoadData()
        {
            var user = App.CurrentUser!;
            FirstName = user.FullName.Split(' ').First();
            TodayDateString = DateTime.Now.ToString("dddd, dd MMM");
            
            var hour = DateTime.Now.Hour;
            GreetingPrefix = hour < 12 ? "Good morning," : hour < 18 ? "Good afternoon," : "Good evening,";

            if (user is Trainee t)
            {
                GoalName = t.GoalName ?? "Maintain";
            }
            else
            {
                GoalName = user.Role;
            }

            var metrics = _goals.GetHealthMetrics(user.PersonId, 1.375);
            BMI = metrics.bmi;
            BMICategory = metrics.category;
            TargetCalories = metrics.targetCalories;

            CurrentWeight = user.WeightKg;
            WeightLastUpdated = "Recently";

            var summary = _nutrition.GetDailySummary(user.PersonId, DateTime.Now);
            TodayCalories = summary.calories;
            ProteinActual = summary.protein;
            CarbsActual = summary.carbs;
            FatActual = summary.fat;
            
            ProteinTarget = (TargetCalories * 0.3) / 4;
            CarbsTarget = (TargetCalories * 0.4) / 4;
            FatTarget = (TargetCalories * 0.3) / 9;

            TodayMeals.Clear();
            foreach (var m in summary.meals) TodayMeals.Add(m);

            ActivePlan = _workouts.GetActivePlan(user.PersonId);
            TodayExercises.Clear();
            if (ActivePlan != null)
            {
                int dayOfWeek = (int)DateTime.Now.DayOfWeek;
                if (dayOfWeek == 0) dayOfWeek = 7;
                
                var todays = ActivePlan.Exercises.Where(x => x.DayOfWeek == dayOfWeek).OrderBy(x => x.OrderInDay);
                foreach (var ex in todays) TodayExercises.Add(ex);
            }

            var history = _workouts.GetSessionHistory(user.PersonId, 10);
            WeekSessionCount = history.Count(x => x.SessionDate >= DateTime.Now.AddDays(-7));

            RecentSnapshots.Clear();
            var snaps = _progress.GetSnapshots(user.PersonId, 3);
            foreach (var s in snaps) RecentSnapshots.Add(s);
        }

        [RelayCommand]
        private void LogMeal()
        {
            var dlg = new FitTrack.UI.Dialogs.LogMealDialog();
            if (dlg.ShowDialog() == true) LoadData();
        }

        [RelayCommand]
        private void LogWorkout()
        {
            var dlg = new FitTrack.UI.Dialogs.LogWorkoutDialog();
            if (dlg.ShowDialog() == true) LoadData();
        }

        [RelayCommand]
        private void LogProgress()
        {
            var dlg = new FitTrack.UI.Dialogs.LogProgressDialog();
            if (dlg.ShowDialog() == true) LoadData();
        }

        [RelayCommand] private void BrowsePlans() { }
    }
}
