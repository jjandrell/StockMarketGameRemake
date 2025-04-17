using System;
using System.Collections.Generic;

namespace StockMarketGame.Core.Models
{
    /// <summary>
    /// Represents a market event that impacts stock prices
    /// </summary>
    public class MarketEvent
    {
        /// <summary>
        /// Unique identifier for the event
        /// </summary>
        public Guid Id { get; set; }
        
        /// <summary>
        /// Turn number when this event occurred
        /// </summary>
        public int Turn { get; set; }
        
        /// <summary>
        /// Text description of the event (shown on ticker tape)
        /// </summary>
        public string EventText { get; set; }
        
        /// <summary>
        /// Impact on stock price (positive or negative)
        /// </summary>
        public decimal PriceImpact { get; set; }
        
        /// <summary>
        /// IDs of stocks affected by this event
        /// </summary>
        public List<Guid> AffectedStockIds { get; set; } = new List<Guid>();
        
        /// <summary>
        /// Optional image or icon representing the event
        /// </summary>
        public string EventIcon { get; set; }
        
        /// <summary>
        /// Whether this is a global market event (affects all stocks)
        /// </summary>
        public bool IsGlobalEvent { get; set; }
        
        /// <summary>
        /// Category of the event (financial, political, etc.)
        /// </summary>
        public string Category { get; set; }
        
        /// <summary>
        /// Optional secondary effects (e.g., interest rate changes)
        /// </summary>
        public string SecondaryEffect { get; set; }
        
        /// <summary>
        /// Optional severity level (for visual indicators)
        /// </summary>
        public int SeverityLevel { get; set; }
        
        /// <summary>
        /// Constructor with basic properties
        /// </summary>
        public MarketEvent()
        {
            Id = Guid.NewGuid();
        }
    }
}
