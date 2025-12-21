using System;
using System.Collections.Generic;

namespace FCG.Game.Application.DTOs
{
    public class TopGamesMetrics
    {
        public List<GameSalesMetric> TopGames { get; set; } = new();
    }

    public class GameSalesMetric
    {
        public Guid GameId { get; set; }
        public int TotalSales { get; set; }
    }

    public class GenreMetrics
    {
        public List<GenreStatistic> Genres { get; set; } = new();
    }

    public class GenreStatistic
    {
        public string Genre { get; set; } = string.Empty;
        public int TotalGames { get; set; }
        public int TotalSales { get; set; }
        public decimal AveragePrice { get; set; }
    }

    public class SalesMetrics
    {
        public decimal TotalRevenue { get; set; }
        public int TotalOrders { get; set; }
        public decimal AverageOrderValue { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }

    public class UserBehaviorMetrics
    {
        public List<TopBuyer> TopBuyers { get; set; } = new();
    }

    public class TopBuyer
    {
        public Guid UserId { get; set; }
        public decimal TotalSpent { get; set; }
        public int OrderCount { get; set; }
    }
}
