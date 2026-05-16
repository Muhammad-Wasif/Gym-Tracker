using CommunityToolkit.Mvvm.ComponentModel;
using FitTrack.Models;
using System.Collections.ObjectModel;
using System.Linq;

namespace FitTrack.UI.ViewModels
{
    public partial class ExercisesViewModel : BaseViewModel
    {
        [ObservableProperty] private string _searchQuery = string.Empty;
        [ObservableProperty] private Exercise? _selectedExercise;

        public ObservableCollection<Exercise> FilteredExercises { get; } = new ObservableCollection<Exercise>();
        private ObservableCollection<Exercise> _allExercises = new ObservableCollection<Exercise>();

        public ExercisesViewModel()
        {
            if (App.CurrentUser != null) LoadData();
        }

        private void LoadData()
        {
            _allExercises.Clear();
            var list = _workouts.GetAllExercises();
            foreach (var ex in list) _allExercises.Add(ex);
            FilterExercises();
        }

        partial void OnSearchQueryChanged(string value)
        {
            FilterExercises();
        }

        private void FilterExercises()
        {
            FilteredExercises.Clear();
            var q = SearchQuery?.ToLower() ?? "";
            
            var res = string.IsNullOrWhiteSpace(q) 
                ? _allExercises 
                : _allExercises.Where(e => e.Name.ToLower().Contains(q) || e.MuscleGroup.ToLower().Contains(q));
                
            foreach (var r in res) FilteredExercises.Add(r);
        }
    }
}
