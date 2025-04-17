using System;
using System.Collections.Generic;
using System.Linq;

namespace StockMarketGame.Core.Models
{
    /// <summary>
    /// Represents a player in the stock market game
    /// </summary>
    public class Player
    {
        /// <summary>
        /// Unique identifier for the player
        /// </summary>
        public Guid Id { get; set; }
        
        /// <summary>
        /// Player's name (max 9 characters in original game)
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        /// Current cash on hand
        /// </summary>
        public decimal Cash { get; set; }
        
        /// <summary>
        /// Current bank loan amount
        /// </summary>
        public decimal LoanAmount { get; set; }
        
        /// <summary>
        /// Current interest rate on the loan
        /// </summary>
        public decimal LoanInterestRate { get; set; }
        
        /// <summary>
        /// Collection of stocks owned by the player
        /// </summary>
        public List<PlayerStockHolding> StockHoldings { get; set; } = new List<PlayerStockHolding>();
        
        /// <summary>
        /// Whether the player is controlled by AI
        /// </summary>
        public bool IsAI { get; set; }
        
        /// <summary>
        /// AI difficulty/personality type (for AI players)
        /// </summary>
        public string AIPersonality { get; set; }
        
        /// <summary>
        /// History of player's total assets at the end of each turn
        /// </summary>
        public List<decimal> AssetsHistory { get; set; } = new List<decimal>();
        
        /// <summary>
        /// Final ranking in the game
        /// </summary>
        public int Ranking { get; set; }
        
        /// <summary>
        /// Final score in the game
        /// </summary>
        public decimal FinalScore { get; set; }

        /// <summary>
        /// Constructor for a new player
        /// </summary>
        /// <param name="name">Player name</param>
        /// <param name="startingCash">Starting cash amount</param>
        /// <param name="isAI">Whether the player is AI-controlled</param>
        public Player(string name, decimal startingCash = 100000, bool isAI = false)
        {
            Id = Guid.NewGuid();
            Name = name;
            Cash = startingCash;
            IsAI = isAI;
        }

        /// <summary>
        /// Buy shares of a stock
        /// </summary>
        /// <param name="stock">Stock to buy</param>
        /// <param name="shares">Number of shares to buy</param>
        /// <returns>True if purchase was successful</returns>
        public bool BuyStock(Stock stock, int shares)
        {
            if (stock == null || shares <= 0 || stock.RemainingShares < shares)
                return false;
                
            decimal cost = stock.CurrentPrice * shares;
            
            if (cost > Cash)
                return false;
                
            // Deduct cash
            Cash -= cost;
            
            // Update stock's remaining shares
            stock.RemainingShares -= shares;
            
            // Update or create player's holding
            var existingHolding = StockHoldings.FirstOrDefault(h => h.StockId == stock.Id);
            
            if (existingHolding != null)
            {
                // Calculate new average cost
                decimal totalValue = (existingHolding.Shares * existingHolding.AverageCost) + cost;
                int totalShares = existingHolding.Shares + shares;
                existingHolding.AverageCost = totalValue / totalShares;
                existingHolding.Shares = totalShares;
            }
            else
            {
                // Create new holding
                StockHoldings.Add(new PlayerStockHolding
                {
                    StockId = stock.Id,
                    Symbol = stock.Symbol,
                    CompanyName = stock.CompanyName,
                    Shares = shares,
                    AverageCost = stock.CurrentPrice
                });
            }
            
            // Check if transaction is large enough to impact market price
            if (shares >= 250000 && stock.CurrentPrice >= 4)
            {
                // Potentially bump up the price (as in original game)
                // Implementation varies based on market conditions
                decimal bumpAmount = stock.CurrentPrice * 0.05m; // 5% bump as an example
                stock.CurrentPrice += bumpAmount;
            }
            
            return true;
        }

        /// <summary>
        /// Sell shares of a stock
        /// </summary>
        /// <param name="stock">Stock to sell</param>
        /// <param name="shares">Number of shares to sell (use -1 to sell all)</param>
        /// <returns>Profit or loss from the sale</returns>
        public decimal SellStock(Stock stock, int shares)
        {
            var holding = StockHoldings.FirstOrDefault(h => h.StockId == stock.Id);
            
            if (holding == null)
                return 0;
                
            // Handle selling all shares
            if (shares == -1 || shares >= holding.Shares)
                shares = holding.Shares;
                
            if (shares <= 0)
                return 0;
                
            // Calculate proceeds and profit/loss
            decimal proceeds = stock.CurrentPrice * shares;
            decimal profitLoss = proceeds - (holding.AverageCost * shares);
            
            // Update cash and holding
            Cash += proceeds;
            
            // Update stock's remaining shares
            stock.RemainingShares += shares;
            
            // Update or remove holding
            if (shares == holding.Shares)
            {
                StockHoldings.Remove(holding);
            }
            else
            {
                holding.Shares -= shares;
            }
            
            // Check if transaction is large enough to impact market price
            if (shares >= 250000 && stock.CurrentPrice > 10)
            {
                // Potentially reduce the price (as in original game)
                decimal reductionAmount = stock.CurrentPrice * 0.05m; // 5% reduction as an example
                stock.CurrentPrice -= reductionAmount;
                
                // Ensure price doesn't go below 10 after large sale
                if (stock.CurrentPrice < 10)
                    stock.CurrentPrice = 10;
            }
            
            return profitLoss;
        }

        /// <summary>
        /// Borrow money from the bank
        /// </summary>
        /// <param name="amount">Amount to borrow</param>
        /// <param name="interestRate">Current interest rate</param>
        /// <returns>True if loan was successful</returns>
        public bool BorrowMoney(decimal amount, decimal interestRate)
        {
            if (amount <= 0)
                return false;
                
            // In the original game, there was a borrowing limit based on assets
            decimal borrowingLimit = CalculateTotalAssets() * 2;
            
            if (LoanAmount + amount > borrowingLimit)
                return false;
                
            // Update loan with new amount and interest rate
            LoanAmount += amount;
            LoanInterestRate = interestRate;
            
            // Add to cash
            Cash += amount;
            
            return true;
        }

        /// <summary>
        /// Repay loan to the bank
        /// </summary>
        /// <param name="amount">Amount to repay (use -1 to repay maximum possible)</param>
        /// <returns>Actual amount repaid</returns>
        public decimal RepayLoan(decimal amount)
        {
            if (LoanAmount <= 0)
                return 0;
                
            // Handle paying maximum possible
            if (amount == -1 || amount > Cash)
                amount = Math.Min(Cash, LoanAmount);
                
            if (amount <= 0)
                return 0;
                
            // Update loan and cash
            LoanAmount -= amount;
            Cash -= amount;
            
            if (LoanAmount <= 0)
            {
                LoanAmount = 0;
                LoanInterestRate = 0;
            }
            
            return amount;
        }

        /// <summary>
        /// Apply interest on the current loan
        /// </summary>
        public void ApplyLoanInterest()
        {
            if (LoanAmount <= 0)
                return;
                
            decimal interest = LoanAmount * (LoanInterestRate / 100);
            LoanAmount += interest;
        }

        /// <summary>
        /// Calculate the total value of all assets (cash + stocks - loans)
        /// </summary>
        /// <param name="stockPrices">Dictionary of current stock prices by ID</param>
        /// <returns>Total asset value</returns>
        public decimal CalculateTotalAssets(Dictionary<Guid, decimal> stockPrices = null)
        {
            decimal stocksValue = 0;
            
            foreach (var holding in StockHoldings)
            {
                decimal price;
                
                if (stockPrices != null && stockPrices.TryGetValue(holding.StockId, out price))
                {
                    stocksValue += price * holding.Shares;
                }
                else
                {
                    // If no current price provided, use the average cost as fallback
                    stocksValue += holding.AverageCost * holding.Shares;
                }
            }
            
            return Cash + stocksValue - LoanAmount;
        }

        /// <summary>
        /// Record the current assets value in history (called at end of turn)
        /// </summary>
        /// <param name="stockPrices">Dictionary of current stock prices by ID</param>
        public void RecordAssetsHistory(Dictionary<Guid, decimal> stockPrices)
        {
            decimal assets = CalculateTotalAssets(stockPrices);
            AssetsHistory.Add(assets);
        }

        /// <summary>
        /// Receive dividends for stocks that qualify
        /// </summary>
        /// <param name="stocks">All stocks in the game (to check dividend eligibility)</param>
        /// <returns>Total dividends received</returns>
        public decimal ReceiveDividends(List<Stock> stocks)
        {
            decimal totalDividends = 0;
            
            foreach (var holding in StockHoldings)
            {
                var stock = stocks.FirstOrDefault(s => s.Id == holding.StockId);
                
                if (stock != null && stock.QualifiesForDividend)
                {
                    decimal dividend = stock.CalculateDividend(holding.Shares);
                    totalDividends += dividend;
                }
            }
            
            // Add dividends to cash
            Cash += totalDividends;
            
            return totalDividends;
        }

        /// <summary>
        /// Handle stock splits for all holdings
        /// </summary>
        /// <param name="stocks">All stocks in the game (to check for splits)</param>
        /// <returns>Dictionary of split ratios by company name</returns>
        public Dictionary<string, int> HandleStockSplits(List<Stock> stocks)
        {
            var splitResults = new Dictionary<string, int>();
            
            foreach (var holding in StockHoldings.ToList())
            {
                var stock = stocks.FirstOrDefault(s => s.Id == holding.StockId);
                
                if (stock != null && stock.QualifiesForSplit)
                {
                    int splitRatio = stock.Split();
                    
                    if (splitRatio > 0)
                    {
                        // Update player's holding
                        holding.Shares *= splitRatio;
                        holding.AverageCost /= 2; // Price always divided by 2
                        
                        splitResults.Add(stock.CompanyName, splitRatio);
                    }
                }
            }
            
            return splitResults;
        }

        /// <summary>
        /// Handle bankruptcy for stocks in player's portfolio
        /// </summary>
        /// <param name="stocks">All stocks in the game</param>
        /// <returns>List of company names that went bankrupt</returns>
        public List<string> HandleBankruptcies(List<Stock> stocks)
        {
            var bankruptCompanies = new List<string>();
            
            foreach (var stock in stocks)
            {
                if (stock.IsBankrupt)
                {
                    var holding = StockHoldings.FirstOrDefault(h => h.StockId == stock.Id);
                    
                    if (holding != null)
                    {
                        StockHoldings.Remove(holding);
                        bankruptCompanies.Add(stock.CompanyName);
                    }
                }
            }
            
            return bankruptCompanies;
        }
    }
}
