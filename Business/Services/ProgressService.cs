using FitTrack.Database;
using FitTrack.Models;

namespace FitTrack.Services;

public class ProgressService
{
    private ProgressRepository _progressRepo = new ProgressRepository();
    private PersonRepository _personRepo = new PersonRepository();

    public (bool success, string message, ProgressSnapshot? snapshot) LogSnapshot(
        int personId, double weightKg, double? bodyFatPct, string? notes)
    {
        Person? person = _personRepo.GetById(personId);
        if (person == null)
            return (false, "User not found.", null);

        double bmi = person.CalculateBMI();

        ProgressSnapshot snapshot = new ProgressSnapshot
        {
            PersonId     = personId,
            SnapshotDate = DateTime.Now,
            WeightKg     = weightKg,
            BodyFatPct   = bodyFatPct,
            BMI          = bmi,
            Notes        = notes
        };

        int id = _progressRepo.InsertSnapshot(snapshot);
        snapshot.SnapshotId = id;

        person.WeightKg = weightKg;
        if (bodyFatPct.HasValue)
            person.BodyFatPct = bodyFatPct;

        _personRepo.Update(person);

        return (true, "Progress snapshot saved.", snapshot);
    }

    public List<ProgressSnapshot> GetSnapshots(int personId, int take = 10)
    {
        return _progressRepo.GetSnapshots(personId, take);
    }

    public (bool success, string message) DeleteSnapshot(int snapshotId)
    {
        _progressRepo.DeleteSnapshot(snapshotId);
        return (true, "Snapshot deleted.");
    }
}
