using FitTrack.Database;
using FitTrack.Models;

namespace FitTrack.Services;

public class GoalService
{
    private GoalRepository _goalRepo = new GoalRepository();
    private PersonRepository _personRepo = new PersonRepository();

    public List<Goal> GetAllGoals()
    {
        return _goalRepo.GetAll();
    }

    public Goal? GetGoalById(int goalId)
    {
        return _goalRepo.GetById(goalId);
    }

    public (double bmi, string category, double bmr, double tdee, double targetCalories)
        GetHealthMetrics(int personId, double activityMultiplier)
    {
        Person? person = _personRepo.GetById(personId);
        if (person == null)
            return (0, "Unknown", 0, 0, 0);

        double bmi  = person.CalculateBMI();
        string cat  = person.GetBMICategory();
        double bmr  = person.CalculateBMR();
        double tdee = person.CalculateTDEE(activityMultiplier);

        double targetCalories = tdee;
        if (person.GoalId.HasValue)
        {
            Goal? goal = _goalRepo.GetById(person.GoalId.Value);
            if (goal != null)
                targetCalories = tdee + goal.CalorieDelta;
        }

        return (bmi, cat, bmr, tdee, targetCalories);
    }
}
