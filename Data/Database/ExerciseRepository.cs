using Microsoft.Data.SqlClient;
using FitTrack.Models;

namespace FitTrack.Database;

public class ExerciseRepository
{
    public List<Exercise> GetAll()
    {
        List<Exercise> list = new List<Exercise>();

        using SqlConnection conn = DatabaseHelper.GetConnection();
        conn.Open();

        string sql = @"
            SELECT e.ExerciseId, e.CategoryId, e.GoalId, e.Name, e.MuscleGroup,
                   e.Equipment, e.DefaultSets, e.DefaultReps, e.DefaultSecs,
                   e.METValue, e.Description, c.CategoryName
            FROM Exercises e
            JOIN ExerciseCategories c ON e.CategoryId = c.CategoryId
            ORDER BY c.CategoryName, e.Name";

        using SqlCommand cmd = new SqlCommand(sql, conn);
        using SqlDataReader reader = cmd.ExecuteReader();

        while (reader.Read())
            list.Add(MapToExercise(reader));

        return list;
    }

    public List<Exercise> GetByCategory(int categoryId)
    {
        List<Exercise> list = new List<Exercise>();

        using SqlConnection conn = DatabaseHelper.GetConnection();
        conn.Open();

        string sql = @"
            SELECT e.ExerciseId, e.CategoryId, e.GoalId, e.Name, e.MuscleGroup,
                   e.Equipment, e.DefaultSets, e.DefaultReps, e.DefaultSecs,
                   e.METValue, e.Description, c.CategoryName
            FROM Exercises e
            JOIN ExerciseCategories c ON e.CategoryId = c.CategoryId
            WHERE e.CategoryId = @CategoryId
            ORDER BY e.Name";

        using SqlCommand cmd = new SqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@CategoryId", categoryId);

        using SqlDataReader reader = cmd.ExecuteReader();
        while (reader.Read())
            list.Add(MapToExercise(reader));

        return list;
    }

    public Exercise? GetById(int exerciseId)
    {
        using SqlConnection conn = DatabaseHelper.GetConnection();
        conn.Open();

        string sql = @"
            SELECT e.ExerciseId, e.CategoryId, e.GoalId, e.Name, e.MuscleGroup,
                   e.Equipment, e.DefaultSets, e.DefaultReps, e.DefaultSecs,
                   e.METValue, e.Description, c.CategoryName
            FROM Exercises e
            JOIN ExerciseCategories c ON e.CategoryId = c.CategoryId
            WHERE e.ExerciseId = @ExerciseId";

        using SqlCommand cmd = new SqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@ExerciseId", exerciseId);

        using SqlDataReader reader = cmd.ExecuteReader();
        if (reader.Read())
            return MapToExercise(reader);

        return null;
    }

    public List<ExerciseCategory> GetAllCategories()
    {
        List<ExerciseCategory> list = new List<ExerciseCategory>();

        using SqlConnection conn = DatabaseHelper.GetConnection();
        conn.Open();

        using SqlCommand cmd = new SqlCommand(
            "SELECT CategoryId, CategoryName FROM ExerciseCategories ORDER BY CategoryName", conn);

        using SqlDataReader reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            list.Add(new ExerciseCategory
            {
                CategoryId   = Convert.ToInt32(reader["CategoryId"]),
                CategoryName = reader["CategoryName"].ToString()!
            });
        }

        return list;
    }

    private Exercise MapToExercise(SqlDataReader reader)
    {
        return new Exercise
        {
            ExerciseId   = Convert.ToInt32(reader["ExerciseId"]),
            CategoryId   = Convert.ToInt32(reader["CategoryId"]),
            GoalId       = reader["GoalId"] == DBNull.Value ? null : Convert.ToInt32(reader["GoalId"]),
            CategoryName = reader["CategoryName"].ToString()!,
            Name         = reader["Name"].ToString()!,
            MuscleGroup  = reader["MuscleGroup"].ToString()!,
            Equipment    = reader["Equipment"].ToString()!,
            DefaultSets  = Convert.ToInt32(reader["DefaultSets"]),
            DefaultReps  = reader["DefaultReps"] == DBNull.Value ? null : Convert.ToInt32(reader["DefaultReps"]),
            DefaultSecs  = reader["DefaultSecs"] == DBNull.Value ? null : Convert.ToInt32(reader["DefaultSecs"]),
            METValue     = Convert.ToDouble(reader["METValue"]),
            Description  = reader["Description"] == DBNull.Value ? null : reader["Description"].ToString()
        };
    }
}
