using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace TimesTable.Mobile.Models;

public class Exercise
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    [Indexed]
    public long StartTime { get; set; }
    
    public long FinishTime { get; set; }

    public long GivenTimePerQuestion { get; set; }

    [Ignore]
    public long DifficultyLevel
    {
        get
        {
            switch (GivenTimePerQuestion)
            {
                case >= 30000: return 1;
                case >= 20000: return 2;
                case >= 15000: return 3;
                case >= 10000: return 5;
                case >= 7000: return 8;
                case >= 5000: return 14;
                case >= 3000: return 21;
            }

            return 0;
        }
    }

    [Ignore]
    public int TotalQuestions
    {
        get
        {
            if (ExerciseQuestions is null)
            {
                return 0;
            }

            return ExerciseQuestions.Count;
        }
    }

    [Ignore]
    public int DisplayedQuestions
    {
        get
        {
            if (ExerciseQuestions is null)
            {
                return 0;
            }

            return ExerciseQuestions.Count(x => x.DisplayTime is not null);
        }
    }

    [Ignore]
    public int AnsweredQuestions
    {
        get
        {
            if (ExerciseQuestions is null)
            {
                return 0;
            }

            return ExerciseQuestions.Count(x => !x.IsSkipped && x.IsDisplayed);
        }
    }

    [Ignore]
    public int CorrectAnswers
    {
        get
        {
            if (ExerciseQuestions is null)
            {
                return 0;
            }

            return ExerciseQuestions.Count(x => x.IsCorrectAnswer);
        }
    }

    [Ignore]
    public int WrongAnswers
    {
        get
        {
            if (ExerciseQuestions is null)
            {
                return 0;
            }

            return ExerciseQuestions.Count(x => !x.IsCorrectAnswer);
        }
    }

    [Ignore]
    public int SkippedQuestions
    {
        get
        {
            if (ExerciseQuestions is null)
            {
                return 0;
            }

            return ExerciseQuestions.Count(x => x is { IsSkipped: true, DisplayTime: not null });
        }
    }

    [Ignore]
    public long TotalPoints
    {
        get
        {
            if (ExerciseQuestions is null)
            {
                return 0;
            }

            return ExerciseQuestions.Sum(x => x.PointsEarned);
        }
    }

    [Ignore]
    public double? AverageResponseTime
    {
        get
        {
            if (ExerciseQuestions is null)
            {
                return 0;
            }

            return ExerciseQuestions.Where(x => x.ResponseTime is not null).Average(x => x.ResponseTime);
        }
    }

    [Ignore]
    public List<ExerciseNumber>? ExercisedNumbers { get; set; }

    [Ignore]
    public List<ExerciseQuestion>? ExerciseQuestions { get; set; }
}

public class ExerciseNumber
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    [Indexed]
    public int ExerciseId { get; set; }

    public int Number { get; set; }
}

public class ExerciseQuestion
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    [Indexed]
    public int ExerciseId { get; set; }

    public int Number1 { get; set; }

    public int Number2 { get; set; }

    public int? Answer { get; set; }

    public long? DisplayTime { get; set; }
    
    public long? ResponseTime { get; set; }

    public long PointsEarned { get; set; }

    [Ignore]
    public bool IsCorrectAnswer
    {
        get
        {
            if (Answer is null)
            {
                return false;
            }

            return Answer.Value == Number1 * Number2;
        }
    }

    [Ignore] public bool IsSkipped => !Answer.HasValue && DisplayTime.HasValue;

    [Ignore] public bool IsDisplayed => DisplayTime.HasValue;
}