IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'FitTrackOOP')
BEGIN
    CREATE DATABASE FitTrackOOP;
END
GO 

USE FitTrackOOP;
GO



IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Goals')
CREATE TABLE Goals (
    GoalId       INT PRIMARY KEY IDENTITY(1,1),
    GoalName     NVARCHAR(50)  NOT NULL UNIQUE,
    Description  NVARCHAR(300) NOT NULL,
    CalorieDelta INT           NOT NULL
);

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Persons')
CREATE TABLE Persons (
    PersonId      INT PRIMARY KEY IDENTITY(1,1),
    FullName      NVARCHAR(100) NOT NULL,
    Username      NVARCHAR(50)  NOT NULL UNIQUE,
    PasswordHash  NVARCHAR(256) NOT NULL,
    Email         NVARCHAR(150) NOT NULL UNIQUE,
    Role          NVARCHAR(10)  NOT NULL,
    Gender        NVARCHAR(10)  NOT NULL,
    Age           INT           NOT NULL,
    HeightCm      FLOAT         NOT NULL,
    WeightKg      FLOAT         NOT NULL,
    BodyFatPct    FLOAT         NULL,
    GoalId        INT           NULL REFERENCES Goals(GoalId),
    TrainerId     INT           NULL REFERENCES Persons(PersonId),
    CreatedAt     DATETIME      NOT NULL DEFAULT GETDATE()
);

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'ExerciseCategories')
CREATE TABLE ExerciseCategories (
    CategoryId   INT PRIMARY KEY IDENTITY(1,1),
    CategoryName NVARCHAR(50) NOT NULL UNIQUE
);

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Exercises')
CREATE TABLE Exercises (
    ExerciseId   INT PRIMARY KEY IDENTITY(1,1),
    CategoryId   INT           NOT NULL REFERENCES ExerciseCategories(CategoryId),
    GoalId       INT           NULL REFERENCES Goals(GoalId),
    Name         NVARCHAR(100) NOT NULL,
    MuscleGroup  NVARCHAR(100) NOT NULL,
    Equipment    NVARCHAR(100) NOT NULL,
    DefaultSets  INT           NOT NULL,
    DefaultReps  INT           NULL,
    DefaultSecs  INT           NULL,
    METValue     FLOAT         NOT NULL,
    Description  NVARCHAR(500) NULL
);

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'WorkoutPlans')
CREATE TABLE WorkoutPlans (
    PlanId             INT PRIMARY KEY IDENTITY(1,1),
    CreatedByPersonId  INT           NOT NULL REFERENCES Persons(PersonId),
    AssignedToPersonId INT           NULL REFERENCES Persons(PersonId),
    GoalId             INT           NOT NULL REFERENCES Goals(GoalId),
    PlanName           NVARCHAR(100) NOT NULL,
    DurationWeeks      INT           NOT NULL,
    IsActive           BIT           NOT NULL DEFAULT 1,
    CreatedAt          DATETIME      NOT NULL DEFAULT GETDATE()
);

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'WorkoutPlanExercises')
CREATE TABLE WorkoutPlanExercises (
    PlanExerciseId INT PRIMARY KEY IDENTITY(1,1),
    PlanId         INT NOT NULL REFERENCES WorkoutPlans(PlanId) ON DELETE CASCADE,
    ExerciseId     INT NOT NULL REFERENCES Exercises(ExerciseId),
    DayOfWeek      INT NOT NULL,
    OrderInDay     INT NOT NULL,
    Sets           INT NOT NULL,
    Reps           INT NULL,
    Seconds        INT NULL,
    RestSeconds    INT NOT NULL
);

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'WorkoutSessions')
CREATE TABLE WorkoutSessions (
    SessionId       INT PRIMARY KEY IDENTITY(1,1),
    PersonId        INT           NOT NULL REFERENCES Persons(PersonId) ON DELETE CASCADE,
    PlanId          INT           NULL REFERENCES WorkoutPlans(PlanId),
    SessionDate     DATETIME      NOT NULL,
    DurationMinutes INT           NOT NULL,
    TotalCalories   FLOAT         NOT NULL,
    Notes           NVARCHAR(500) NULL
);

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'SessionLogs')
CREATE TABLE SessionLogs (
    LogId          INT PRIMARY KEY IDENTITY(1,1),
    SessionId      INT   NOT NULL REFERENCES WorkoutSessions(SessionId) ON DELETE CASCADE,
    ExerciseId     INT   NOT NULL REFERENCES Exercises(ExerciseId),
    SetNumber      INT   NOT NULL,
    ActualReps     INT   NULL,
    ActualSeconds  INT   NULL,
    WeightKg       FLOAT NULL,
    CaloriesBurned FLOAT NOT NULL
);

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'FoodCategories')
CREATE TABLE FoodCategories (
    FoodCategoryId INT PRIMARY KEY IDENTITY(1,1),
    CategoryName   NVARCHAR(50) NOT NULL UNIQUE
);

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'FoodItems')
CREATE TABLE FoodItems (
    FoodItemId      INT PRIMARY KEY IDENTITY(1,1),
    FoodCategoryId  INT           NOT NULL REFERENCES FoodCategories(FoodCategoryId),
    GoalId          INT           NULL REFERENCES Goals(GoalId),
    FoodName        NVARCHAR(150) NOT NULL UNIQUE,
    CaloriesPer100g FLOAT         NOT NULL,
    ProteinPer100g  FLOAT         NOT NULL,
    CarbsPer100g    FLOAT         NOT NULL,
    FatPer100g      FLOAT         NOT NULL,
    FiberPer100g    FLOAT         NULL
);

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'NutritionLogs')
CREATE TABLE NutritionLogs (
    NutritionLogId INT PRIMARY KEY IDENTITY(1,1),
    PersonId       INT           NOT NULL REFERENCES Persons(PersonId) ON DELETE CASCADE,
    FoodItemId     INT           NOT NULL REFERENCES FoodItems(FoodItemId),
    MealType       NVARCHAR(20)  NOT NULL,
    ServingGrams   FLOAT         NOT NULL,
    Calories       FLOAT         NOT NULL,
    ProteinG       FLOAT         NOT NULL,
    CarbsG         FLOAT         NOT NULL,
    FatG           FLOAT         NOT NULL,
    LoggedAt       DATETIME      NOT NULL DEFAULT GETDATE()
);

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'ProgressSnapshots')
CREATE TABLE ProgressSnapshots (
    SnapshotId   INT PRIMARY KEY IDENTITY(1,1),
    PersonId     INT           NOT NULL REFERENCES Persons(PersonId) ON DELETE CASCADE,
    SnapshotDate DATETIME      NOT NULL DEFAULT GETDATE(),
    WeightKg     FLOAT         NOT NULL,
    BodyFatPct   FLOAT         NULL,
    BMI          FLOAT         NOT NULL,
    Notes        NVARCHAR(500) NULL
);
GO


IF NOT EXISTS (SELECT 1 FROM Goals)
BEGIN

    INSERT INTO Goals (GoalName, Description, CalorieDelta) VALUES
    ('Weight Loss',  'Lose overall body weight through calorie deficit and cardio.', -500),
    ('Fat Loss',     'Reduce body fat percentage with HIIT and strength training.',  -300),
    ('Muscle Gain',  'Build lean muscle with progressive strength training.',         300),
    ('Weight Gain',  'Increase total body weight with heavy compound lifts.',         500),
    ('Maintain',     'Maintain current body composition with balanced training.',       0);

 
    INSERT INTO ExerciseCategories (CategoryName) VALUES
    ('Strength'), ('Cardio'), ('HIIT'), ('Flexibility');

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
    (4, 5, 'Neck Side Stretch',    'Neck',         'Bodyweight',      2, NULL, 20,   1.5,  'Tilt ear toward shoulder. Gentle overpressure. Do not rotate.');

    INSERT INTO FoodCategories (CategoryName) VALUES
    ('Protein'), 
    ('Carbohydrate'), 
    ('Fat'), 
    ('Vegetable'), 
    ('Fruit'), 
    ('Dairy'),
    ('Beverages'),
    ('Snacks & Appetizers'),
    ('Breakfast Cereal'),
    ('Mixed Meals'),
    ('Soups & Stocks');

    -- Bulk insert for FoodItems from local CSV
CREATE VIEW vw_FoodItems AS
SELECT FoodCategoryId, GoalId, FoodName, CaloriesPer100g, ProteinPer100g, CarbsPer100g, FatPer100g, FiberPer100g
FROM FoodItems;
GO

BULK INSERT vw_FoodItems
FROM 'C:\fittrakpro\FoodItems_Enriched.csv'
WITH (
    FORMAT = 'CSV',
    FIRSTROW = 2,
    FIELDTERMINATOR = ',',
    ROWTERMINATOR = '\n' -- Use '\n' or '\r\n' based on file encoding
);
GO

DROP VIEW vw_FoodItems;
GO


    PRINT '✅ Seed data inserted successfully.';
END
ELSE
BEGIN
    PRINT '⏭️ Seed data already exists — skipping.';
END
GO

PRINT '✅ FitTrackOOP database is ready.';
GO

select * from Persons;

select * from FoodItems;