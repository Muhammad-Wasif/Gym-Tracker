namespace FitTrack.Models;

public class ProgressSnapshot
{
    public int SnapshotId { get; set; }
    public int PersonId { get; set; }
    public DateTime SnapshotDate { get; set; }
    public double WeightKg { get; set; }
    public double? BodyFatPct { get; set; }
    public double BMI { get; set; }
    public string? Notes { get; set; }

    public override string ToString()
    {
        string bf = BodyFatPct.HasValue ? $"{BodyFatPct:0.0}%" : "N/A";
        return $"[{SnapshotId}] {SnapshotDate:yyyy-MM-dd} | {WeightKg}kg | BMI: {BMI} | BF: {bf}";
    }
}
