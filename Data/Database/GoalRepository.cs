using Microsoft.Data.SqlClient;
using FitTrack.Models;

namespace FitTrack.Database;

public class GoalRepository
{
    public List<Goal> GetAll()
    {
        List<Goal> goals = new List<Goal>();

        using SqlConnection conn = DatabaseHelper.GetConnection();
        conn.Open();

        using SqlCommand cmd = new SqlCommand(
            "SELECT GoalId, GoalName, Description, CalorieDelta FROM Goals ORDER BY GoalId", conn);

        using SqlDataReader reader = cmd.ExecuteReader();
        while (reader.Read())
            goals.Add(MapToGoal(reader));

        return goals;
    }

    public Goal? GetById(int goalId)
    {
        using SqlConnection conn = DatabaseHelper.GetConnection();
        conn.Open();

        using SqlCommand cmd = new SqlCommand(
            "SELECT GoalId, GoalName, Description, CalorieDelta FROM Goals WHERE GoalId = @GoalId", conn);
        cmd.Parameters.AddWithValue("@GoalId", goalId);

        using SqlDataReader reader = cmd.ExecuteReader();
        if (reader.Read())
            return MapToGoal(reader);

        return null;
    }

    public bool Exists(int goalId)
    {
        using SqlConnection conn = DatabaseHelper.GetConnection();
        conn.Open();

        using SqlCommand cmd = new SqlCommand(
            "SELECT COUNT(1) FROM Goals WHERE GoalId = @GoalId", conn);
        cmd.Parameters.AddWithValue("@GoalId", goalId);

        return (int)cmd.ExecuteScalar()! > 0;
    }

    private Goal MapToGoal(SqlDataReader reader)
    {
        return new Goal
        {
            GoalId       = Convert.ToInt32(reader["GoalId"]),
            GoalName     = reader["GoalName"].ToString()!,
            Description  = reader["Description"].ToString()!,
            CalorieDelta = Convert.ToInt32(reader["CalorieDelta"])
        };
    }
}
