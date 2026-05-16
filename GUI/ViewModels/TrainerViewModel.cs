using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FitTrack.Models;
using System.Collections.ObjectModel;
using System.Linq;

namespace FitTrack.UI.ViewModels
{
    public partial class TrainerViewModel : BaseViewModel
    {
        public ObservableCollection<Person> AssignedTrainees { get; } = new ObservableCollection<Person>();
        
        [ObservableProperty] private Person? _selectedTrainee;
        [ObservableProperty] private string _traineeDetailsText = string.Empty;

        public TrainerViewModel()
        {
            if (App.CurrentUser != null) LoadData();
        }

        private void LoadData()
        {
            var myTrainees = _users.GetTraineesByTrainer(App.CurrentUser!.PersonId);
            
            AssignedTrainees.Clear();
            foreach (var t in myTrainees) AssignedTrainees.Add(t);
        }

        partial void OnSelectedTraineeChanged(Person? value)
        {
            if (value != null)
            {
                var goalName = "None";
                if (value.GoalId.HasValue)
                {
                    var g = _goals.GetGoalById(value.GoalId.Value);
                    if (g != null) goalName = g.GoalName;
                }
                TraineeDetailsText = $"Name: {value.FullName}\nUsername: {value.Username}\nEmail: {value.Email}\nAge: {value.Age}\nGoal: {goalName}\nBMI: {value.CalculateBMI():0.0}";
            }
            else
            {
                TraineeDetailsText = string.Empty;
            }
        }

        [RelayCommand]
        private void AssignPlan()
        {
            if (SelectedTrainee == null)
            {
                SetError("Please select a trainee first.");
                return;
            }
            System.Windows.MessageBox.Show($"Assign Plan to {SelectedTrainee.FullName}", "FitTrack Pro", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
        }
    }
}
