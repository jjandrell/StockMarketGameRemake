using System;
using System.Collections.Generic;

namespace StockMarketGame.Core.Models
{
    /// <summary>
    /// Represents a stock in the game
    /// </summary>
    public class Stock
    {
        /// <summary>
        /// Unique identifier for the stock
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Name of the company
        /// </summary>
        public string CompanyName { get; set; }

        /// <summary>
        /// Stock symbol/ticker
        /// </summary>
        public string Symbol { get; set; }

        /// <summary>
        /// Current price per share
        /// </summary>
        public decimal CurrentPrice { get; set; }

        /// <summary>
        /// Previous turn's price per share
        /// </summary>
        public decimal LastPrice { get; set; }

        /// <summary>
        /// Total shares available for this company (initially 1,000,000 in original game)
        /// </summary>
        public int TotalShares { get; set; } = 1000000;

        /// <summary>
        /// Remaining shares available to purchase
        /// </summary>
        public int RemainingShares { get; set; } = 1000000;

        /// <summary>
        /// Price history for graphing (price at end of each turn)
        /// </summary>
        public List<decimal> PriceHistory { get; set; } = new List<decimal>();

        /// <summary>
        /// True if the company is bankrupt
        /// </summary>
        public bool IsBankrupt { get; set; }

        /// <summary>
        /// Brief description of the company
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Industry sector this stock belongs to
        /// </summary>
        public string Sector { get; set; }

        /// <summary>
        /// Calculate the change in price from last turn
        /// </summary>
        public decimal PriceChange => CurrentPrice - LastPrice;

        /// <summary>
        /// Calculate the percentage change from last turn
        /// </summary>
        public decimal PercentageChange => LastPrice != 0 ? (PriceChange / LastPrice) * 100 : 0;

        /// <summary>
        /// Checks if stock qualifies for a split (>=140 in original game)
        /// </summary>
        public bool QualifiesForSplit => CurrentPrice >= 140;

        /// <summary>
        /// Checks if stock qualifies for dividend (>10 in original game)
        /// </summary>
        public bool QualifiesForDividend => CurrentPrice > 10;

        /// <summary>
        /// Update stock price based on market conditions and events
        /// </summary>
        /// <param name="baseChange">Base random change</param>
        /// <param name="eventChange">Change from market events</param>
        /// <param name="isBullMarket">Whether the market is a bull market</param>
        public void UpdatePrice(decimal baseChange, decimal eventChange, bool isBullMarket)
        {
            LastPrice = CurrentPrice;
            
            // Apply the base change (random fluctuation)
            CurrentPrice += baseChange;
            
            // Apply event-based change
            CurrentPrice += eventChange;
            
            // Ensure price doesn't go below 1
            if (CurrentPrice < 1)
            {
                // Chance of bankruptcy if price is too low
                if (CurrentPrice < 0.5m && new Random().Next(100) < 30)
                {
                    IsBankrupt = true;
                    CurrentPrice = 0;
                }
                else
                {
                    CurrentPrice = 1;
                }
            }
            
            // Record the price in history
            PriceHistory.Add(CurrentPrice);
        }

        /// <summary>
        /// Split the stock according to the rules (price divided by 2, shares multiplied)
        /// </summary>
        /// <returns>The split ratio (2, 3, or 4 for 1)</returns>
        public int Split()
        {
            if (!QualifiesForSplit)
                return 0;

            // Determine split ratio randomly (2:1, 3:1, or 4:1 as in original game)
            var random = new Random();
            int splitRatio = random.Next(2, 5); // 2, 3, or 4
            
            // Apply split - price is always divided by 2 regardless of split ratio
            LastPrice = CurrentPrice;
            CurrentPrice /= 2;
            
            // Total and remaining shares are multiplied by the split ratio
            TotalShares *= splitRatio;
            RemainingShares *= splitRatio;
            
            return splitRatio;
        }

        /// <summary>
        /// Calculate dividend payment for a player holding specific number of shares
        /// </summary>
        /// <param name="sharesHeld">Number of shares held by the player</param>
        /// <returns>Dividend amount to be paid</returns>
        public decimal CalculateDividend(int sharesHeld)
        {
            if (!QualifiesForDividend)
                return 0;
                
            // Simple dividend calculation - could be enhanced with more factors
            decimal dividendPerShare = (CurrentPrice * 0.01m); // 1% dividend yield
            return Math.Round(dividendPerShare * sharesHeld, 2);
        }
    }
}
