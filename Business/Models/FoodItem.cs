namespace FitTrack.Models;

public class FoodCategory
{
    public int FoodCategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;

    public override string ToString()
    {
        return $"[{FoodCategoryId}] {CategoryName}";
    }
}

public class FoodItem
{
    public int FoodItemId { get; set; }
    public int FoodCategoryId { get; set; }
    public int? GoalId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public string FoodName { get; set; } = string.Empty;
    public double CaloriesPer100g { get; set; }
    public double ProteinPer100g { get; set; }
    public double CarbsPer100g { get; set; }
    public double FatPer100g { get; set; }
    public double? FiberPer100g { get; set; }

    public double CalculateCalories(double servingGrams)
    {
        return Math.Round(CaloriesPer100g * servingGrams / 100.0, 2);
    }

    public double CalculateProtein(double servingGrams)
    {
        return Math.Round(ProteinPer100g * servingGrams / 100.0, 2);
    }

    public double CalculateCarbs(double servingGrams)
    {
        return Math.Round(CarbsPer100g * servingGrams / 100.0, 2);
    }

    public double CalculateFat(double servingGrams)
    {
        return Math.Round(FatPer100g * servingGrams / 100.0, 2);
    }

    public override string ToString()
    {
        return $"[{FoodItemId}] {FoodName} | {CaloriesPer100g} kcal | P:{ProteinPer100g}g C:{CarbsPer100g}g F:{FatPer100g}g per 100g";
    }
}
