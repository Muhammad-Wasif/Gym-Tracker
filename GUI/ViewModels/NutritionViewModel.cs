using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FitTrack.Models;
using System;
using System.Collections.ObjectModel;

namespace FitTrack.UI.ViewModels
{
    public partial class NutritionViewModel : BaseViewModel
    {
        [ObservableProperty] private double _caloriesActual;
        [ObservableProperty] private double _proteinActual;
        [ObservableProperty] private double _carbsActual;
        [ObservableProperty] private double _fatActual;
        
        [ObservableProperty] private double _caloriesTarget;
        [ObservableProperty] private double _proteinTarget;
        [ObservableProperty] private double _carbsTarget;
        [ObservableProperty] private double _fatTarget;

        public ObservableCollection<NutritionLog> DailyLogs { get; } = new ObservableCollection<NutritionLog>();

        public NutritionViewModel()
        {
            if (App.CurrentUser != null) LoadData();
        }

        private void LoadData()
        {
            var user = App.CurrentUser!;
            var metrics = _goals.GetHealthMetrics(user.PersonId, 1.375);
            CaloriesTarget = metrics.targetCalories;
            ProteinTarget = (CaloriesTarget * 0.3) / 4;
            CarbsTarget = (CaloriesTarget * 0.4) / 4;
            FatTarget = (CaloriesTarget * 0.3) / 9;

            var summary = _nutrition.GetDailySummary(user.PersonId, DateTime.Now);
            CaloriesActual = summary.calories;
            ProteinActual = summary.protein;
            CarbsActual = summary.carbs;
            FatActual = summary.fat;

            DailyLogs.Clear();
            foreach (var log in summary.meals) DailyLogs.Add(log);
        }

        [RelayCommand]
        private void LogMeal()
        {
            var dlg = new FitTrack.UI.Dialogs.LogMealDialog();
            if (dlg.ShowDialog() == true) LoadData();
        }
    }
}
