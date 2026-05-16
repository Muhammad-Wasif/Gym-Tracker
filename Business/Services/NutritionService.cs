using FitTrack.Database;
using FitTrack.Models;

namespace FitTrack.Services;

public class NutritionService
{
    private NutritionRepository _nutritionRepo = new NutritionRepository();
    private PersonRepository _personRepo = new PersonRepository();
    private GoalRepository _goalRepo = new GoalRepository();

    public List<FoodItem> GetAllFoodItems()
    {
        return _nutritionRepo.GetAllFoodItems();
    }

    public List<FoodItem> SearchFood(string query)
    {
        return _nutritionRepo.SearchFoodItems(query);
    }

    public (bool success, string message, NutritionLog? log) LogMeal(
        int personId, int foodItemId, string mealType, double servingGrams)
    {
        Person? person = _personRepo.GetById(personId);
        if (person == null)
            return (false, "User not found.", null);

        FoodItem? food = _nutritionRepo.GetFoodItemById(foodItemId);
        if (food == null)
            return (false, "Food item not found.", null);

        if (servingGrams <= 0)
            return (false, "Serving grams must be greater than zero.", null);

        NutritionLog log = new NutritionLog
        {
            PersonId     = personId,
            FoodItemId   = foodItemId,
            FoodName     = food.FoodName,
            MealType     = mealType,
            ServingGrams = servingGrams,
            Calories     = food.CalculateCalories(servingGrams),
            ProteinG     = food.CalculateProtein(servingGrams),
            CarbsG       = food.CalculateCarbs(servingGrams),
            FatG         = food.CalculateFat(servingGrams),
            LoggedAt     = DateTime.Now
        };

        int logId = _nutritionRepo.InsertNutritionLog(log);
        log.NutritionLogId = logId;

        return (true, "Meal logged.", log);
    }

    public (double calories, double protein, double carbs, double fat, List<NutritionLog> meals)
        GetDailySummary(int personId, DateTime date)
    {
        List<NutritionLog> meals = _nutritionRepo.GetLogsForDay(personId, date);

        double calories = 0, protein = 0, carbs = 0, fat = 0;
        foreach (NutritionLog meal in meals)
        {
            calories += meal.Calories;
            protein  += meal.ProteinG;
            carbs    += meal.CarbsG;
            fat      += meal.FatG;
        }

        return (Math.Round(calories, 2), Math.Round(protein, 2),
                Math.Round(carbs, 2), Math.Round(fat, 2), meals);
    }

    public List<NutritionLog> GetRecentLogs(int personId, int days = 7)
    {
        return _nutritionRepo.GetRecentLogs(personId, days);
    }

    public (bool success, string message) DeleteLog(int nutritionLogId)
    {
        _nutritionRepo.DeleteLog(nutritionLogId);
        return (true, "Log deleted.");
    }
}
