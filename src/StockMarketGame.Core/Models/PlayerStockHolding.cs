using System;

namespace StockMarketGame.Core.Models
{
    /// <summary>
    /// Represents a player's holding of a specific stock
    /// </summary>
    public class PlayerStockHolding
    {
        /// <summary>
        /// ID of the stock
        /// </summary>
        public Guid StockId { get; set; }
        
        /// <summary>
        /// Stock symbol for quick reference
        /// </summary>
        public string Symbol { get; set; }
        
        /// <summary>
        /// Company name for display
        /// </summary>
        public string CompanyName { get; set; }
        
        /// <summary>
        /// Number of shares owned
        /// </summary>
        public int Shares { get; set; }
        
        /// <summary>
        /// Average cost basis per share
        /// </summary>
        public decimal AverageCost { get; set; }
        
        /// <summary>
        /// Calculate the total cost basis of this holding
        /// </summary>
        public decimal TotalCost => AverageCost * Shares;
        
        /// <summary>
        /// Calculate the current value of this holding
        /// </summary>
        /// <param name="currentPrice">Current market price per share</param>
        /// <returns>Total current value</returns>
        public decimal CurrentValue(decimal currentPrice) => currentPrice * Shares;
        
        /// <summary>
        /// Calculate profit or loss on this holding
        /// </summary>
        /// <param name="currentPrice">Current market price per share</param>
        /// <returns>Total profit or loss</returns>
        public decimal ProfitLoss(decimal currentPrice) => CurrentValue(currentPrice) - TotalCost;
        
        /// <summary>
        /// Calculate percentage gain or loss on this holding
        /// </summary>
        /// <param name="currentPrice">Current market price per share</param>
        /// <returns>Percentage gain or loss</returns>
        public decimal PercentageGainLoss(decimal currentPrice)
        {
            if (TotalCost == 0)
                return 0;
                
            return (ProfitLoss(currentPrice) / TotalCost) * 100;
        }
    }
}
