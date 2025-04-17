# Project Structure

The Stock Market Game Remake is organized into a modular architecture with the following components:

## Core Components

### 1. StockMarketGame.Core
Contains the core game logic and domain models:
- Player management
- Stock market simulation
- Game state management
- Market events

### 2. StockMarketGame.Data
Handles data persistence:
- Database context and migrations
- Repository implementations
- Data models and mapping

### 3. StockMarketGame.UI
User interface implementation:
- WPF UI for initial version
- Views for game screens
- ViewModels for UI logic

## Planned/Future Components

### 4. StockMarketGame.AI
AI and machine learning integration:
- AI player behavior
- News generation
- Market event predictions

### 5. StockMarketGame.Network
Multiplayer functionality:
- Network protocols
- Server/client architecture
- Chat system

### 6. StockMarketGame.Unity (Later Phase)
3D UI implementation using Unity:
- 3D office environment
- Interactive elements
- Advanced visualizations

## Development Approach

The project follows a layered architecture pattern:
- Core business logic is independent of UI and data access
- Dependency injection for loose coupling between components
- MVVM pattern for UI development
- Repository pattern for data access
