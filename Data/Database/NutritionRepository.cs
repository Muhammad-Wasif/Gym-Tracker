using Microsoft.Data.SqlClient;
using FitTrack.Models;

namespace FitTrack.Database;

public class NutritionRepository
{
    public List<FoodItem> GetAllFoodItems()
    {
        List<FoodItem> list = new List<FoodItem>();

        using SqlConnection conn = DatabaseHelper.GetConnection();
        conn.Open();

        string sql = @"
            SELECT f.FoodItemId, f.FoodCategoryId, f.GoalId, f.FoodName,
                   f.CaloriesPer100g, f.ProteinPer100g, f.CarbsPer100g,
                   f.FatPer100g, f.FiberPer100g, c.CategoryName
            FROM FoodItems f
            JOIN FoodCategories c ON f.FoodCategoryId = c.FoodCategoryId
            ORDER BY c.CategoryName, f.FoodName";

        using SqlCommand cmd = new SqlCommand(sql, conn);
        using SqlDataReader reader = cmd.ExecuteReader();

        while (reader.Read())
            list.Add(MapToFoodItem(reader));

        return list;
    }

    public List<FoodItem> SearchFoodItems(string query)
    {
        List<FoodItem> list = new List<FoodItem>();

        using SqlConnection conn = DatabaseHelper.GetConnection();
        conn.Open();

        string sql = @"
            SELECT f.FoodItemId, f.FoodCategoryId, f.GoalId, f.FoodName,
                   f.CaloriesPer100g, f.ProteinPer100g, f.CarbsPer100g,
                   f.FatPer100g, f.FiberPer100g, c.CategoryName
            FROM FoodItems f
            JOIN FoodCategories c ON f.FoodCategoryId = c.FoodCategoryId
            WHERE f.FoodName LIKE @Query
            ORDER BY f.FoodName";

        using SqlCommand cmd = new SqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@Query", $"%{query}%");

        using SqlDataReader reader = cmd.ExecuteReader();
        while (reader.Read())
            list.Add(MapToFoodItem(reader));

        return list;
    }

    public FoodItem? GetFoodItemById(int foodItemId)
    {
        using SqlConnection conn = DatabaseHelper.GetConnection();
        conn.Open();

        string sql = @"
            SELECT f.FoodItemId, f.FoodCategoryId, f.GoalId, f.FoodName,
                   f.CaloriesPer100g, f.ProteinPer100g, f.CarbsPer100g,
                   f.FatPer100g, f.FiberPer100g, c.CategoryName
            FROM FoodItems f
            JOIN FoodCategories c ON f.FoodCategoryId = c.FoodCategoryId
            WHERE f.FoodItemId = @FoodItemId";

        using SqlCommand cmd = new SqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@FoodItemId", foodItemId);

        using SqlDataReader reader = cmd.ExecuteReader();
        if (reader.Read())
            return MapToFoodItem(reader);

        return null;
    }

    public int InsertNutritionLog(NutritionLog log)
    {
        using SqlConnection conn = DatabaseHelper.GetConnection();
        conn.Open();

        string sql = @"
            INSERT INTO NutritionLogs (PersonId, FoodItemId, MealType, ServingGrams, Calories, ProteinG, CarbsG, FatG, LoggedAt)
            VALUES (@PersonId, @FoodItemId, @MealType, @ServingGrams, @Calories, @ProteinG, @CarbsG, @FatG, @LoggedAt);
            SELECT SCOPE_IDENTITY();";

        using SqlCommand cmd = new SqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@PersonId", log.PersonId);
        cmd.Parameters.AddWithValue("@FoodItemId", log.FoodItemId);
        cmd.Parameters.AddWithValue("@MealType", log.MealType);
        cmd.Parameters.AddWithValue("@ServingGrams", log.ServingGrams);
        cmd.Parameters.AddWithValue("@Calories", log.Calories);
        cmd.Parameters.AddWithValue("@ProteinG", log.ProteinG);
        cmd.Parameters.AddWithValue("@CarbsG", log.CarbsG);
        cmd.Parameters.AddWithValue("@FatG", log.FatG);
        cmd.Parameters.AddWithValue("@LoggedAt", log.LoggedAt);

        return Convert.ToInt32(cmd.ExecuteScalar());
    }

    public List<NutritionLog> GetLogsForDay(int personId, DateTime date)
    {
        List<NutritionLog> list = new List<NutritionLog>();

        using SqlConnection conn = DatabaseHelper.GetConnection();
        conn.Open();

        string sql = @"
            SELECT nl.NutritionLogId, nl.PersonId, nl.FoodItemId, nl.MealType,
                   nl.ServingGrams, nl.Calories, nl.ProteinG, nl.CarbsG, nl.FatG,
                   nl.LoggedAt, f.FoodName
            FROM NutritionLogs nl
            JOIN FoodItems f ON nl.FoodItemId = f.FoodItemId
            WHERE nl.PersonId = @PersonId
              AND CAST(nl.LoggedAt AS DATE) = @Date
            ORDER BY nl.LoggedAt";

        using SqlCommand cmd = new SqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@PersonId", personId);
        cmd.Parameters.AddWithValue("@Date", date.Date);

        using SqlDataReader reader = cmd.ExecuteReader();
        while (reader.Read())
            list.Add(MapToLog(reader));

        return list;
    }

    public List<NutritionLog> GetRecentLogs(int personId, int days)
    {
        List<NutritionLog> list = new List<NutritionLog>();

        using SqlConnection conn = DatabaseHelper.GetConnection();
        conn.Open();

        string sql = @"
            SELECT nl.NutritionLogId, nl.PersonId, nl.FoodItemId, nl.MealType,
                   nl.ServingGrams, nl.Calories, nl.ProteinG, nl.CarbsG, nl.FatG,
                   nl.LoggedAt, f.FoodName
            FROM NutritionLogs nl
            JOIN FoodItems f ON nl.FoodItemId = f.FoodItemId
            WHERE nl.PersonId = @PersonId
              AND nl.LoggedAt >= DATEADD(DAY, @Days, GETDATE())
            ORDER BY nl.LoggedAt DESC";

        using SqlCommand cmd = new SqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@PersonId", personId);
        cmd.Parameters.AddWithValue("@Days", -days);

        using SqlDataReader reader = cmd.ExecuteReader();
        while (reader.Read())
            list.Add(MapToLog(reader));

        return list;
    }

    public void DeleteLog(int nutritionLogId)
    {
        using SqlConnection conn = DatabaseHelper.GetConnection();
        conn.Open();

        using SqlCommand cmd = new SqlCommand(
            "DELETE FROM NutritionLogs WHERE NutritionLogId = @Id", conn);
        cmd.Parameters.AddWithValue("@Id", nutritionLogId);
        cmd.ExecuteNonQuery();
    }

    private FoodItem MapToFoodItem(SqlDataReader reader)
    {
        return new FoodItem
        {
            FoodItemId      = Convert.ToInt32(reader["FoodItemId"]),
            FoodCategoryId  = Convert.ToInt32(reader["FoodCategoryId"]),
            GoalId          = reader["GoalId"] == DBNull.Value ? null : Convert.ToInt32(reader["GoalId"]),
            CategoryName    = reader["CategoryName"].ToString()!,
            FoodName        = reader["FoodName"].ToString()!,
            CaloriesPer100g = Convert.ToDouble(reader["CaloriesPer100g"]),
            ProteinPer100g  = Convert.ToDouble(reader["ProteinPer100g"]),
            CarbsPer100g    = Convert.ToDouble(reader["CarbsPer100g"]),
            FatPer100g      = Convert.ToDouble(reader["FatPer100g"]),
            FiberPer100g    = reader["FiberPer100g"] == DBNull.Value ? null : Convert.ToDouble(reader["FiberPer100g"])
        };
    }

    private NutritionLog MapToLog(SqlDataReader reader)
    {
        return new NutritionLog
        {
            NutritionLogId = Convert.ToInt32(reader["NutritionLogId"]),
            PersonId       = Convert.ToInt32(reader["PersonId"]),
            FoodItemId     = Convert.ToInt32(reader["FoodItemId"]),
            FoodName       = reader["FoodName"].ToString()!,
            MealType       = reader["MealType"].ToString()!,
            ServingGrams   = Convert.ToDouble(reader["ServingGrams"]),
            Calories       = Convert.ToDouble(reader["Calories"]),
            ProteinG       = Convert.ToDouble(reader["ProteinG"]),
            CarbsG         = Convert.ToDouble(reader["CarbsG"]),
            FatG           = Convert.ToDouble(reader["FatG"]),
            LoggedAt       = Convert.ToDateTime(reader["LoggedAt"])
        };
    }
}
