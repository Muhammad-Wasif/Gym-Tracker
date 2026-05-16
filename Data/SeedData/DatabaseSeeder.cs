using Microsoft.Data.SqlClient;

namespace FitTrack.Database;

public class DatabaseSeeder
{
    public static void Seed()
    {
        if (IsAlreadySeeded()) return;

        SeedGoals();
        SeedExerciseCategories();
        SeedExercises();
        SeedFoodCategories();
        SeedFoodItems();
    }

    private static bool IsAlreadySeeded()
    {
        using SqlConnection conn = DatabaseHelper.GetConnection();
        conn.Open();

        using SqlCommand cmd = new SqlCommand("SELECT COUNT(1) FROM Goals", conn);
        return (int)cmd.ExecuteScalar()! > 0;
    }

    private static void SeedGoals()
    {
        string sql = @"
            INSERT INTO Goals (GoalName, Description, CalorieDelta) VALUES
            ('Weight Loss',  'Lose overall body weight through calorie deficit and cardio.', -500),
            ('Fat Loss',     'Reduce body fat percentage with HIIT and strength training.',  -300),
            ('Muscle Gain',  'Build lean muscle with progressive strength training.',         300),
            ('Weight Gain',  'Increase total body weight with heavy compound lifts.',         500),
            ('Maintain',     'Maintain current body composition with balanced training.',       0);";

        DatabaseHelper.RunSql(sql);
    }

    private static void SeedExerciseCategories()
    {
        string sql = @"
            INSERT INTO ExerciseCategories (CategoryName) VALUES
            ('Strength'), ('Cardio'), ('HIIT'), ('Flexibility');";

        DatabaseHelper.RunSql(sql);
    }

    private static void SeedExercises()
    {
        string sql = @"
            INSERT INTO Exercises (CategoryId, GoalId, Name, MuscleGroup, Equipment, DefaultSets, DefaultReps, DefaultSecs, METValue, Description) VALUES
            (1, 3, 'Bench Press',            'Chest',        'Barbell',         4, 8,    NULL, 5.0,  'Lie flat. Lower bar to chest. Press to lockout.'),
            (1, 4, 'Squat',                  'Quadriceps',   'Barbell',         4, 6,    NULL, 5.0,  'Bar on traps. Descend to parallel. Drive through heels.'),
            (1, 4, 'Deadlift',               'Lower Back',   'Barbell',         4, 5,    NULL, 6.0,  'Hinge at hips. Neutral spine. Lock hips at top.'),
            (1, 3, 'Overhead Press',         'Shoulders',    'Barbell',         4, 8,    NULL, 4.0,  'Press bar vertical overhead. Squeeze glutes for stability.'),
            (1, 3, 'Barbell Row',            'Upper Back',   'Barbell',         4, 8,    NULL, 4.5,  'Hinge to parallel. Pull to lower chest.'),
            (1, 3, 'Pull-Up',               'Lats',         'Bodyweight',      3, 10,   NULL, 4.0,  'Dead hang. Pull chin over bar. Controlled descent.'),
            (1, 3, 'Dumbbell Curl',          'Biceps',       'Dumbbell',        3, 12,   NULL, 3.0,  'Keep elbows pinned. Full range of motion.'),
            (1, 3, 'Tricep Dip',            'Triceps',      'Parallel Bars',   3, 12,   NULL, 3.8,  'Lower until elbows 90 degrees. Full lockout.'),
            (1, 4, 'Leg Press',             'Quadriceps',   'Machine',         4, 10,   NULL, 4.0,  'Feet hip-width. Lower to 90 degrees. No knee lockout.'),
            (1, 3, 'Romanian Deadlift',     'Hamstrings',   'Barbell',         3, 10,   NULL, 4.5,  'Soft knee. Hinge till hamstring stretch. Drive hips forward.'),
            (1, 3, 'Hip Thrust',            'Glutes',       'Barbell',         4, 12,   NULL, 4.0,  'Bench under shoulder blades. Drive hips to ceiling.'),
            (1, 3, 'Incline Dumbbell Press','Upper Chest',  'Dumbbell',        3, 10,   NULL, 4.0,  'Bench at 30 degrees. Press at angle of incline.'),
            (1, 3, 'Lateral Raise',         'Shoulders',    'Dumbbell',        3, 15,   NULL, 2.5,  'Raise to shoulder height. Lead with elbows.'),
            (1, 5, 'Plank',                 'Core',         'Bodyweight',      3, NULL, 60,   3.5,  'Forearms on floor. Body straight. Breathe steadily.'),
            (1, 5, 'Push-Up',              'Chest',        'Bodyweight',      3, 20,   NULL, 3.8,  'Hands below shoulders. Lower chest to floor. Full lockout.'),
            (1, 2, 'Dumbbell Lunge',        'Quadriceps',   'Dumbbell',        3, 12,   NULL, 4.0,  'Step forward. Lower back knee to floor. Drive front heel up.'),
            (1, 3, 'Hammer Curl',           'Brachialis',   'Dumbbell',        3, 12,   NULL, 3.0,  'Neutral grip throughout. No swinging.'),
            (1, 4, 'Calf Raise',            'Calves',       'Machine',         4, 15,   NULL, 2.5,  'Full range of motion. Slow lowering phase.'),
            (2, 1, 'Treadmill Running',     'Full Body',    'Treadmill',       1, NULL, 1800, 9.8,  'Moderate pace 5-7 mph. Land midfoot.'),
            (2, 1, 'Stationary Cycling',    'Quadriceps',   'Stationary Bike', 1, NULL, 1800, 7.0,  'Seat at hip height. Maintain 80-100 RPM.'),
            (2, 2, 'Rowing Machine',        'Full Body',    'Rowing Machine',  1, NULL, 1200, 7.0,  'Legs then back then arms. Keep back straight.'),
            (2, 1, 'Jump Rope',            'Calves',       'Jump Rope',       5, NULL, 120,  11.8, 'Stay on balls of feet. Wrists do the work.'),
            (2, 1, 'Elliptical Trainer',    'Full Body',    'Elliptical',      1, NULL, 1800, 5.0,  'Push and pull arms. Maintain upright posture.'),
            (2, 2, 'Stair Climber',         'Glutes',       'Stair Machine',   1, NULL, 1200, 9.0,  'Do not lean on handrails. Full step not just toes.'),
            (2, 1, 'Incline Walk',          'Glutes',       'Treadmill',       1, NULL, 2400, 5.0,  '10-15 percent incline. 3-4 mph. Do not hold handrails.'),
            (3, 2, 'Burpee',               'Full Body',    'Bodyweight',      5, 10,   NULL, 8.0,  'Squat thrust. Explosive jump at top.'),
            (3, 2, 'Box Jump',             'Quadriceps',   'Plyo Box',        4, 8,    NULL, 8.0,  'Land softly with bent knees. Step down never jump down.'),
            (3, 2, 'Kettlebell Swing',      'Hamstrings',   'Kettlebell',      4, 15,   NULL, 9.8,  'Hip hinge not squat. Snap hips forward. Bell to shoulder height.'),
            (3, 1, 'Mountain Climber',      'Core',         'Bodyweight',      4, NULL, 30,   8.0,  'High plank. Drive knees to chest alternately. Hips level.'),
            (3, 2, 'Sprint Interval',       'Full Body',    'Treadmill',       8, NULL, 30,   14.0, '90 percent max effort. Full recovery between sprints.'),
            (3, 2, 'Jump Squat',           'Quadriceps',   'Bodyweight',      4, 12,   NULL, 7.0,  'Squat to parallel. Explode upward. Land softly.'),
            (3, 2, 'Battle Ropes',          'Shoulders',    'Battle Ropes',    5, NULL, 30,   10.0, 'Alternate waves. Stay in athletic stance.'),
            (3, 2, 'Medicine Ball Slam',    'Core',         'Medicine Ball',   4, 10,   NULL, 8.0,  'Raise overhead. Slam with maximum force.'),
            (4, 5, 'Hamstring Stretch',     'Hamstrings',   'Bodyweight',      2, NULL, 30,   2.3,  'Hinge at hips. Keep back flat. Feel stretch in hamstrings.'),
            (4, 5, 'Hip Flexor Stretch',    'Hip Flexors',  'Bodyweight',      2, NULL, 30,   2.3,  'Kneeling lunge. Tuck pelvis. Drive hip forward.'),
            (4, 5, 'Pigeon Pose',          'Glutes',       'Yoga Mat',        2, NULL, 45,   2.3,  'Front leg 90 degrees. Lower torso over front leg.'),
            (4, 5, 'Cat-Cow Stretch',      'Spine',        'Yoga Mat',        2, 10,   NULL, 2.0,  'Alternate rounding and arching spine. Breathe with movement.'),
            (4, 5, 'Standing Quad Stretch','Quadriceps',   'Bodyweight',      2, NULL, 30,   2.0,  'Balance on one leg. Pull heel to glute. Knees together.'),
            (4, 5, 'Child Pose',           'Lower Back',   'Yoga Mat',        2, NULL, 45,   2.0,  'Kneel. Sit on heels. Arms extended. Forehead to mat.'),
            (4, 5, 'Neck Side Stretch',    'Neck',         'Bodyweight',      2, NULL, 20,   1.5,  'Tilt ear toward shoulder. Gentle overpressure. Do not rotate.');";

        DatabaseHelper.RunSql(sql);
    }

    private static void SeedFoodCategories()
    {
        string sql = @"
            INSERT INTO FoodCategories (CategoryName) VALUES
            ('Protein'), ('Carbohydrate'), ('Fat'), ('Vegetable'), ('Fruit'), ('Dairy');";

        DatabaseHelper.RunSql(sql);
    }

    private static void SeedFoodItems()
    {
        string sql = @"
            INSERT INTO FoodItems (FoodCategoryId, GoalId, FoodName, CaloriesPer100g, ProteinPer100g, CarbsPer100g, FatPer100g, FiberPer100g) VALUES
            (1, 3, 'Chicken Breast',       165, 31.0, 0.0,  3.6,  0.0),
            (1, 3, 'Salmon',               208, 20.0, 0.0,  13.0, 0.0),
            (1, 2, 'Tuna in Water',        116, 26.0, 0.0,  1.0,  0.0),
            (1, 5, 'Whole Egg',            155, 13.0, 1.1,  11.0, 0.0),
            (1, 2, 'Egg White',             52, 11.0, 0.7,  0.2,  0.0),
            (1, 3, 'Lean Ground Beef',     170, 26.0, 0.0,  7.0,  0.0),
            (1, 1, 'Turkey Breast',        135, 30.0, 0.0,  1.0,  0.0),
            (1, 2, 'Shrimp',                99, 24.0, 0.0,  0.3,  0.0),
            (1, 1, 'Cod',                  105, 23.0, 0.0,  0.9,  0.0),
            (1, 3, 'Whey Protein Powder',  370, 80.0, 8.0,  4.0,  0.0),
            (1, 5, 'Tofu Firm',             76,  8.0, 2.0,  4.8,  0.3),
            (1, 1, 'Lentils Cooked',       116,  9.0, 20.0, 0.4,  7.9),
            (1, 5, 'Chickpeas Cooked',     164,  8.9, 27.0, 2.6,  7.6),
            (1, 5, 'Black Beans Cooked',   132,  8.9, 24.0, 0.5,  8.7),
            (2, 4, 'White Rice Cooked',    130,  2.7, 28.0, 0.3,  0.4),
            (2, 3, 'Brown Rice Cooked',    112,  2.6, 23.0, 0.9,  1.8),
            (2, 3, 'Oats Dry',             389, 17.0, 66.0, 7.0,  10.6),
            (2, 5, 'Whole Wheat Bread',    247, 13.0, 41.0, 4.2,  6.8),
            (2, 4, 'White Bread',          265,  9.0, 49.0, 3.2,  2.7),
            (2, 5, 'Quinoa Cooked',        120,  4.4, 22.0, 1.9,  2.8),
            (2, 1, 'Sweet Potato Cooked',   86,  1.6, 20.0, 0.1,  3.3),
            (2, 4, 'White Potato Cooked',   87,  2.5, 20.0, 0.1,  1.8),
            (2, 3, 'Pasta Cooked',         158,  5.8, 31.0, 0.9,  1.8),
            (2, 5, 'Whole Wheat Pasta',    149,  5.5, 29.0, 1.1,  4.0),
            (2, 4, 'Dates',                277,  1.8, 75.0, 0.2,  6.7),
            (3, 2, 'Avocado',              160,  2.0,  9.0, 15.0, 6.7),
            (3, 3, 'Almond Butter',        614, 21.0, 19.0, 56.0, 10.3),
            (3, 4, 'Peanut Butter',        588, 25.0, 20.0, 50.0, 6.0),
            (3, 5, 'Olive Oil',            884,  0.0,  0.0, 100.0, 0.0),
            (3, 3, 'Almonds',              579, 21.0, 22.0, 50.0, 12.5),
            (3, 5, 'Walnuts',              654, 15.0, 14.0, 65.0, 6.7),
            (3, 2, 'Chia Seeds',           486, 17.0, 42.0, 31.0, 34.4),
            (4, 1, 'Broccoli Raw',          34,  2.8,  7.0, 0.4,  2.6),
            (4, 1, 'Spinach Raw',           23,  2.9,  3.6, 0.4,  2.2),
            (4, 2, 'Kale Raw',              49,  4.3,  9.0, 0.9,  3.6),
            (4, 1, 'Cucumber',              15,  0.7,  3.6, 0.1,  0.5),
            (4, 5, 'Carrot Raw',            41,  0.9, 10.0, 0.2,  2.8),
            (4, 1, 'Bell Pepper Raw',       31,  1.0,  6.0, 0.3,  2.1),
            (4, 1, 'Tomato Raw',            18,  0.9,  3.9, 0.2,  1.2),
            (4, 1, 'Zucchini Cooked',       17,  1.2,  3.6, 0.4,  1.1),
            (4, 2, 'Cauliflower Raw',       25,  1.9,  5.0, 0.3,  2.0),
            (4, 1, 'Asparagus Cooked',      22,  2.4,  4.1, 0.2,  2.1),
            (4, 5, 'Mushrooms Raw',         22,  3.1,  3.3, 0.3,  1.0),
            (4, 1, 'Celery Raw',            16,  0.7,  3.0, 0.2,  1.6),
            (5, 3, 'Banana',                89,  1.1, 23.0, 0.3,  2.6),
            (5, 5, 'Apple',                 52,  0.3, 14.0, 0.2,  2.4),
            (5, 2, 'Blueberries',           57,  0.7, 14.0, 0.3,  2.4),
            (5, 1, 'Strawberries',          32,  0.7,  7.7, 0.3,  2.0),
            (5, 4, 'Mango',                 60,  0.8, 15.0, 0.4,  1.6),
            (5, 1, 'Watermelon',            30,  0.6,  7.6, 0.2,  0.4),
            (5, 5, 'Orange',                47,  0.9, 12.0, 0.1,  2.4),
            (6, 2, 'Greek Yogurt 0 Fat',    59, 10.0,  3.6, 0.4,  0.0),
            (6, 3, 'Greek Yogurt Full Fat',  97,  9.0,  3.98, 5.0, 0.0),
            (6, 2, 'Cottage Cheese Low Fat', 72, 12.0,  3.4, 1.0,  0.0),
            (6, 4, 'Whole Milk',            61,  3.2,  4.8, 3.3,  0.0),
            (6, 1, 'Skim Milk',             34,  3.4,  5.0, 0.1,  0.0),
            (6, 3, 'Cheddar Cheese',       403, 25.0,  1.3, 33.0, 0.0),
            (6, 3, 'Mozzarella',           280, 28.0,  3.1, 17.0, 0.0);";

        DatabaseHelper.RunSql(sql);
    }
}
