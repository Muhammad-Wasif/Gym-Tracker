namespace FitTrack.Models;

public class Exercise
{
    public int ExerciseId { get; set; }
    public int CategoryId { get; set; }
    public int? GoalId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string MuscleGroup { get; set; } = string.Empty;
    public string Equipment { get; set; } = string.Empty;
    public int DefaultSets { get; set; }
    public int? DefaultReps { get; set; }
    public int? DefaultSecs { get; set; }
    public double METValue { get; set; }
    public string? Description { get; set; }

    public double CalculateCaloriesBurned(double weightKg, int durationSeconds)
    {
        return Math.Round(METValue * weightKg * (durationSeconds / 3600.0), 2);
    }

    public override string ToString()
    {
        string volume = DefaultReps.HasValue ? $"{DefaultSets}x{DefaultReps}" : $"{DefaultSets}x{DefaultSecs}s";
        return $"[{ExerciseId}] {Name} | {CategoryName} | {MuscleGroup} | {Equipment} | {volume}";
    }
}
