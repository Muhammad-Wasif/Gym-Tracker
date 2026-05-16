using FitTrack.Database;
using FitTrack.Models;

namespace FitTrack.Services;

public class WorkoutService
{
    private WorkoutRepository _workoutRepo = new WorkoutRepository();
    private ExerciseRepository _exerciseRepo = new ExerciseRepository();
    private PersonRepository _personRepo = new PersonRepository();
    private GoalRepository _goalRepo = new GoalRepository();

    public List<Exercise> GetAllExercises()
    {
        return _exerciseRepo.GetAll();
    }

    public List<ExerciseCategory> GetAllCategories()
    {
        return _exerciseRepo.GetAllCategories();
    }

    public List<Exercise> GetExercisesByCategory(int categoryId)
    {
        return _exerciseRepo.GetByCategory(categoryId);
    }

    public WorkoutPlan? GetActivePlan(int personId)
    {
        return _workoutRepo.GetActivePlan(personId);
    }

    public List<WorkoutPlan> GetAllPlansForPerson(int personId)
    {
        return _workoutRepo.GetAllPlansForPerson(personId);
    }

    public List<WorkoutPlan> GetAllPlans()
    {
        return _workoutRepo.GetAllPlans();
    }

    public (bool success, string message, WorkoutPlan? plan) CreatePlan(
        int createdByPersonId, int? assignedToPersonId,
        int goalId, string planName, int durationWeeks,
        List<(int exerciseId, int dayOfWeek, int orderInDay, int sets, int? reps, int? seconds, int restSeconds)> exercises)
    {
        if (string.IsNullOrWhiteSpace(planName))
            return (false, "Plan name cannot be empty.", null);

        if (!_goalRepo.Exists(goalId))
            return (false, "Goal not found.", null);

        if (assignedToPersonId.HasValue)
        {
            Person? assignee = _personRepo.GetById(assignedToPersonId.Value);
            if (assignee == null)
                return (false, "Assigned user not found.", null);

            _workoutRepo.DeactivateAllPlansForPerson(assignedToPersonId.Value);
        }

        WorkoutPlan plan = new WorkoutPlan
        {
            CreatedByPersonId  = createdByPersonId,
            AssignedToPersonId = assignedToPersonId,
            GoalId             = goalId,
            PlanName           = planName.Trim(),
            DurationWeeks      = durationWeeks,
            IsActive           = true,
            CreatedAt          = DateTime.Now
        };

        int planId = _workoutRepo.InsertPlan(plan);
        plan.PlanId = planId;

        foreach (var ex in exercises)
        {
            WorkoutPlanExercise pe = new WorkoutPlanExercise
            {
                PlanId     = planId,
                ExerciseId = ex.exerciseId,
                DayOfWeek  = ex.dayOfWeek,
                OrderInDay = ex.orderInDay,
                Sets       = ex.sets,
                Reps       = ex.reps,
                Seconds    = ex.seconds,
                RestSeconds = ex.restSeconds
            };
            _workoutRepo.InsertPlanExercise(pe);
        }

        WorkoutPlan? saved = _workoutRepo.GetActivePlan(assignedToPersonId ?? createdByPersonId);
        return (true, "Plan created successfully.", saved);
    }

    public (bool success, string message) DeletePlan(int planId)
    {
        _workoutRepo.DeletePlan(planId);
        return (true, "Plan deleted.");
    }

    public (bool success, string message, WorkoutSession? session) LogSession(
        int personId, int? planId, int durationMinutes, string? notes,
        List<(int exerciseId, int setNumber, int? actualReps, int? actualSeconds, double? weightKg)> sets)
    {
        Person? person = _personRepo.GetById(personId);
        if (person == null)
            return (false, "User not found.", null);

        if (sets.Count == 0)
            return (false, "Session must have at least one set.", null);

        WorkoutSession session = new WorkoutSession
        {
            PersonId        = personId,
            PlanId          = planId,
            SessionDate     = DateTime.Now,
            DurationMinutes = durationMinutes,
            TotalCalories   = 0,
            Notes           = notes
        };

        int sessionId = _workoutRepo.InsertSession(session);
        session.SessionId = sessionId;

        double totalCalories = 0;

        foreach (var set in sets)
        {
            Exercise? exercise = _exerciseRepo.GetById(set.exerciseId);
            if (exercise == null) continue;

            int durationSecs = set.actualSeconds ?? (set.actualReps.HasValue ? set.actualReps.Value * 3 : 0);
            double calories  = exercise.CalculateCaloriesBurned(person.WeightKg, durationSecs);
            totalCalories   += calories;

            SessionLog log = new SessionLog
            {
                SessionId     = sessionId,
                ExerciseId    = set.exerciseId,
                ExerciseName  = exercise.Name,
                SetNumber     = set.setNumber,
                ActualReps    = set.actualReps,
                ActualSeconds = set.actualSeconds,
                WeightKg      = set.weightKg,
                CaloriesBurned = Math.Round(calories, 2)
            };

            _workoutRepo.InsertSessionLog(log);
        }

        _workoutRepo.UpdateSessionCalories(sessionId, Math.Round(totalCalories, 2));

        WorkoutSession? saved = _workoutRepo.GetSessionById(sessionId);
        return (true, "Session logged.", saved);
    }

    public List<WorkoutSession> GetSessionHistory(int personId, int take = 10)
    {
        return _workoutRepo.GetSessionHistory(personId, take);
    }
}
