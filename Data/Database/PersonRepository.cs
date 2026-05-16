using Microsoft.Data.SqlClient;
using FitTrack.Models;

namespace FitTrack.Database;

public class PersonRepository
{
    public Person? GetById(int personId)
    {
        using SqlConnection conn = DatabaseHelper.GetConnection();
        conn.Open();

        string sql = @"
            SELECT p.PersonId, p.FullName, p.Username, p.PasswordHash, p.Email,
                   p.Role, p.Gender, p.Age, p.HeightCm, p.WeightKg,
                   p.BodyFatPct, p.GoalId, p.TrainerId, p.CreatedAt,
                   g.GoalName, t.FullName AS TrainerName
            FROM Persons p
            LEFT JOIN Goals g ON p.GoalId = g.GoalId
            LEFT JOIN Persons t ON p.TrainerId = t.PersonId
            WHERE p.PersonId = @PersonId";

        using SqlCommand cmd = new SqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@PersonId", personId);

        using SqlDataReader reader = cmd.ExecuteReader();
        if (reader.Read())
            return MapToPerson(reader);

        return null;
    }

    public Person? GetByUsername(string username)
    {
        using SqlConnection conn = DatabaseHelper.GetConnection();
        conn.Open();

        string sql = @"
            SELECT p.PersonId, p.FullName, p.Username, p.PasswordHash, p.Email,
                   p.Role, p.Gender, p.Age, p.HeightCm, p.WeightKg,
                   p.BodyFatPct, p.GoalId, p.TrainerId, p.CreatedAt,
                   g.GoalName, t.FullName AS TrainerName
            FROM Persons p
            LEFT JOIN Goals g ON p.GoalId = g.GoalId
            LEFT JOIN Persons t ON p.TrainerId = t.PersonId
            WHERE p.Username = @Username";

        using SqlCommand cmd = new SqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@Username", username);

        using SqlDataReader reader = cmd.ExecuteReader();
        if (reader.Read())
            return MapToPerson(reader);

        return null;
    }

    public List<Person> GetAll()
    {
        List<Person> people = new List<Person>();

        using SqlConnection conn = DatabaseHelper.GetConnection();
        conn.Open();

        string sql = @"
            SELECT p.PersonId, p.FullName, p.Username, p.PasswordHash, p.Email,
                   p.Role, p.Gender, p.Age, p.HeightCm, p.WeightKg,
                   p.BodyFatPct, p.GoalId, p.TrainerId, p.CreatedAt,
                   g.GoalName, t.FullName AS TrainerName
            FROM Persons p
            LEFT JOIN Goals g ON p.GoalId = g.GoalId
            LEFT JOIN Persons t ON p.TrainerId = t.PersonId
            ORDER BY p.Role, p.FullName";

        using SqlCommand cmd = new SqlCommand(sql, conn);
        using SqlDataReader reader = cmd.ExecuteReader();

        while (reader.Read())
            people.Add(MapToPerson(reader));

        return people;
    }

    public List<Person> GetAllByRole(string role)
    {
        List<Person> people = new List<Person>();

        using SqlConnection conn = DatabaseHelper.GetConnection();
        conn.Open();

        string sql = @"
            SELECT p.PersonId, p.FullName, p.Username, p.PasswordHash, p.Email,
                   p.Role, p.Gender, p.Age, p.HeightCm, p.WeightKg,
                   p.BodyFatPct, p.GoalId, p.TrainerId, p.CreatedAt,
                   g.GoalName, t.FullName AS TrainerName
            FROM Persons p
            LEFT JOIN Goals g ON p.GoalId = g.GoalId
            LEFT JOIN Persons t ON p.TrainerId = t.PersonId
            WHERE p.Role = @Role
            ORDER BY p.FullName";

        using SqlCommand cmd = new SqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@Role", role);

        using SqlDataReader reader = cmd.ExecuteReader();
        while (reader.Read())
            people.Add(MapToPerson(reader));

        return people;
    }

    public List<Person> GetTraineesByTrainer(int trainerId)
    {
        List<Person> people = new List<Person>();

        using SqlConnection conn = DatabaseHelper.GetConnection();
        conn.Open();

        string sql = @"
            SELECT p.PersonId, p.FullName, p.Username, p.PasswordHash, p.Email,
                   p.Role, p.Gender, p.Age, p.HeightCm, p.WeightKg,
                   p.BodyFatPct, p.GoalId, p.TrainerId, p.CreatedAt,
                   g.GoalName, t.FullName AS TrainerName
            FROM Persons p
            LEFT JOIN Goals g ON p.GoalId = g.GoalId
            LEFT JOIN Persons t ON p.TrainerId = t.PersonId
            WHERE p.TrainerId = @TrainerId
            ORDER BY p.FullName";

        using SqlCommand cmd = new SqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@TrainerId", trainerId);

        using SqlDataReader reader = cmd.ExecuteReader();
        while (reader.Read())
            people.Add(MapToPerson(reader));

        return people;
    }

    public bool UsernameExists(string username)
    {
        using SqlConnection conn = DatabaseHelper.GetConnection();
        conn.Open();

        using SqlCommand cmd = new SqlCommand(
            "SELECT COUNT(1) FROM Persons WHERE Username = @Username", conn);
        cmd.Parameters.AddWithValue("@Username", username);

        return (int)cmd.ExecuteScalar()! > 0;
    }

    public bool EmailExists(string email)
    {
        using SqlConnection conn = DatabaseHelper.GetConnection();
        conn.Open();

        using SqlCommand cmd = new SqlCommand(
            "SELECT COUNT(1) FROM Persons WHERE Email = @Email", conn);
        cmd.Parameters.AddWithValue("@Email", email.ToLower());

        return (int)cmd.ExecuteScalar()! > 0;
    }

    public int Insert(Person person)
    {
        using SqlConnection conn = DatabaseHelper.GetConnection();
        conn.Open();

        string sql = @"
            INSERT INTO Persons (FullName, Username, PasswordHash, Email, Role, Gender,
                                 Age, HeightCm, WeightKg, BodyFatPct, GoalId, TrainerId, CreatedAt)
            VALUES (@FullName, @Username, @PasswordHash, @Email, @Role, @Gender,
                    @Age, @HeightCm, @WeightKg, @BodyFatPct, @GoalId, @TrainerId, @CreatedAt);
            SELECT SCOPE_IDENTITY();";

        using SqlCommand cmd = new SqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@FullName", person.FullName);
        cmd.Parameters.AddWithValue("@Username", person.Username);
        cmd.Parameters.AddWithValue("@PasswordHash", person.PasswordHash);
        cmd.Parameters.AddWithValue("@Email", person.Email.ToLower());
        cmd.Parameters.AddWithValue("@Role", person.Role);
        cmd.Parameters.AddWithValue("@Gender", person.Gender);
        cmd.Parameters.AddWithValue("@Age", person.Age);
        cmd.Parameters.AddWithValue("@HeightCm", person.HeightCm);
        cmd.Parameters.AddWithValue("@WeightKg", person.WeightKg);
        cmd.Parameters.AddWithValue("@BodyFatPct", (object?)person.BodyFatPct ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@GoalId", (object?)person.GoalId ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@TrainerId", (object?)person.TrainerId ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@CreatedAt", DateTime.Now);

        return Convert.ToInt32(cmd.ExecuteScalar());
    }

    public void Update(Person person)
    {
        using SqlConnection conn = DatabaseHelper.GetConnection();
        conn.Open();

        string sql = @"
            UPDATE Persons SET
                FullName     = @FullName,
                Email        = @Email,
                Gender       = @Gender,
                Age          = @Age,
                HeightCm     = @HeightCm,
                WeightKg     = @WeightKg,
                BodyFatPct   = @BodyFatPct,
                GoalId       = @GoalId
            WHERE PersonId = @PersonId";

        using SqlCommand cmd = new SqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@FullName", person.FullName);
        cmd.Parameters.AddWithValue("@Email", person.Email.ToLower());
        cmd.Parameters.AddWithValue("@Gender", person.Gender);
        cmd.Parameters.AddWithValue("@Age", person.Age);
        cmd.Parameters.AddWithValue("@HeightCm", person.HeightCm);
        cmd.Parameters.AddWithValue("@WeightKg", person.WeightKg);
        cmd.Parameters.AddWithValue("@BodyFatPct", (object?)person.BodyFatPct ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@GoalId", (object?)person.GoalId ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@PersonId", person.PersonId);

        cmd.ExecuteNonQuery();
    }

    public void UpdatePassword(int personId, string newHash)
    {
        using SqlConnection conn = DatabaseHelper.GetConnection();
        conn.Open();

        using SqlCommand cmd = new SqlCommand(
            "UPDATE Persons SET PasswordHash = @Hash WHERE PersonId = @PersonId", conn);
        cmd.Parameters.AddWithValue("@Hash", newHash);
        cmd.Parameters.AddWithValue("@PersonId", personId);
        cmd.ExecuteNonQuery();
    }

    public void UpdateRole(int personId, string role)
    {
        using SqlConnection conn = DatabaseHelper.GetConnection();
        conn.Open();

        using SqlCommand cmd = new SqlCommand(
            "UPDATE Persons SET Role = @Role WHERE PersonId = @PersonId", conn);
        cmd.Parameters.AddWithValue("@Role", role);
        cmd.Parameters.AddWithValue("@PersonId", personId);
        cmd.ExecuteNonQuery();
    }

    public void UpdateTrainer(int traineeId, int trainerId)
    {
        using SqlConnection conn = DatabaseHelper.GetConnection();
        conn.Open();

        using SqlCommand cmd = new SqlCommand(
            "UPDATE Persons SET TrainerId = @TrainerId WHERE PersonId = @TraineeId", conn);
        cmd.Parameters.AddWithValue("@TrainerId", trainerId);
        cmd.Parameters.AddWithValue("@TraineeId", traineeId);
        cmd.ExecuteNonQuery();
    }

    public void Delete(int personId)
    {
        using SqlConnection conn = DatabaseHelper.GetConnection();
        conn.Open();

        using SqlCommand cmd = new SqlCommand(
            "DELETE FROM Persons WHERE PersonId = @PersonId", conn);
        cmd.Parameters.AddWithValue("@PersonId", personId);
        cmd.ExecuteNonQuery();
    }

    private Person MapToPerson(SqlDataReader reader)
    {
        string role = reader["Role"].ToString()!;

        Person person = role switch
        {
            "Trainer" => new Trainer(),
            "Admin"   => new Admin(),
            _         => new Trainee()
        };

        person.PersonId     = Convert.ToInt32(reader["PersonId"]);
        person.FullName     = reader["FullName"].ToString()!;
        person.Username     = reader["Username"].ToString()!;
        person.PasswordHash = reader["PasswordHash"].ToString()!;
        person.Email        = reader["Email"].ToString()!;
        person.Role         = role;
        person.Gender       = reader["Gender"].ToString()!;
        person.Age          = Convert.ToInt32(reader["Age"]);
        person.HeightCm     = Convert.ToDouble(reader["HeightCm"]);
        person.WeightKg     = Convert.ToDouble(reader["WeightKg"]);
        person.BodyFatPct   = reader["BodyFatPct"] == DBNull.Value ? null : Convert.ToDouble(reader["BodyFatPct"]);
        person.GoalId       = reader["GoalId"] == DBNull.Value ? null : Convert.ToInt32(reader["GoalId"]);
        person.TrainerId    = reader["TrainerId"] == DBNull.Value ? null : Convert.ToInt32(reader["TrainerId"]);
        person.CreatedAt    = Convert.ToDateTime(reader["CreatedAt"]);

        if (person is Trainee trainee)
        {
            trainee.GoalName    = reader["GoalName"] == DBNull.Value ? null : reader["GoalName"].ToString();
            trainee.TrainerName = reader["TrainerName"] == DBNull.Value ? null : reader["TrainerName"].ToString();
        }

        return person;
    }
}
