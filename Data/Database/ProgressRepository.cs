using Microsoft.Data.SqlClient;
using FitTrack.Models;

namespace FitTrack.Database;

public class ProgressRepository
{
    public int InsertSnapshot(ProgressSnapshot snapshot)
    {
        using SqlConnection conn = DatabaseHelper.GetConnection();
        conn.Open();

        string sql = @"
            INSERT INTO ProgressSnapshots (PersonId, SnapshotDate, WeightKg, BodyFatPct, BMI, Notes)
            VALUES (@PersonId, @SnapshotDate, @WeightKg, @BodyFatPct, @BMI, @Notes);
            SELECT SCOPE_IDENTITY();";

        using SqlCommand cmd = new SqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@PersonId", snapshot.PersonId);
        cmd.Parameters.AddWithValue("@SnapshotDate", snapshot.SnapshotDate);
        cmd.Parameters.AddWithValue("@WeightKg", snapshot.WeightKg);
        cmd.Parameters.AddWithValue("@BodyFatPct", (object?)snapshot.BodyFatPct ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@BMI", snapshot.BMI);
        cmd.Parameters.AddWithValue("@Notes", (object?)snapshot.Notes ?? DBNull.Value);

        return Convert.ToInt32(cmd.ExecuteScalar());
    }

    public List<ProgressSnapshot> GetSnapshots(int personId, int take)
    {
        List<ProgressSnapshot> list = new List<ProgressSnapshot>();

        using SqlConnection conn = DatabaseHelper.GetConnection();
        conn.Open();

        string sql = $@"
            SELECT TOP {take} SnapshotId, PersonId, SnapshotDate, WeightKg, BodyFatPct, BMI, Notes
            FROM ProgressSnapshots
            WHERE PersonId = @PersonId
            ORDER BY SnapshotDate DESC";

        using SqlCommand cmd = new SqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@PersonId", personId);

        using SqlDataReader reader = cmd.ExecuteReader();
        while (reader.Read())
            list.Add(MapToSnapshot(reader));

        return list;
    }

    public void DeleteSnapshot(int snapshotId)
    {
        using SqlConnection conn = DatabaseHelper.GetConnection();
        conn.Open();

        using SqlCommand cmd = new SqlCommand(
            "DELETE FROM ProgressSnapshots WHERE SnapshotId = @SnapshotId", conn);
        cmd.Parameters.AddWithValue("@SnapshotId", snapshotId);
        cmd.ExecuteNonQuery();
    }

    private ProgressSnapshot MapToSnapshot(SqlDataReader reader)
    {
        return new ProgressSnapshot
        {
            SnapshotId   = Convert.ToInt32(reader["SnapshotId"]),
            PersonId     = Convert.ToInt32(reader["PersonId"]),
            SnapshotDate = Convert.ToDateTime(reader["SnapshotDate"]),
            WeightKg     = Convert.ToDouble(reader["WeightKg"]),
            BodyFatPct   = reader["BodyFatPct"] == DBNull.Value ? null : Convert.ToDouble(reader["BodyFatPct"]),
            BMI          = Convert.ToDouble(reader["BMI"]),
            Notes        = reader["Notes"] == DBNull.Value ? null : reader["Notes"].ToString()
        };
    }
}
