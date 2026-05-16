namespace FitTrack.Models;

public class ExerciseCategory
{
    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;

    public override string ToString()
    {
        return $"[{CategoryId}] {CategoryName}";
    }
}
