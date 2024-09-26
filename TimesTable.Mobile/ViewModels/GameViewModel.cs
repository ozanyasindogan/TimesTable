using System.Timers;
using AndroidX.Emoji2.Text.FlatBuffer;
using AndroidX.Fragment.App.StrictMode;
using TimesTable.Mobile.Models;
using TimesTable.Mobile.Services;
using Timer = System.Timers.Timer;

namespace TimesTable.Mobile.ViewModels;

public partial class GameViewModel : BaseViewModel
{
    [ObservableProperty]
    private bool _isGameRunning;

    [ObservableProperty] 
    private bool _showSetupScreen = true;

    [ObservableProperty] 
    private Exercise? _currentExercise;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CurrentQuestionNumber))]
    private int _currentQuestionIndex;

    public int CurrentQuestionNumber => CurrentQuestionIndex + 1;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(GiveAnswerAndProceedCommand))]
    private string? _currentResponse;

    [ObservableProperty] 
    private int _totalQuestions;

    [ObservableProperty] 
    private int _currentQuestionNumber1;

    [ObservableProperty]
    private int _currentQuestionNumber2;

    [ObservableProperty] 
    private ObservableCollection<DifficultyLevel> _difficultyLevels =
    [
        new DifficultyLevel(30000),
        new DifficultyLevel(20000),
        new DifficultyLevel(15000),
        new DifficultyLevel(10000),
        new DifficultyLevel(7000),
        new DifficultyLevel(5000),
        new DifficultyLevel(3000)
    ];

    [ObservableProperty]
    private int? _repeatCount = 1;

    [NotifyCanExecuteChangedFor(nameof(StartGameCommand))]
    [ObservableProperty] 
    private DifficultyLevel? _selectedDifficultyLevel;

    [NotifyCanExecuteChangedFor(nameof(StartGameCommand))]
    [ObservableProperty]
    private bool _tableTwo;

    [NotifyCanExecuteChangedFor(nameof(StartGameCommand))]
    [ObservableProperty]
    private bool _tableThree;

    [NotifyCanExecuteChangedFor(nameof(StartGameCommand))]
    [ObservableProperty]
    private bool _tableFour;

    [NotifyCanExecuteChangedFor(nameof(StartGameCommand))]
    [ObservableProperty]
    private bool _tableFive;

    [NotifyCanExecuteChangedFor(nameof(StartGameCommand))]
    [ObservableProperty]
    private bool _tableSix;

    [NotifyCanExecuteChangedFor(nameof(StartGameCommand))]
    [ObservableProperty]
    private bool _tableSeven;

    [NotifyCanExecuteChangedFor(nameof(StartGameCommand))]
    [ObservableProperty]
    private bool _tableEight;

    [NotifyCanExecuteChangedFor(nameof(StartGameCommand))]
    [ObservableProperty]
    private bool _tableNine;

    [ObservableProperty] 
    private long _currentRemainingTime;

    [ObservableProperty]
    private double _currentProgressValue;

    private bool CanGiveAnswerAndProceed => GetCurrentResponse() != 0;

    private readonly DatabaseService _databaseService;

    private readonly System.Timers.Timer _timer;

    public bool AllTables
    {
        get => TableTwo && TableThree && TableFour && TableFive && TableSix && TableSeven && TableEight && TableNine;
        set
        {
            TableTwo = value;
            TableThree = value;
            TableFour = value;
            TableFive = value;
            TableSix = value;
            TableSeven = value;
            TableEight = value;
            TableNine = value;
        }
    }

    private bool CanStartGame()
    {
        if (SelectedDifficultyLevel is null)
        {
            return false;
        }

        return TableTwo || TableThree || TableFour || TableFive || TableSix || TableSeven || TableEight || TableNine;
    }

    public GameViewModel(DatabaseService databaseService)
    {
        _databaseService = databaseService;
        SelectedDifficultyLevel = DifficultyLevels[0];
        AllTables = true;
        
        _timer = new Timer(100);
        _timer.AutoReset = true;
        _timer.Elapsed += TimerOnElapsed;
        _timer.Enabled = false;
    }

    private void TimerOnElapsed(object? sender, ElapsedEventArgs e)
    {
        CurrentRemainingTime = CurrentRemainingTime - 100;
        var givenTime = CurrentExercise?.GivenTimePerQuestion ?? 30000;

        if (CurrentRemainingTime <= 0)
        {
            // go to next question
            CurrentRemainingTime = givenTime;
        }

        CurrentProgressValue = (double)CurrentRemainingTime / (double)givenTime;
    }

    [RelayCommand]
    private void ToggleTables()
    {
        AllTables = !AllTables;
    }

    [RelayCommand(CanExecute = nameof(CanStartGame))]
    private async Task StartGameAsync()
    {
        var numbers = new List<int>(10);

        if (TableTwo) numbers.Add(2);
        if (TableThree) numbers.Add(3);
        if (TableFour) numbers.Add(4);
        if (TableFive) numbers.Add(5);
        if (TableSix) numbers.Add(6);
        if (TableSeven) numbers.Add(7);
        if (TableEight) numbers.Add(8);
        if (TableNine) numbers.Add(9);

        CurrentExercise = await _databaseService.StartNewExerciseAsync(numbers, SelectedDifficultyLevel!.GivenTimePerQuestion,
            RepeatCount!.Value);

        if (CurrentExercise is null)
        {
            return;
        }

        TotalQuestions = CurrentExercise.TotalQuestions;
        
        ShowSetupScreen = false;
        IsGameRunning = true;
        
        LoadQuestion(0);
    }

    [RelayCommand(CanExecute = nameof(CanGiveAnswerAndProceed))]
    private void GiveAnswerAndProceed()
    {
        var answer = GetCurrentResponse();

        var question = CurrentExercise?.ExerciseQuestions?[CurrentQuestionIndex];
        
        if (question is null)
        {
            return;
        }

        
    }

    private void LoadQuestion(int index)
    {
        _timer.Enabled = false;

        if (CurrentExercise?.ExerciseQuestions is null || index >= TotalQuestions)
        {
            return;
        }
        
        var question = CurrentExercise.ExerciseQuestions[index];
        
        CurrentResponse = null;
        CurrentQuestionIndex = index;
        CurrentQuestionNumber1 = question.Number1;
        CurrentQuestionNumber2 = question.Number2;

        question.DisplayTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        CurrentRemainingTime = CurrentExercise.GivenTimePerQuestion;

        _timer.Enabled = true;
    }

    private int GetCurrentResponse()
    {
        if (CurrentResponse is null)
        {
            return 0;
        }

        return int.TryParse(CurrentResponse, out var result) ? result : 0;
    }
}
