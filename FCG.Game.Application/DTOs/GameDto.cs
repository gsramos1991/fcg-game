using System;
using System.Collections.Generic;
using FCG.Game.Domain.Entities;

namespace FCG.Game.Application.DTOs
{
    public class GameDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Genre { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string Publisher { get; set; } = string.Empty;
        public DateTime ReleaseDate { get; set; }
        public List<string> Tags { get; set; } = new();
        public string CoverImageUrl { get; set; } = string.Empty;

        public static GameDto? FromGame(Domain.Entities.Game game)
        {
            if (game == null) return null;

            return new GameDto
            {
                Id = game.Id,
                Title = game.Title,
                Description = game.Description,
                Genre = game.Genre,
                Price = game.Price,
                Publisher = game.Publisher,
                ReleaseDate = game.ReleaseDate,
                Tags = game.Tags,
                CoverImageUrl = game.CoverImageUrl
            };
        }
    }
}