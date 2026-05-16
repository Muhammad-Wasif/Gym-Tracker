using FitTrack.Database;
using FitTrack.Models;

namespace FitTrack.Services;

public class UserService
{
    private PersonRepository _personRepo = new PersonRepository();
    private GoalRepository _goalRepo = new GoalRepository();

    public Person? GetById(int personId)
    {
        return _personRepo.GetById(personId);
    }

    public List<Person> GetAllTrainees()
    {
        return _personRepo.GetAllByRole("Trainee");
    }

    public List<Person> GetAllTrainers()
    {
        return _personRepo.GetAllByRole("Trainer");
    }

    public List<Person> GetAll()
    {
        return _personRepo.GetAll();
    }

    public List<Person> GetTraineesByTrainer(int trainerId)
    {
        return _personRepo.GetTraineesByTrainer(trainerId);
    }

    public (bool success, string message) UpdateProfile(
        int personId, string fullName, string email,
        string gender, int age, double heightCm, double weightKg,
        double? bodyFatPct, int? goalId)
    {
        Person? person = _personRepo.GetById(personId);
        if (person == null)
            return (false, "User not found.");

        if (goalId.HasValue && !_goalRepo.Exists(goalId.Value))
            return (false, "Selected goal does not exist.");

        person.FullName   = fullName.Trim();
        person.Email      = email.Trim().ToLower();
        person.Gender     = gender;
        person.Age        = age;
        person.HeightCm   = heightCm;
        person.WeightKg   = weightKg;
        person.BodyFatPct = bodyFatPct;
        person.GoalId     = goalId;

        _personRepo.Update(person);
        return (true, "Profile updated.");
    }

    public (bool success, string message) AssignTrainer(int traineeId, int trainerId)
    {
        Person? trainee = _personRepo.GetById(traineeId);
        if (trainee == null)
            return (false, "Trainee not found.");

        Person? trainer = _personRepo.GetById(trainerId);
        if (trainer == null || trainer.Role != "Trainer")
            return (false, "Trainer not found or user is not a Trainer.");

        _personRepo.UpdateTrainer(traineeId, trainerId);
        return (true, "Trainer assigned successfully.");
    }

    public (bool success, string message) SetRole(int personId, string role)
    {
        string[] allowed = { "Trainee", "Trainer", "Admin" };
        if (!allowed.Contains(role))
            return (false, "Role must be Trainee, Trainer, or Admin.");

        Person? person = _personRepo.GetById(personId);
        if (person == null)
            return (false, "User not found.");

        _personRepo.UpdateRole(personId, role);
        return (true, $"Role updated to {role}.");
    }

    public (bool success, string message) DeleteUser(int personId)
    {
        Person? person = _personRepo.GetById(personId);
        if (person == null)
            return (false, "User not found.");

        _personRepo.Delete(personId);
        return (true, "User deleted.");
    }
}
