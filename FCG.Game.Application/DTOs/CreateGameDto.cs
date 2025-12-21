using System;
using System.Collections.Generic;

namespace FCG.Game.Application.DTOs
{
    public record CreateGameDto(
        string Title,
        string Description,
        string Genre,
        decimal Price,
        string Publisher,
        DateTime ReleaseDate,
        List<string> Tags,
        string CoverImageUrl
    );
}