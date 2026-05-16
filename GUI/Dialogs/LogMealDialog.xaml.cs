using FitTrack.Models;
using FitTrack.Services;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace FitTrack.UI.Dialogs
{
    public partial class LogMealDialog : Window
    {
        private readonly NutritionService _nutrition = new NutritionService();
        private FoodItem? _selectedFood;

        public LogMealDialog()
        {
            InitializeComponent();
            LoadFoods();
        }

        private void LoadFoods()
        {
            FoodList.ItemsSource = _nutrition.GetAllFoodItems();
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var q = SearchBox.Text;
            if (string.IsNullOrWhiteSpace(q))
                LoadFoods();
            else
                FoodList.ItemsSource = _nutrition.SearchFood(q);
        }

        private void FoodList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _selectedFood = FoodList.SelectedItem as FoodItem;
        }

        private void LogMeal_Click(object sender, RoutedEventArgs e)
        {
            ErrorText.Text = "";
            SuccessText.Text = "";

            if (_selectedFood == null)
            {
                ErrorText.Text = "Please select a food item.";
                return;
            }

            if (!double.TryParse(ServingBox.Text, out double grams) || grams <= 0)
            {
                ErrorText.Text = "Please enter valid serving grams.";
                return;
            }

            var mealType = (MealTypeCombo.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "Snack";
            var result = _nutrition.LogMeal(App.CurrentUser!.PersonId, _selectedFood.FoodItemId, mealType, grams);

            if (result.success)
            {
                SuccessText.Text = "Meal logged successfully!";
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
