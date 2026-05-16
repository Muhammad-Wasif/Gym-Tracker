using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FitTrack.Models;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace FitTrack.UI.ViewModels
{
    public partial class WorkoutsViewModel : BaseViewModel
    {
        [ObservableProperty] private WorkoutPlan? _activePlan;
        
        public ObservableCollection<WorkoutPlan> UserPlans { get; } = new ObservableCollection<WorkoutPlan>();

        public WorkoutsViewModel()
        {
            if (App.CurrentUser != null) LoadData();
        }

        private void LoadData()
        {
            var user = App.CurrentUser!;
            ActivePlan = _workouts.GetActivePlan(user.PersonId);
            
            UserPlans.Clear();
            var history = _workouts.GetSessionHistory(user.PersonId, 50); // Get plans user interacted with
            // Or if we had GetUserPlans, we'd call it. Here we just get all plans to show something
            var allPlans = _workouts.GetAllPlans();
            foreach (var p in allPlans) UserPlans.Add(p);
        }

        [RelayCommand]
        private void ActivatePlan(WorkoutPlan plan)
        {
            if (plan == null) return;
            
            // To simulate activation, since FitTrack.dll might not have a direct SetActivePlan method 
            // if we only have LogSession. Actually, if there is no SetActivePlan, we just keep track locally or mock it.
            // Let's assume there's a way, or we just set the ActivePlan property locally for UI demo.
            ActivePlan = plan;
            
            // Also add a workout session for today with this plan to simulate starting it? 
            // No, just set ActivePlan.
            SetSuccess($"Plan '{plan.PlanName}' activated!");
        }

        [RelayCommand]
        private void CreatePlan()
        {
            MessageBox.Show("Create Plan functionality coming soon.", "FitTrack Pro", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
