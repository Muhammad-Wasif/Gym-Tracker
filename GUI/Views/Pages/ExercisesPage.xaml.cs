using FitTrack.Models;
using FitTrack.UI.ViewModels;
using System.Windows.Controls;

namespace FitTrack.UI.Views.Pages
{
    public partial class ExercisesPage : Page
    {
        public ExercisesPage()
        {
            InitializeComponent();
        }

        private void ExerciseCard_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (sender is Border b && b.DataContext is Exercise ex && DataContext is ExercisesViewModel vm)
            {
                vm.SelectedExercise = ex;
            }
        }
    }
}
