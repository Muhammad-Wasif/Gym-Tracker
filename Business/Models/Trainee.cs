namespace FitTrack.Models;

public class Trainee : Person
{
    public string? GoalName { get; set; }
    public string? TrainerName { get; set; }

    public Trainee()
    {
        Role = "Trainee";
    }

    public override string GetRoleLabel()
    {
        return "Trainee";
    }

    public override string ToString()
    {
        return base.ToString() + $" | Goal: {GoalName ?? "None"} | Trainer: {TrainerName ?? "None"}";
    }
}
