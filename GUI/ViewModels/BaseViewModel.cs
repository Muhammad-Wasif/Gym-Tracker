using CommunityToolkit.Mvvm.ComponentModel;
using FitTrack.Models;
using FitTrack.Services;

namespace FitTrack.UI.ViewModels
{
    public partial class BaseViewModel : ObservableObject
    {
        protected Person CurrentUser => App.CurrentUser!;

        protected readonly AuthService _auth = new AuthService();
        protected readonly UserService _users = new UserService();
        protected readonly GoalService _goals = new GoalService();
        protected readonly WorkoutService _workouts = new WorkoutService();
        protected readonly NutritionService _nutrition = new NutritionService();
        protected readonly ProgressService _progress = new ProgressService();

        [ObservableProperty]
        private bool _isBusy;

        [ObservableProperty]
        private string _errorMessage = string.Empty;

        [ObservableProperty]
        private string _successMessage = string.Empty;

        protected void SetError(string message)
        {
            ErrorMessage = message;
            SuccessMessage = string.Empty;
        }

        protected void SetSuccess(string message)
        {
            SuccessMessage = message;
            ErrorMessage = string.Empty;
        }

        protected void ClearMessages()
        {
            ErrorMessage = string.Empty;
            SuccessMessage = string.Empty;
        }
    }
}
