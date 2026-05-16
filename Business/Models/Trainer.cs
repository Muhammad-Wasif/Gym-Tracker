namespace FitTrack.Models;

public class Trainer : Person
{
    public List<Trainee> Trainees { get; set; } = new List<Trainee>();

    public Trainer()
    {
        Role = "Trainer";
    }

    public override string GetRoleLabel()
    {
        return "Trainer";
    }

    public void AddTrainee(Trainee trainee)
    {
        Trainees.Add(trainee);
    }

    public override string ToString()
    {
        return base.ToString() + $" | Clients: {Trainees.Count}";
    }
}
