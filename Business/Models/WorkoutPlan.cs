namespace FitTrack.Models;

public class WorkoutPlanExercise
{
    public int PlanExerciseId { get; set; }
    public int PlanId { get; set; }
    public int ExerciseId { get; set; }
    public string ExerciseName { get; set; } = string.Empty;
    public string MuscleGroup { get; set; } = string.Empty;
    public int DayOfWeek { get; set; }
    public int OrderInDay { get; set; }
    public int Sets { get; set; }
    public int? Reps { get; set; }
    public int? Seconds { get; set; }
    public int RestSeconds { get; set; }

    public override string ToString()
    {
        string volume = Reps.HasValue ? $"{Sets}x{Reps}" : $"{Sets}x{Seconds}s";
        return $"  Day {DayOfWeek} #{OrderInDay}: {ExerciseName} ({MuscleGroup}) {volume} — Rest {RestSeconds}s";
    }
}

public class WorkoutPlan
{
    public int PlanId { get; set; }
    public int CreatedByPersonId { get; set; }
    public int? AssignedToPersonId { get; set; }
    public int GoalId { get; set; }
    public string GoalName { get; set; } = string.Empty;
    public string PlanName { get; set; } = string.Empty;
    public int DurationWeeks { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<WorkoutPlanExercise> Exercises { get; set; } = new List<WorkoutPlanExercise>();

    public override string ToString()
    {
        return $"[{PlanId}] {PlanName} | Goal: {GoalName} | {DurationWeeks} weeks | Active: {IsActive}";
    }
}
