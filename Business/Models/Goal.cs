namespace FitTrack.Models;

public class Goal
{
    public int GoalId { get; set; }
    public string GoalName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int CalorieDelta { get; set; }

    public override string ToString()
    {
        string direction = CalorieDelta >= 0 ? $"+{CalorieDelta}" : $"{CalorieDelta}";
        return $"[{GoalId}] {GoalName} ({direction} kcal from TDEE) — {Description}";
    }
}
