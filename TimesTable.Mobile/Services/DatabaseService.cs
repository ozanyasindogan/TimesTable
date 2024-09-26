using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;
using TimesTable.Mobile.Models;

namespace TimesTable.Mobile.Services;

public class DatabaseService
{
    private SQLiteAsyncConnection? _connection;
    private readonly string _databasePath = Path.Combine(FileSystem.AppDataDirectory, "timestable.db3");
    private const SQLiteOpenFlags OpenFlags = SQLite.SQLiteOpenFlags.ReadWrite | SQLite.SQLiteOpenFlags.Create | SQLiteOpenFlags.SharedCache;

    public async Task<bool> InitializeDatabaseAsync()
    {
        try
        {
            if (_connection is not null)
            {
                return true;
            }

            _connection = new SQLiteAsyncConnection(_databasePath, OpenFlags);
            await _connection.CreateTablesAsync(CreateFlags.None, typeof(Exercise), typeof(ExerciseNumber),
                typeof(ExerciseQuestion));

            #if DEBUG
            // ************************************ remove
            await _connection.DeleteAllAsync<ExerciseQuestion>();
            await _connection.DeleteAllAsync<ExerciseNumber>();
            await _connection.DeleteAllAsync<Exercise>();
            #endif

            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<Exercise?> StartNewExerciseAsync(List<int> numbers, long givenTimeForQuestions, int repeatCount)
    {
        if (!await InitializeDatabaseAsync())
        {
            return null;
        }

        var exercise = new Exercise
        {
            GivenTimePerQuestion = givenTimeForQuestions
        };

        if (await _connection!.InsertAsync(exercise) == 0)
        {
            return null;
        }

        exercise.ExercisedNumbers = new List<ExerciseNumber>(numbers.Count);

        foreach (var number in numbers)
        {
            var exerciseNumber = new ExerciseNumber()
            {
                Number = number,
                ExerciseId = exercise.Id
            };

            await _connection.InsertAsync(exerciseNumber);
            exercise.ExercisedNumbers.Add(exerciseNumber);
        }

        var totalQuestions = numbers.Count * repeatCount * 10;

        var numberPairs = new List<(int, int)>(totalQuestions);

        for (var i = 0; i < repeatCount; i++)
        {
            foreach (var number in numbers)
            {
                for (var n = 1; n < 11; n++)
                {
                    numberPairs.Add((number, n));
                }
            }
        }

        var count = numberPairs.Count;
        exercise.ExerciseQuestions = new List<ExerciseQuestion>(count);

        while (count > 0)
        {
            var index = Random.Shared.Next(0, numberPairs.Count);
            var numberPair = numberPairs[index];
            
            var question = new ExerciseQuestion()
            {
                ExerciseId = exercise.Id,
                Number1 = numberPair.Item1,
                Number2 = numberPair.Item2,
            };

            await _connection.InsertAsync(question);
            exercise.ExerciseQuestions.Add(question);
            numberPairs.RemoveAt(index);

            count--;
        }

        exercise.StartTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        await _connection.UpdateAsync(exercise);

        return exercise;
    }
}