using Microsoft.Data.SqlClient;

namespace FitTrack.Database;

public class DatabaseHelper
{
    private static string _connectionString = string.Empty;

    public static void Initialise(string connectionString)
    {
        _connectionString = connectionString;
    }

    public static SqlConnection GetConnection()
    {
        return new SqlConnection(_connectionString);
    }

    public static void RunSql(string sql)
    {
        using SqlConnection conn = GetConnection();
        conn.Open();
        using SqlCommand cmd = new SqlCommand(sql, conn);
        cmd.ExecuteNonQuery();
    }
}
