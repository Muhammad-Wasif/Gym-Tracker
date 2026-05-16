namespace FitTrack.Models;

public abstract class Person
{
    public int PersonId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public string Gender { get; set; } = string.Empty;
    public int Age { get; set; }
    public double HeightCm { get; set; }
    public double WeightKg { get; set; }
    public double? BodyFatPct { get; set; }
    public int? GoalId { get; set; }
    public int? TrainerId { get; set; }
    public DateTime CreatedAt { get; set; }

    public abstract string GetRoleLabel();

    public double CalculateBMI()
    {
        double heightM = HeightCm / 100.0;
        return Math.Round(WeightKg / (heightM * heightM), 2);
    }

    public string GetBMICategory()
    {
        double bmi = CalculateBMI();
        if (bmi < 18.5) return "Underweight";
        if (bmi < 25.0) return "Normal";
        if (bmi < 30.0) return "Overweight";
        return "Obese";
    }

    public double CalculateBMR()
    {
        double bmr = 10 * WeightKg + 6.25 * HeightCm - 5 * Age;
        return Gender.Equals("Male", StringComparison.OrdinalIgnoreCase) ? bmr + 5 : bmr - 161;
    }

    public double CalculateTDEE(double activityMultiplier)
    {
        return Math.Round(CalculateBMR() * activityMultiplier, 0);
    }

    public override string ToString()
    {
        return $"[{PersonId}] {FullName} ({Username}) — {GetRoleLabel()}";
    }
}
