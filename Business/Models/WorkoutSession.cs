namespace FitTrack.Models;

public class SessionLog
{
    public int LogId { get; set; }
    public int SessionId { get; set; }
    public int ExerciseId { get; set; }
    public string ExerciseName { get; set; } = string.Empty;
    public int SetNumber { get; set; }
    public int? ActualReps { get; set; }
    public int? ActualSeconds { get; set; }
    public double? WeightKg { get; set; }
    public double CaloriesBurned { get; set; }

    public override string ToString()
    {
        string volume = ActualReps.HasValue ? $"{ActualReps} reps" : $"{ActualSeconds}s";
        string weight = WeightKg.HasValue ? $" @ {WeightKg}kg" : "";
        return $"  Set {SetNumber}: {ExerciseName} — {volume}{weight} | {CaloriesBurned} kcal";
    }
}

public class WorkoutSession
{
    public int SessionId { get; set; }
    public int PersonId { get; set; }
    public int? PlanId { get; set; }
    public DateTime SessionDate { get; set; }
    public int DurationMinutes { get; set; }
    public double TotalCalories { get; set; }
    public string? Notes { get; set; }
    public List<SessionLog> Logs { get; set; } = new List<SessionLog>();

    public override string ToString()
    {
        return $"[{SessionId}] {SessionDate:yyyy-MM-dd} | {DurationMinutes} min | {TotalCalories} kcal | {Logs.Count} sets";
    }
}
