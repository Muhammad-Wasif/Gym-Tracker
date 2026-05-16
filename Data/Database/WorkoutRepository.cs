using Microsoft.Data.SqlClient;
using FitTrack.Models;

namespace FitTrack.Database;

public class WorkoutRepository
{
    public int InsertPlan(WorkoutPlan plan)
    {
        using SqlConnection conn = DatabaseHelper.GetConnection();
        conn.Open();

        string sql = @"
            INSERT INTO WorkoutPlans (CreatedByPersonId, AssignedToPersonId, GoalId, PlanName, DurationWeeks, IsActive, CreatedAt)
            VALUES (@CreatedBy, @AssignedTo, @GoalId, @PlanName, @DurationWeeks, 1, @CreatedAt);
            SELECT SCOPE_IDENTITY();";

        using SqlCommand cmd = new SqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@CreatedBy", plan.CreatedByPersonId);
        cmd.Parameters.AddWithValue("@AssignedTo", (object?)plan.AssignedToPersonId ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@GoalId", plan.GoalId);
        cmd.Parameters.AddWithValue("@PlanName", plan.PlanName);
        cmd.Parameters.AddWithValue("@DurationWeeks", plan.DurationWeeks);
        cmd.Parameters.AddWithValue("@CreatedAt", DateTime.Now);

        return Convert.ToInt32(cmd.ExecuteScalar());
    }

    public void InsertPlanExercise(WorkoutPlanExercise pe)
    {
        using SqlConnection conn = DatabaseHelper.GetConnection();
        conn.Open();

        string sql = @"
            INSERT INTO WorkoutPlanExercises (PlanId, ExerciseId, DayOfWeek, OrderInDay, Sets, Reps, Seconds, RestSeconds)
            VALUES (@PlanId, @ExerciseId, @DayOfWeek, @OrderInDay, @Sets, @Reps, @Seconds, @RestSeconds)";

        using SqlCommand cmd = new SqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@PlanId", pe.PlanId);
        cmd.Parameters.AddWithValue("@ExerciseId", pe.ExerciseId);
        cmd.Parameters.AddWithValue("@DayOfWeek", pe.DayOfWeek);
        cmd.Parameters.AddWithValue("@OrderInDay", pe.OrderInDay);
        cmd.Parameters.AddWithValue("@Sets", pe.Sets);
        cmd.Parameters.AddWithValue("@Reps", (object?)pe.Reps ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@Seconds", (object?)pe.Seconds ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@RestSeconds", pe.RestSeconds);

        cmd.ExecuteNonQuery();
    }

    public WorkoutPlan? GetActivePlan(int personId)
    {
        using SqlConnection conn = DatabaseHelper.GetConnection();
        conn.Open();

        string sql = @"
            SELECT wp.PlanId, wp.CreatedByPersonId, wp.AssignedToPersonId, wp.GoalId,
                   wp.PlanName, wp.DurationWeeks, wp.IsActive, wp.CreatedAt, g.GoalName
            FROM WorkoutPlans wp
            JOIN Goals g ON wp.GoalId = g.GoalId
            WHERE wp.AssignedToPersonId = @PersonId AND wp.IsActive = 1";

        using SqlCommand cmd = new SqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@PersonId", personId);

        using SqlDataReader reader = cmd.ExecuteReader();
        if (!reader.Read()) return null;

        WorkoutPlan plan = MapToPlan(reader);
        reader.Close();

        plan.Exercises = GetPlanExercises(plan.PlanId);
        return plan;
    }

    public List<WorkoutPlan> GetAllPlansForPerson(int personId)
    {
        List<WorkoutPlan> plans = new List<WorkoutPlan>();

        using SqlConnection conn = DatabaseHelper.GetConnection();
        conn.Open();

        string sql = @"
            SELECT wp.PlanId, wp.CreatedByPersonId, wp.AssignedToPersonId, wp.GoalId,
                   wp.PlanName, wp.DurationWeeks, wp.IsActive, wp.CreatedAt, g.GoalName
            FROM WorkoutPlans wp
            JOIN Goals g ON wp.GoalId = g.GoalId
            WHERE wp.AssignedToPersonId = @PersonId
            ORDER BY wp.CreatedAt DESC";

        using SqlCommand cmd = new SqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@PersonId", personId);

        using SqlDataReader reader = cmd.ExecuteReader();
        while (reader.Read())
            plans.Add(MapToPlan(reader));

        return plans;
    }

    public List<WorkoutPlan> GetAllPlans()
    {
        List<WorkoutPlan> plans = new List<WorkoutPlan>();

        using SqlConnection conn = DatabaseHelper.GetConnection();
        conn.Open();

        string sql = @"
            SELECT wp.PlanId, wp.CreatedByPersonId, wp.AssignedToPersonId, wp.GoalId,
                   wp.PlanName, wp.DurationWeeks, wp.IsActive, wp.CreatedAt, g.GoalName
            FROM WorkoutPlans wp
            JOIN Goals g ON wp.GoalId = g.GoalId
            ORDER BY wp.CreatedAt DESC";

        using SqlCommand cmd = new SqlCommand(sql, conn);
        using SqlDataReader reader = cmd.ExecuteReader();
        while (reader.Read())
            plans.Add(MapToPlan(reader));

        return plans;
    }

    public void DeactivateAllPlansForPerson(int personId)
    {
        using SqlConnection conn = DatabaseHelper.GetConnection();
        conn.Open();

        using SqlCommand cmd = new SqlCommand(
            "UPDATE WorkoutPlans SET IsActive = 0 WHERE AssignedToPersonId = @PersonId", conn);
        cmd.Parameters.AddWithValue("@PersonId", personId);
        cmd.ExecuteNonQuery();
    }

    public void DeletePlan(int planId)
    {
        using SqlConnection conn = DatabaseHelper.GetConnection();
        conn.Open();

        using SqlCommand cmd = new SqlCommand(
            "DELETE FROM WorkoutPlans WHERE PlanId = @PlanId", conn);
        cmd.Parameters.AddWithValue("@PlanId", planId);
        cmd.ExecuteNonQuery();
    }

    public int InsertSession(WorkoutSession session)
    {
        using SqlConnection conn = DatabaseHelper.GetConnection();
        conn.Open();

        string sql = @"
            INSERT INTO WorkoutSessions (PersonId, PlanId, SessionDate, DurationMinutes, TotalCalories, Notes)
            VALUES (@PersonId, @PlanId, @SessionDate, @DurationMinutes, @TotalCalories, @Notes);
            SELECT SCOPE_IDENTITY();";

        using SqlCommand cmd = new SqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@PersonId", session.PersonId);
        cmd.Parameters.AddWithValue("@PlanId", (object?)session.PlanId ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@SessionDate", session.SessionDate);
        cmd.Parameters.AddWithValue("@DurationMinutes", session.DurationMinutes);
        cmd.Parameters.AddWithValue("@TotalCalories", session.TotalCalories);
        cmd.Parameters.AddWithValue("@Notes", (object?)session.Notes ?? DBNull.Value);

        return Convert.ToInt32(cmd.ExecuteScalar());
    }

    public void InsertSessionLog(SessionLog log)
    {
        using SqlConnection conn = DatabaseHelper.GetConnection();
        conn.Open();

        string sql = @"
            INSERT INTO SessionLogs (SessionId, ExerciseId, SetNumber, ActualReps, ActualSeconds, WeightKg, CaloriesBurned)
            VALUES (@SessionId, @ExerciseId, @SetNumber, @ActualReps, @ActualSeconds, @WeightKg, @CaloriesBurned)";

        using SqlCommand cmd = new SqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@SessionId", log.SessionId);
        cmd.Parameters.AddWithValue("@ExerciseId", log.ExerciseId);
        cmd.Parameters.AddWithValue("@SetNumber", log.SetNumber);
        cmd.Parameters.AddWithValue("@ActualReps", (object?)log.ActualReps ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@ActualSeconds", (object?)log.ActualSeconds ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@WeightKg", (object?)log.WeightKg ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@CaloriesBurned", log.CaloriesBurned);

        cmd.ExecuteNonQuery();
    }

    public void UpdateSessionCalories(int sessionId, double totalCalories)
    {
        using SqlConnection conn = DatabaseHelper.GetConnection();
        conn.Open();

        using SqlCommand cmd = new SqlCommand(
            "UPDATE WorkoutSessions SET TotalCalories = @Cal WHERE SessionId = @SessionId", conn);
        cmd.Parameters.AddWithValue("@Cal", totalCalories);
        cmd.Parameters.AddWithValue("@SessionId", sessionId);
        cmd.ExecuteNonQuery();
    }

    public List<WorkoutSession> GetSessionHistory(int personId, int take)
    {
        List<WorkoutSession> sessions = new List<WorkoutSession>();

        using SqlConnection conn = DatabaseHelper.GetConnection();
        conn.Open();

        string sql = $@"
            SELECT TOP {take} SessionId, PersonId, PlanId, SessionDate, DurationMinutes, TotalCalories, Notes
            FROM WorkoutSessions
            WHERE PersonId = @PersonId
            ORDER BY SessionDate DESC";

        using SqlCommand cmd = new SqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@PersonId", personId);

        using SqlDataReader reader = cmd.ExecuteReader();
        while (reader.Read())
            sessions.Add(MapToSession(reader));

        return sessions;
    }

    public WorkoutSession? GetSessionById(int sessionId)
    {
        using SqlConnection conn = DatabaseHelper.GetConnection();
        conn.Open();

        string sql = @"
            SELECT SessionId, PersonId, PlanId, SessionDate, DurationMinutes, TotalCalories, Notes
            FROM WorkoutSessions WHERE SessionId = @SessionId";

        using SqlCommand cmd = new SqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@SessionId", sessionId);

        using SqlDataReader reader = cmd.ExecuteReader();
        if (!reader.Read()) return null;

        WorkoutSession session = MapToSession(reader);
        reader.Close();

        session.Logs = GetSessionLogs(sessionId);
        return session;
    }

    private List<WorkoutPlanExercise> GetPlanExercises(int planId)
    {
        List<WorkoutPlanExercise> list = new List<WorkoutPlanExercise>();

        using SqlConnection conn = DatabaseHelper.GetConnection();
        conn.Open();

        string sql = @"
            SELECT pe.PlanExerciseId, pe.PlanId, pe.ExerciseId, pe.DayOfWeek,
                   pe.OrderInDay, pe.Sets, pe.Reps, pe.Seconds, pe.RestSeconds,
                   e.Name, e.MuscleGroup
            FROM WorkoutPlanExercises pe
            JOIN Exercises e ON pe.ExerciseId = e.ExerciseId
            WHERE pe.PlanId = @PlanId
            ORDER BY pe.DayOfWeek, pe.OrderInDay";

        using SqlCommand cmd = new SqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@PlanId", planId);

        using SqlDataReader reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            list.Add(new WorkoutPlanExercise
            {
                PlanExerciseId = Convert.ToInt32(reader["PlanExerciseId"]),
                PlanId         = Convert.ToInt32(reader["PlanId"]),
                ExerciseId     = Convert.ToInt32(reader["ExerciseId"]),
                ExerciseName   = reader["Name"].ToString()!,
                MuscleGroup    = reader["MuscleGroup"].ToString()!,
                DayOfWeek      = Convert.ToInt32(reader["DayOfWeek"]),
                OrderInDay     = Convert.ToInt32(reader["OrderInDay"]),
                Sets           = Convert.ToInt32(reader["Sets"]),
                Reps           = reader["Reps"] == DBNull.Value ? null : Convert.ToInt32(reader["Reps"]),
                Seconds        = reader["Seconds"] == DBNull.Value ? null : Convert.ToInt32(reader["Seconds"]),
                RestSeconds    = Convert.ToInt32(reader["RestSeconds"])
            });
        }

        return list;
    }

    private List<SessionLog> GetSessionLogs(int sessionId)
    {
        List<SessionLog> list = new List<SessionLog>();

        using SqlConnection conn = DatabaseHelper.GetConnection();
        conn.Open();

        string sql = @"
            SELECT sl.LogId, sl.SessionId, sl.ExerciseId, sl.SetNumber,
                   sl.ActualReps, sl.ActualSeconds, sl.WeightKg, sl.CaloriesBurned,
                   e.Name AS ExerciseName
            FROM SessionLogs sl
            JOIN Exercises e ON sl.ExerciseId = e.ExerciseId
            WHERE sl.SessionId = @SessionId
            ORDER BY sl.SetNumber";

        using SqlCommand cmd = new SqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@SessionId", sessionId);

        using SqlDataReader reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            list.Add(new SessionLog
            {
                LogId         = Convert.ToInt32(reader["LogId"]),
                SessionId     = Convert.ToInt32(reader["SessionId"]),
                ExerciseId    = Convert.ToInt32(reader["ExerciseId"]),
                ExerciseName  = reader["ExerciseName"].ToString()!,
                SetNumber     = Convert.ToInt32(reader["SetNumber"]),
                ActualReps    = reader["ActualReps"] == DBNull.Value ? null : Convert.ToInt32(reader["ActualReps"]),
                ActualSeconds = reader["ActualSeconds"] == DBNull.Value ? null : Convert.ToInt32(reader["ActualSeconds"]),
                WeightKg      = reader["WeightKg"] == DBNull.Value ? null : Convert.ToDouble(reader["WeightKg"]),
                CaloriesBurned = Convert.ToDouble(reader["CaloriesBurned"])
            });
        }

        return list;
    }

    private WorkoutPlan MapToPlan(SqlDataReader reader)
    {
        return new WorkoutPlan
        {
            PlanId             = Convert.ToInt32(reader["PlanId"]),
            CreatedByPersonId  = Convert.ToInt32(reader["CreatedByPersonId"]),
            AssignedToPersonId = reader["AssignedToPersonId"] == DBNull.Value ? null : Convert.ToInt32(reader["AssignedToPersonId"]),
            GoalId             = Convert.ToInt32(reader["GoalId"]),
            GoalName           = reader["GoalName"].ToString()!,
            PlanName           = reader["PlanName"].ToString()!,
            DurationWeeks      = Convert.ToInt32(reader["DurationWeeks"]),
            IsActive           = Convert.ToBoolean(reader["IsActive"]),
            CreatedAt          = Convert.ToDateTime(reader["CreatedAt"])
        };
    }

    private WorkoutSession MapToSession(SqlDataReader reader)
    {
        return new WorkoutSession
        {
            SessionId       = Convert.ToInt32(reader["SessionId"]),
            PersonId        = Convert.ToInt32(reader["PersonId"]),
            PlanId          = reader["PlanId"] == DBNull.Value ? null : Convert.ToInt32(reader["PlanId"]),
            SessionDate     = Convert.ToDateTime(reader["SessionDate"]),
            DurationMinutes = Convert.ToInt32(reader["DurationMinutes"]),
            TotalCalories   = Convert.ToDouble(reader["TotalCalories"]),
            Notes           = reader["Notes"] == DBNull.Value ? null : reader["Notes"].ToString()
        };
    }
}
