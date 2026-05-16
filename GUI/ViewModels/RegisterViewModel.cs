using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FitTrack.Models;
using FitTrack.UI.Views;
using System.Collections.ObjectModel;
using System.Windows;

namespace FitTrack.UI.ViewModels
{
    public partial class RegisterViewModel : BaseViewModel
    {
        [ObservableProperty] private string _fullName = string.Empty;
        [ObservableProperty] private string _username = string.Empty;
        [ObservableProperty] private string _password = string.Empty;
        [ObservableProperty] private string _email = string.Empty;
        [ObservableProperty] private string _role = "Trainee";
        [ObservableProperty] private string _gender = "Male";
        [ObservableProperty] private int _age = 25;
        [ObservableProperty] private double _heightCm = 175.0;
        [ObservableProperty] private double _weightKg = 70.0;
        [ObservableProperty] private double? _bodyFatPct;
        [ObservableProperty] private Goal? _selectedGoal;

        public ObservableCollection<Goal> Goals { get; } = new ObservableCollection<Goal>();

        public RegisterViewModel()
        {
            LoadData();
        }

        private void LoadData()
        {
            Goals.Clear();
            var allGoals = _goals.GetAllGoals();
            foreach (var g in allGoals)
            {
                Goals.Add(g);
            }
        }

        [RelayCommand]
        private void Register()
        {
            if (string.IsNullOrWhiteSpace(FullName) || string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password))
            {
                SetError("Please fill out all required fields.");
                return;
            }

            IsBusy = true;
            ClearMessages();

            var result = _auth.Register(FullName, Username, Password, Email, Role, Gender, Age, HeightCm, WeightKg, BodyFatPct, SelectedGoal?.GoalId);
            IsBusy = false;

            if (result.success && result.person != null)
            {
                App.CurrentUser = result.person;
                App.NavigateToMain();
            }
            else
            {
                SetError(result.message ?? "Registration failed.");
            }
        }

        [RelayCommand]
        private void NavigateToLogin()
        {
            var login = new LoginWindow();
            login.Show();
            
            foreach (Window w in Application.Current.Windows)
            {
                if (w is RegisterWindow rw)
                {
                    rw.Close();
                    break;
                }
            }
        }
    }
}
