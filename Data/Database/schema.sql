CREATE DATABASE FitTrackOOP;
GO

USE FitTrackOOP;
GO

CREATE TABLE Goals (
    GoalId       INT PRIMARY KEY IDENTITY(1,1),
    GoalName     NVARCHAR(50)  NOT NULL UNIQUE,
    Description  NVARCHAR(300) NOT NULL,
    CalorieDelta INT           NOT NULL
);

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

CREATE TABLE ExerciseCategories (
    CategoryId   INT PRIMARY KEY IDENTITY(1,1),
    CategoryName NVARCHAR(50) NOT NULL UNIQUE
);

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

CREATE TABLE WorkoutSessions (
    SessionId       INT PRIMARY KEY IDENTITY(1,1),
    PersonId        INT           NOT NULL REFERENCES Persons(PersonId) ON DELETE CASCADE,
    PlanId          INT           NULL REFERENCES WorkoutPlans(PlanId),
    SessionDate     DATETIME      NOT NULL,
    DurationMinutes INT           NOT NULL,
    TotalCalories   FLOAT         NOT NULL,
    Notes           NVARCHAR(500) NULL
);

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

CREATE TABLE FoodCategories (
    FoodCategoryId INT PRIMARY KEY IDENTITY(1,1),
    CategoryName   NVARCHAR(50) NOT NULL UNIQUE
);

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

CREATE TABLE ProgressSnapshots (
    SnapshotId   INT PRIMARY KEY IDENTITY(1,1),
    PersonId     INT           NOT NULL REFERENCES Persons(PersonId) ON DELETE CASCADE,
    SnapshotDate DATETIME      NOT NULL DEFAULT GETDATE(),
    WeightKg     FLOAT         NOT NULL,
    BodyFatPct   FLOAT         NULL,
    BMI          FLOAT         NOT NULL,
    Notes        NVARCHAR(500) NULL
);

select * from Goals;

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

-- Clear existing old data (the 58 items) before bulk inserting
DELETE FROM FoodItems;
DBCC CHECKIDENT ('FoodItems', RESEED, 0);
GO
CREATE VIEW vw_FoodItems AS
SELECT FoodCategoryId, GoalId, FoodName, CaloriesPer100g, ProteinPer100g, CarbsPer100g, FatPer100g, FiberPer100g
FROM FoodItems;
GO

BULK INSERT vw_FoodItems
FROM 'C:\Users\wasif_znaefim\Downloads\fitpro\Data\FoodItems_Enriched.csv'
WITH (
    FORMAT = 'CSV',
    FIRSTROW = 2,
    FIELDTERMINATOR = ',',
    ROWTERMINATOR = '\n' -- Use '\n' or '\r\n' based on file encoding
);
GO

DROP VIEW vw_FoodItems;
GO