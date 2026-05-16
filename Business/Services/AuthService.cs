using FitTrack.Database;
using FitTrack.Models;

namespace FitTrack.Services;

public class AuthService
{
    private PersonRepository _personRepo = new PersonRepository();
    private GoalRepository _goalRepo = new GoalRepository();

    public (bool success, string message, Person? person) Register(
        string fullName, string username, string password, string email,
        string role, string gender, int age, double heightCm, double weightKg,
        double? bodyFatPct, int? goalId)
    {
        if (string.IsNullOrWhiteSpace(username))
            return (false, "Username cannot be empty.", null);

        if (string.IsNullOrWhiteSpace(password) || password.Length < 4)
            return (false, "Password must be at least 4 characters.", null);

        if (age < 10 || age > 120)
            return (false, "Age must be between 10 and 120.", null);

        if (heightCm < 50 || heightCm > 300)
            return (false, "Height must be between 50 and 300 cm.", null);

        if (weightKg < 20 || weightKg > 500)
            return (false, "Weight must be between 20 and 500 kg.", null);

        if (_personRepo.UsernameExists(username))
            return (false, "Username already taken.", null);

        if (_personRepo.EmailExists(email))
            return (false, "Email already registered.", null);

        if (goalId.HasValue && !_goalRepo.Exists(goalId.Value))
            return (false, "Selected goal does not exist.", null);

        Person person = role switch
        {
            "Trainer" => new Trainer(),
            "Admin"   => new Admin(),
            _         => new Trainee()
        };

        person.FullName     = fullName.Trim();
        person.Username     = username.Trim();
        person.PasswordHash = BCrypt.Net.BCrypt.HashPassword(password);
        person.Email        = email.Trim().ToLower();
        person.Gender       = gender;
        person.Age          = age;
        person.HeightCm     = heightCm;
        person.WeightKg     = weightKg;
        person.BodyFatPct   = bodyFatPct;
        person.GoalId       = goalId;
        person.CreatedAt    = DateTime.Now;

        int newId = _personRepo.Insert(person);
        person.PersonId = newId;

        Person? saved = _personRepo.GetById(newId);
        return (true, "Registration successful.", saved);
    }

    public (bool success, string message, Person? person) Login(string username, string password)
    {
        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            return (false, "Username and password are required.", null);

        Person? person = _personRepo.GetByUsername(username);

        if (person == null)
            return (false, "Invalid username or password.", null);

        if (!BCrypt.Net.BCrypt.Verify(password, person.PasswordHash))
            return (false, "Invalid username or password.", null);

        return (true, "Login successful.", person);
    }

    public (bool success, string message) ChangePassword(int personId, string currentPassword, string newPassword)
    {
        Person? person = _personRepo.GetById(personId);
        if (person == null)
            return (false, "User not found.");

        if (!BCrypt.Net.BCrypt.Verify(currentPassword, person.PasswordHash))
            return (false, "Current password is incorrect.");

        if (newPassword.Length < 4)
            return (false, "New password must be at least 4 characters.");

        _personRepo.UpdatePassword(personId, BCrypt.Net.BCrypt.HashPassword(newPassword));
        return (true, "Password changed successfully.");
    }
}
