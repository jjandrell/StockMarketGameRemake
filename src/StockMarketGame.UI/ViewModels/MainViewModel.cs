using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using StockMarketGame.Core.Models;

namespace StockMarketGame.UI.ViewModels
{
    /// <summary>
    /// Main view model for the application
    /// </summary>
    public class MainViewModel : INotifyPropertyChanged
    {
        private Game _currentGame;
        private bool _isGameInProgress;
        private string _statusMessage;
        private bool _isBusy;

        /// <summary>
        /// Current game being played
        /// </summary>
        public Game CurrentGame
        {
            get => _currentGame;
            set
            {
                _currentGame = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsGameInProgress));
            }
        }

        /// <summary>
        /// Indicates if a game is currently in progress
        /// </summary>
        public bool IsGameInProgress
        {
            get => _isGameInProgress;
            set
            {
                _isGameInProgress = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Status message to display to the user
        /// </summary>
        public string StatusMessage
        {
            get => _statusMessage;
            set
            {
                _statusMessage = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Indicates if the application is currently busy
        /// </summary>
        public bool IsBusy
        {
            get => _isBusy;
            set
            {
                _isBusy = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Collection of available saved games
        /// </summary>
        public ObservableCollection<GameData> SavedGames { get; } = new ObservableCollection<GameData>();

        /// <summary>
        /// Collection of high scores
        /// </summary>
        public ObservableCollection<HighScore> HighScores { get; } = new ObservableCollection<HighScore>();

        /// <summary>
        /// Command to start a new game
        /// </summary>
        public ICommand NewGameCommand { get; }

        /// <summary>
        /// Command to load a saved game
        /// </summary>
        public ICommand LoadGameCommand { get; }

        /// <summary>
        /// Command to save the current game
        /// </summary>
        public ICommand SaveGameCommand { get; }

        /// <summary>
        /// Command to exit the application
        /// </summary>
        public ICommand ExitCommand { get; }

        /// <summary>
        /// Command to show high scores
        /// </summary>
        public ICommand ShowHighScoresCommand { get; }

        /// <summary>
        /// Command to show settings
        /// </summary>
        public ICommand ShowSettingsCommand { get; }

        /// <summary>
        /// Constructor
        /// </summary>
        public MainViewModel()
        {
            // Initialize commands
            NewGameCommand = new RelayCommand(ExecuteNewGame, CanExecuteNewGame);
            LoadGameCommand = new RelayCommand<GameData>(ExecuteLoadGame, CanExecuteLoadGame);
            SaveGameCommand = new RelayCommand(ExecuteSaveGame, CanExecuteSaveGame);
            ExitCommand = new RelayCommand(ExecuteExit);
            ShowHighScoresCommand = new RelayCommand(ExecuteShowHighScores);
            ShowSettingsCommand = new RelayCommand(ExecuteShowSettings);

            // Initialize properties
            IsGameInProgress = false;
            StatusMessage = "Welcome to the Stock Market Game Remake!";

            // Load saved games and high scores
            LoadSavedGamesAsync();
            LoadHighScoresAsync();
        }

        /// <summary>
        /// Load saved games from the database
        /// </summary>
        private async Task LoadSavedGamesAsync()
        {
            try
            {
                IsBusy = true;
                StatusMessage = "Loading saved games...";

                // TODO: Implement actual data loading
                await Task.Delay(100); // Simulate loading
                
                // For now, just add some dummy data
                SavedGames.Clear();
                SavedGames.Add(new GameData { Id = Guid.NewGuid(), Name = "Example Game 1", CreatedAt = DateTime.Now.AddDays(-2) });
                SavedGames.Add(new GameData { Id = Guid.NewGuid(), Name = "Example Game 2", CreatedAt = DateTime.Now.AddDays(-1) });

                StatusMessage = "Saved games loaded successfully.";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error loading saved games: {ex.Message}";
            }
            finally
            {
                IsBusy = false;
            }
        }

        /// <summary>
        /// Load high scores from the database
        /// </summary>
        private async Task LoadHighScoresAsync()
        {
            try
            {
                IsBusy = true;
                StatusMessage = "Loading high scores...";

                // TODO: Implement actual data loading
                await Task.Delay(100); // Simulate loading
                
                // For now, just add some dummy data
                HighScores.Clear();
                HighScores.Add(new HighScore { PlayerName = "Wall Street Wizard", Score = 5500000, Date = DateTime.Now.AddDays(-10) });
                HighScores.Add(new HighScore { PlayerName = "Stock Master", Score = 3200000, Date = DateTime.Now.AddDays(-5) });
                HighScores.Add(new HighScore { PlayerName = "Investment Pro", Score = 1800000, Date = DateTime.Now.AddDays(-2) });

                StatusMessage = "High scores loaded successfully.";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error loading high scores: {ex.Message}";
            }
            finally
            {
                IsBusy = false;
            }
        }

        /// <summary>
        /// Execute new game command
        /// </summary>
        private void ExecuteNewGame()
        {
            // TODO: Show new game setup dialog
            StatusMessage = "Starting new game...";
            
            // For now, just create a new game with default settings
            CurrentGame = new Game();
            
            // Add a couple of players for testing
            CurrentGame.SetupGame(new System.Collections.Generic.List<string> { "Player 1", "AI Opponent" });
            CurrentGame.Players[1].IsAI = true;
            
            IsGameInProgress = true;
            CurrentGame.StartNewTurn(); // Start first turn
            
            StatusMessage = "New game started. It's Player 1's turn.";
        }

        /// <summary>
        /// Determine if new game command can execute
        /// </summary>
        private bool CanExecuteNewGame()
        {
            return !IsBusy;
        }

        /// <summary>
        /// Execute load game command
        /// </summary>
        private void ExecuteLoadGame(GameData gameData)
        {
            if (gameData == null)
                return;
                
            StatusMessage = $"Loading game '{gameData.Name}'...";
            
            // TODO: Implement actual game loading
            // For now, just create a dummy game
            CurrentGame = new Game
            {
                Id = gameData.Id,
                CreatedAt = gameData.CreatedAt,
                LastUpdatedAt = DateTime.Now
            };
            
            // Add a couple of players for testing
            CurrentGame.SetupGame(new System.Collections.Generic.List<string> { "Player 1", "Player 2" });
            CurrentGame.CurrentTurn = 3; // Set to mid-game
            
            IsGameInProgress = true;
            StatusMessage = $"Game '{gameData.Name}' loaded successfully.";
        }

        /// <summary>
        /// Determine if load game command can execute
        /// </summary>
        private bool CanExecuteLoadGame(GameData gameData)
        {
            return !IsBusy && gameData != null;
        }

        /// <summary>
        /// Execute save game command
        /// </summary>
        private void ExecuteSaveGame()
        {
            // TODO: Show save game dialog
            StatusMessage = "Saving game...";
            
            // TODO: Implement actual game saving
            StatusMessage = "Game saved successfully.";
        }

        /// <summary>
        /// Determine if save game command can execute
        /// </summary>
        private bool CanExecuteSaveGame()
        {
            return !IsBusy && IsGameInProgress;
        }

        /// <summary>
        /// Execute exit command
        /// </summary>
        private void ExecuteExit()
        {
            // TODO: Prompt to save if game in progress
            StatusMessage = "Exiting application...";
            
            // TODO: Implement application exit
        }

        /// <summary>
        /// Execute show high scores command
        /// </summary>
        private void ExecuteShowHighScores()
        {
            // TODO: Show high scores dialog
            StatusMessage = "Showing high scores...";
        }

        /// <summary>
        /// Execute show settings command
        /// </summary>
        private void ExecuteShowSettings()
        {
            // TODO: Show settings dialog
            StatusMessage = "Showing settings...";
        }

        /// <summary>
        /// Property changed event
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Raise property changed event
        /// </summary>
        /// <param name="propertyName">Name of the property that changed</param>
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    /// <summary>
    /// Simple relay command implementation
    /// </summary>
    public class RelayCommand : ICommand
    {
        private readonly Action _execute;
        private readonly Func<bool> _canExecute;

        public RelayCommand(Action execute, Func<bool> canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute?.Invoke() ?? true;
        }

        public void Execute(object parameter)
        {
            _execute();
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
    }

    /// <summary>
    /// Relay command with parameter
    /// </summary>
    public class RelayCommand<T> : ICommand
    {
        private readonly Action<T> _execute;
        private readonly Predicate<T> _canExecute;

        public RelayCommand(Action<T> execute, Predicate<T> canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute?.Invoke((T)parameter) ?? true;
        }

        public void Execute(object parameter)
        {
            _execute((T)parameter);
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
    }
}
