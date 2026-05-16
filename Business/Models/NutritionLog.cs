namespace FitTrack.Models;

public class NutritionLog
{
    public int NutritionLogId { get; set; }
    public int PersonId { get; set; }
    public int FoodItemId { get; set; }
    public string FoodName { get; set; } = string.Empty;
    public string MealType { get; set; } = string.Empty;
    public double ServingGrams { get; set; }
    public double Calories { get; set; }
    public double ProteinG { get; set; }
    public double CarbsG { get; set; }
    public double FatG { get; set; }
    public DateTime LoggedAt { get; set; }

    public override string ToString()
    {
        return $"[{NutritionLogId}] {MealType}: {FoodName} {ServingGrams}g | {Calories} kcal | P:{ProteinG}g C:{CarbsG}g F:{FatG}g";
    }
}
