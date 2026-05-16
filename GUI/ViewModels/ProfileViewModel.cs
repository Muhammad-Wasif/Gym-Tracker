using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FitTrack.Models;
using System.Collections.ObjectModel;
using System.Windows.Media;
using System.Linq;

namespace FitTrack.UI.ViewModels
{
    public partial class ProfileViewModel : BaseViewModel
    {
        [ObservableProperty] private string _fullName = string.Empty;
        [ObservableProperty] private string _email = string.Empty;
        [ObservableProperty] private string _gender = string.Empty;
        [ObservableProperty] private int _age;
        [ObservableProperty] private double _heightCm;
        [ObservableProperty] private double _weightKg;
        [ObservableProperty] private double? _bodyFatPct;
        [ObservableProperty] private Goal? _selectedGoal;
        
        [ObservableProperty] private string _username = string.Empty;
        [ObservableProperty] private string _role = string.Empty;
        [ObservableProperty] private string _memberSince = string.Empty;
        [ObservableProperty] private string _trainerName = string.Empty;
        
        [ObservableProperty] private double _bMI;
        [ObservableProperty] private string _bMICategory = string.Empty;
        [ObservableProperty] private double _bMR;
        [ObservableProperty] private double _tDEE;
        [ObservableProperty] private double _targetCalories;
        
        [ObservableProperty] private string _initialsText = string.Empty;
        
        [ObservableProperty] private string _currentPassword = string.Empty;
        [ObservableProperty] private string _newPassword = string.Empty;
        [ObservableProperty] private string _confirmNewPassword = string.Empty;

        public ObservableCollection<Goal> Goals { get; } = new ObservableCollection<Goal>();

        public ProfileViewModel()
        {
            if (App.CurrentUser != null)
                LoadData();
        }

        public void LoadData()
        {
            Goals.Clear();
            foreach (var g in _goals.GetAllGoals()) Goals.Add(g);

            var user = App.CurrentUser!;
            FullName = user.FullName;
            Email = user.Email;
            Gender = user.Gender;
            Age = user.Age;
            HeightCm = user.HeightCm;
            WeightKg = user.WeightKg;
            BodyFatPct = user.BodyFatPct;
            
            if (user.GoalId.HasValue)
            {
                SelectedGoal = Goals.FirstOrDefault(g => g.GoalId == user.GoalId.Value);
            }

            Username = user.Username;
            Role = user.Role;
            MemberSince = user.CreatedAt.ToString("MMM yyyy");
            
            if (user is Trainee t && !string.IsNullOrEmpty(t.TrainerName))
                TrainerName = t.TrainerName;

            var metrics = _goals.GetHealthMetrics(user.PersonId, 1.375);
            BMI = metrics.bmi;
            BMICategory = metrics.category;
            BMR = metrics.bmr;
            TDEE = metrics.tdee;
            TargetCalories = metrics.targetCalories;

            var parts = FullName.Split(' ', System.StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length > 0)
            {
                InitialsText = parts.Length == 1 ? parts[0].Substring(0, 1).ToUpper() : (parts[0].Substring(0, 1) + parts[parts.Length - 1].Substring(0, 1)).ToUpper();
            }
        }

        [RelayCommand]
        private void SaveProfile()
        {
            var res = _users.UpdateProfile(App.CurrentUser!.PersonId, FullName, Email, Gender, Age, HeightCm, WeightKg, BodyFatPct, SelectedGoal?.GoalId);
            if (res.success)
            {
                App.CurrentUser!.FullName = FullName;
                App.CurrentUser!.Email = Email;
                App.CurrentUser!.Gender = Gender;
                App.CurrentUser!.Age = Age;
                App.CurrentUser!.HeightCm = HeightCm;
                App.CurrentUser!.WeightKg = WeightKg;
                App.CurrentUser!.BodyFatPct = BodyFatPct;
                App.CurrentUser!.GoalId = SelectedGoal?.GoalId;
                SetSuccess("Profile updated successfully");
                LoadData(); // reload stats
            }
            else
            {
                SetError(res.message);
            }
        }

        [RelayCommand]
        private void ChangePassword()
        {
            if (string.IsNullOrEmpty(CurrentPassword) || string.IsNullOrEmpty(NewPassword))
            {
                SetError("Please enter current and new password");
                return;
            }
            if (NewPassword != ConfirmNewPassword)
            {
                SetError("New passwords do not match");
                return;
            }

            var res = _auth.ChangePassword(App.CurrentUser!.PersonId, CurrentPassword, NewPassword);
            if (res.success) SetSuccess("Password changed");
            else SetError(res.message);
        }
    }
}
