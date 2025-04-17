using System;
using System.Collections.Generic;
using System.Linq;

namespace StockMarketGame.Core.Models
{
    /// <summary>
    /// Market condition types
    /// </summary>
    public enum MarketCondition
    {
        Bull, // Prices can fluctuate more (8-38 points in original game)
        Bear  // Prices fluctuate less (5-19 points in original game)
    }
    
    /// <summary>
    /// Represents the overall game state and logic
    /// </summary>
    public class Game
    {
        /// <summary>
        /// Unique identifier for the game
        /// </summary>
        public Guid Id { get; set; }
        
        /// <summary>
        /// When the game was created
        /// </summary>
        public DateTime CreatedAt { get; set; }
        
        /// <summary>
        /// When the game was last updated
        /// </summary>
        public DateTime LastUpdatedAt { get; set; }
        
        /// <summary>
        /// List of players in the game
        /// </summary>
        public List<Player> Players { get; set; } = new List<Player>();
        
        /// <summary>
        /// List of stocks in the game
        /// </summary>
        public List<Stock> Stocks { get; set; } = new List<Stock>();
        
        /// <summary>
        /// Current market condition
        /// </summary>
        public MarketCondition CurrentMarketCondition { get; set; }
        
        /// <summary>
        /// Current turn number (1-10 in original game)
        /// </summary>
        public int CurrentTurn { get; set; } = 0;
        
        /// <summary>
        /// Total number of turns in the game
        /// </summary>
        public int TotalTurns { get; set; } = 10;
        
        /// <summary>
        /// Index of the current player taking their turn
        /// </summary>
        public int CurrentPlayerIndex { get; set; } = -1;
        
        /// <summary>
        /// Random order of players for the current turn
        /// </summary>
        public List<int> PlayerOrder { get; set; } = new List<int>();
        
        /// <summary>
        /// Current bank interest rate
        /// </summary>
        public decimal BankInterestRate { get; set; } = 10.0m;
        
        /// <summary>
        /// History of market news events
        /// </summary>
        public List<MarketEvent> MarketEvents { get; set; } = new List<MarketEvent>();
        
        /// <summary>
        /// High scores from this and previous games
        /// </summary>
        public List<HighScore> HighScores { get; set; } = new List<HighScore>();
        
        /// <summary>
        /// Whether to suppress beeps and reduce waits (for experienced players)
        /// </summary>
        public bool FastMode { get; set; }

        /// <summary>
        /// Constructor for a new game
        /// </summary>
        public Game()
        {
            Id = Guid.NewGuid();
            CreatedAt = DateTime.Now;
            LastUpdatedAt = DateTime.Now;
            
            // Default to bull market to start
            CurrentMarketCondition = MarketCondition.Bull;
        }

        /// <summary>
        /// Set up a new game with players
        /// </summary>
        /// <param name="playerNames">List of player names</param>
        /// <param name="startingCash">Starting cash for players</param>
        /// <param name="fastMode">Whether to run in fast mode</param>
        public void SetupGame(List<string> playerNames, decimal startingCash = 100000, bool fastMode = false)
        {
            // Clear existing data
            Players.Clear();
            CurrentTurn = 0;
            CurrentPlayerIndex = -1;
            PlayerOrder.Clear();
            FastMode = fastMode;
            
            // Add players
            foreach (var name in playerNames)
            {
                Players.Add(new Player(name, startingCash));
            }
            
            // Generate initial stocks if none exist
            if (Stocks.Count == 0)
            {
                GenerateStocks();
            }
        }

        /// <summary>
        /// Generate a set of stocks for the game
        /// </summary>
        /// <param name="count">Number of stocks to generate</param>
        private void GenerateStocks(int count = 12)
        {
            Stocks.Clear();
            
            // Example stock data (this would be expanded or loaded from a data file)
            var stockData = new[]
            {
                new { Name = "TechGiant", Symbol = "TCHN", Sector = "Technology", BasePrice = 55m },
                new { Name = "GlobalBank", Symbol = "GBNK", Sector = "Finance", BasePrice = 78m },
                new { Name = "AutoFuture", Symbol = "AUTF", Sector = "Automotive", BasePrice = 42m },
                new { Name = "EnergyCore", Symbol = "NRGY", Sector = "Energy", BasePrice = 36m },
                new { Name = "RetailKing", Symbol = "RTKN", Sector = "Retail", BasePrice = 28m },
                new { Name = "FoodGlobal", Symbol = "FDGL", Sector = "Consumer Goods", BasePrice = 60m },
                new { Name = "PharmaPlus", Symbol = "PHRM", Sector = "Healthcare", BasePrice = 85m },
                new { Name = "AeroSpace", Symbol = "ARSP", Sector = "Aerospace", BasePrice = 95m },
                new { Name = "MediaMax", Symbol = "MEDX", Sector = "Entertainment", BasePrice = 32m },
                new { Name = "ConstructAll", Symbol = "CSTR", Sector = "Construction", BasePrice = 45m },
                new { Name = "MiningPro", Symbol = "MING", Sector = "Mining", BasePrice = 38m },
                new { Name = "TeleComm", Symbol = "TELC", Sector = "Telecommunications", BasePrice = 52m }
            };
            
            var random = new Random();
            
            // Ensure we don't try to create more stocks than we have data for
            count = Math.Min(count, stockData.Length);
            
            for (int i = 0; i < count; i++)
            {
                var data = stockData[i];
                
                // Add some randomness to starting price
                decimal price = data.BasePrice * (1 + (decimal)(random.NextDouble() * 0.4 - 0.2)); // ±20%
                
                var stock = new Stock
                {
                    Id = Guid.NewGuid(),
                    CompanyName = data.Name,
                    Symbol = data.Symbol,
                    CurrentPrice = Math.Round(price, 2),
                    LastPrice = Math.Round(price, 2), // Start with same last price
                    Sector = data.Sector,
                    Description = $"A leading company in the {data.Sector} sector.",
                    TotalShares = 1000000,
                    RemainingShares = 1000000
                };
                
                // Initialize price history with starting price
                stock.PriceHistory.Add(stock.CurrentPrice);
                
                Stocks.Add(stock);
            }
        }

        /// <summary>
        /// Start a new turn
        /// </summary>
        /// <returns>True if a new turn was started, false if game is over</returns>
        public bool StartNewTurn()
        {
            if (CurrentTurn >= TotalTurns)
                return false;
                
            CurrentTurn++;
            
            // Randomize player order
            PlayerOrder = Enumerable.Range(0, Players.Count).ToList();
            Shuffle(PlayerOrder);
            
            // Reset current player index
            CurrentPlayerIndex = -1;
            
            // Determine market condition for this turn
            DetermineMarketCondition();
            
            // Generate market events for this turn
            GenerateMarketEvents();
            
            // Calculate interest on loans
            foreach (var player in Players)
            {
                player.ApplyLoanInterest();
            }
            
            return true;
        }

        /// <summary>
        /// Move to the next player's turn
        /// </summary>
        /// <returns>True if moved to next player, false if turn is complete</returns>
        public bool NextPlayer()
        {
            CurrentPlayerIndex++;
            
            if (CurrentPlayerIndex >= Players.Count)
                return false;
                
            return true;
        }

        /// <summary>
        /// Get the current player
        /// </summary>
        /// <returns>Current player or null if no current player</returns>
        public Player GetCurrentPlayer()
        {
            if (CurrentPlayerIndex < 0 || CurrentPlayerIndex >= Players.Count)
                return null;
                
            return Players[PlayerOrder[CurrentPlayerIndex]];
        }

        /// <summary>
        /// Apply market changes at the end of a turn
        /// </summary>
        public void ApplyMarketChanges()
        {
            var random = new Random();
            
            // Process each stock
            foreach (var stock in Stocks)
            {
                if (stock.IsBankrupt)
                    continue;
                    
                // Base random change depends on market condition
                decimal baseChange;
                if (CurrentMarketCondition == MarketCondition.Bull)
                {
                    // 8-38 points in original game
                    baseChange = random.Next(8, 39) * (random.Next(2) == 0 ? -1 : 1);
                }
                else
                {
                    // 5-19 points in original game
                    baseChange = random.Next(5, 20) * (random.Next(2) == 0 ? -1 : 1);
                }
                
                // Get event-based changes for this stock
                decimal eventChange = 0;
                foreach (var marketEvent in MarketEvents.Where(e => e.Turn == CurrentTurn))
                {
                    if (marketEvent.AffectedStockIds.Contains(stock.Id))
                    {
                        eventChange += marketEvent.PriceImpact;
                    }
                }
                
                // Update the stock price
                stock.UpdatePrice(baseChange, eventChange, CurrentMarketCondition == MarketCondition.Bull);
            }
            
            // Process stock splits
            foreach (var stock in Stocks)
            {
                if (stock.QualifiesForSplit)
                {
                    stock.Split();
                }
            }
            
            // Process dividends (1-3 companies per turn that are > 10 points)
            var eligibleForDividends = Stocks.Where(s => s.QualifiesForDividend).ToList();
            if (eligibleForDividends.Any())
            {
                int dividendCount = Math.Min(random.Next(1, 4), eligibleForDividends.Count);
                var dividendStocks = eligibleForDividends.OrderBy(_ => random.Next()).Take(dividendCount).ToList();
                
                foreach (var player in Players)
                {
                    player.ReceiveDividends(dividendStocks);
                }
            }
            
            // Handle bankruptcies
            foreach (var player in Players)
            {
                player.HandleBankruptcies(Stocks);
            }
            
            // Calculate and store player assets for history
            var stockPrices = Stocks.ToDictionary(s => s.Id, s => s.CurrentPrice);
            foreach (var player in Players)
            {
                player.RecordAssetsHistory(stockPrices);
            }
        }

        /// <summary>
        /// End the game and calculate final scores
        /// </summary>
        public void EndGame()
        {
            CurrentTurn = TotalTurns;
            
            // Calculate final scores
            var stockPrices = Stocks.ToDictionary(s => s.Id, s => s.CurrentPrice);
            
            foreach (var player in Players)
            {
                player.FinalScore = player.CalculateTotalAssets(stockPrices);
            }
            
            // Assign rankings
            var rankedPlayers = Players.OrderByDescending(p => p.FinalScore).ToList();
            for (int i = 0; i < rankedPlayers.Count; i++)
            {
                rankedPlayers[i].Ranking = i + 1;
            }
            
            // Add to high scores
            foreach (var player in Players.Where(p => !p.IsAI))
            {
                HighScores.Add(new HighScore
                {
                    PlayerName = player.Name,
                    Score = player.FinalScore,
                    Date = DateTime.Now
                });
            }
            
            // Sort high scores
            HighScores = HighScores.OrderByDescending(h => h.Score).Take(12).ToList();
            
            LastUpdatedAt = DateTime.Now;
        }

        /// <summary>
        /// Randomly determine market condition for a turn
        /// </summary>
        private void DetermineMarketCondition()
        {
            var random = new Random();
            
            // 60% chance of bull market, 40% chance of bear market
            CurrentMarketCondition = random.NextDouble() < 0.6 ? 
                MarketCondition.Bull : MarketCondition.Bear;
        }

        /// <summary>
        /// Generate market events for the current turn
        /// </summary>
        private void GenerateMarketEvents()
        {
            var random = new Random();
            
            // Number of events depends on turn number and randomness
            int eventCount = Math.Min(random.Next(1, 4), Stocks.Count / 2);
            
            for (int i = 0; i < eventCount; i++)
            {
                // Select random stocks to be affected
                var affectedStockIds = Stocks
                    .OrderBy(_ => random.Next())
                    .Take(random.Next(1, 4))
                    .Select(s => s.Id)
                    .ToList();
                
                // Determine impact magnitude based on market condition
                decimal impact;
                if (CurrentMarketCondition == MarketCondition.Bull)
                {
                    impact = (decimal)(random.Next(5, 16) * (random.NextDouble() < 0.6 ? 1 : -1));
                }
                else
                {
                    impact = (decimal)(random.Next(3, 11) * (random.NextDouble() < 0.5 ? 1 : -1));
                }
                
                // Create a simple event description
                string eventText = GenerateEventText(impact, affectedStockIds);
                
                MarketEvents.Add(new MarketEvent
                {
                    Id = Guid.NewGuid(),
                    Turn = CurrentTurn,
                    EventText = eventText,
                    PriceImpact = impact,
                    AffectedStockIds = affectedStockIds
                });
            }
        }

        /// <summary>
        /// Generate a description for a market event
        /// </summary>
        /// <param name="impact">Price impact</param>
        /// <param name="affectedStockIds">Affected stock IDs</param>
        /// <returns>Event description</returns>
        private string GenerateEventText(decimal impact, List<Guid> affectedStockIds)
        {
            // This would be replaced with more sophisticated text generation (possibly AI-driven)
            string direction = impact > 0 ? "rises" : "falls";
            string magnitude = Math.Abs(impact) > 10 ? "sharply" : "slightly";
            
            var affectedCompanies = affectedStockIds
                .Select(id => Stocks.First(s => s.Id == id).CompanyName)
                .ToList();
            
            string companies = string.Join(", ", affectedCompanies);
            
            return $"{companies} stock {direction} {magnitude} on market news.";
        }

        /// <summary>
        /// Shuffle a list using Fisher-Yates algorithm
        /// </summary>
        /// <typeparam name="T">Type of list items</typeparam>
        /// <param name="list">List to shuffle</param>
        private void Shuffle<T>(List<T> list)
        {
            var random = new Random();
            int n = list.Count;
            
            while (n > 1)
            {
                n--;
                int k = random.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
    }
}
