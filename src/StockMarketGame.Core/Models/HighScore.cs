using System;

namespace StockMarketGame.Core.Models
{
    /// <summary>
    /// Represents a high score entry
    /// </summary>
    public class HighScore
    {
        /// <summary>
        /// Unique identifier for the high score
        /// </summary>
        public Guid Id { get; set; }
        
        /// <summary>
        /// Name of the player
        /// </summary>
        public string PlayerName { get; set; }
        
        /// <summary>
        /// Final score achieved
        /// </summary>
        public decimal Score { get; set; }
        
        /// <summary>
        /// Date and time when the score was achieved
        /// </summary>
        public DateTime Date { get; set; }
        
        /// <summary>
        /// Optional game difficulty/mode
        /// </summary>
        public string GameMode { get; set; }
        
        /// <summary>
        /// Constructor for a new high score
        /// </summary>
        public HighScore()
        {
            Id = Guid.NewGuid();
        }
        
        /// <summary>
        /// Format the score for display
        /// </summary>
        /// <returns>Formatted score string</returns>
        public string FormatScore()
        {
            return Score.ToString("C0");
        }
        
        /// <summary>
        /// Get a short description of the high score achievement
        /// </summary>
        /// <returns>Description string</returns>
        public string GetDescription()
        {
            string achievement;
            
            if (Score >= 10000000)
                achievement = "Stock Market Legend";
            else if (Score >= 5000000)
                achievement = "Wall Street Wizard";
            else if (Score >= 1000000)
                achievement = "Market Millionaire";
            else if (Score >= 500000)
                achievement = "Savvy Investor";
            else if (Score >= 250000)
                achievement = "Market Player";
            else if (Score >= 100000)
                achievement = "Break Even";
            else
                achievement = "Market Novice";
                
            return $"{PlayerName}: {FormatScore()} - {achievement}";
        }
    }
}
