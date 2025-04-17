using Microsoft.EntityFrameworkCore;
using StockMarketGame.Core.Models;
using System;
using System.Collections.Generic;
using System.Text.Json;

namespace StockMarketGame.Data
{
    /// <summary>
    /// Database context for the Stock Market Game
    /// </summary>
    public class GameDbContext : DbContext
    {
        /// <summary>
        /// Games data
        /// </summary>
        public DbSet<GameData> Games { get; set; }
        
        /// <summary>
        /// High scores data
        /// </summary>
        public DbSet<HighScore> HighScores { get; set; }

        /// <summary>
        /// Constructor with options
        /// </summary>
        /// <param name="options">DB context options</param>
        public GameDbContext(DbContextOptions<GameDbContext> options)
            : base(options)
        {
        }

        /// <summary>
        /// Configure the database model
        /// </summary>
        /// <param name="modelBuilder">Model builder</param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            // Configure Game entity
            modelBuilder.Entity<GameData>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.CreatedAt).IsRequired();
                entity.Property(e => e.LastUpdatedAt).IsRequired();
                entity.Property(e => e.GameState).IsRequired();
            });
            
            // Configure HighScore entity
            modelBuilder.Entity<HighScore>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.PlayerName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Score).IsRequired();
                entity.Property(e => e.Date).IsRequired();
                entity.Property(e => e.GameMode).HasMaxLength(50);
            });
        }
    }

    /// <summary>
    /// Data entity for storing game state in the database
    /// </summary>
    public class GameData
    {
        /// <summary>
        /// Unique identifier for the game data
        /// </summary>
        public Guid Id { get; set; }
        
        /// <summary>
        /// Name of the saved game
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        /// When the game was created
        /// </summary>
        public DateTime CreatedAt { get; set; }
        
        /// <summary>
        /// When the game was last updated
        /// </summary>
        public DateTime LastUpdatedAt { get; set; }
        
        /// <summary>
        /// Serialized game state data
        /// </summary>
        public string GameState { get; set; }
        
        /// <summary>
        /// Convert a Game object to GameData for storage
        /// </summary>
        /// <param name="game">Game to convert</param>
        /// <param name="name">Name for the saved game</param>
        /// <returns>GameData for database storage</returns>
        public static GameData FromGame(Game game, string name)
        {
            return new GameData
            {
                Id = game.Id,
                Name = name,
                CreatedAt = game.CreatedAt,
                LastUpdatedAt = DateTime.Now,
                GameState = JsonSerializer.Serialize(game, new JsonSerializerOptions
                {
                    WriteIndented = true
                })
            };
        }
        
        /// <summary>
        /// Convert stored GameData back to a Game object
        /// </summary>
        /// <returns>Deserialized Game object</returns>
        public Game ToGame()
        {
            return JsonSerializer.Deserialize<Game>(GameState, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
        }
    }
}
