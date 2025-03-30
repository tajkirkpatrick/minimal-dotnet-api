

using Microsoft.AspNetCore.Http.HttpResults;

namespace WebApplication1.Movies;
public static class MoviesEndpoints
{

    private static readonly List<Movie> _movies = new();

    public static void MapMoviesEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/movies");

        group.MapGet("/", () => GetMovies());
        group.MapGet("/{id}", (string id) => GetMovieById(id));
        group.MapPost("/", (MovieCreateDto movie) => AddMovie(movie));
        group.MapPut("/{id}", (string id, MovieCreateDto movie) => UpdateMovie(id, movie));
        group.MapDelete("/{id}", (string id) => DeleteMovie(id));
    }

    private static Results<Ok<MoviesReturnedDto>, BadRequest> GetMovies()
    {
        return TypedResults.Ok(new MoviesReturnedDto { Items = _movies });
    }

    private static Results<Ok<Movie>, NotFound, BadRequest> GetMovieById(string id)
    {
        if (string.IsNullOrEmpty(id))
        {
            return TypedResults.BadRequest();
        }

        var movie = _movies.FirstOrDefault(m => m.Id == id);
        if (movie is null)
        {
            return TypedResults.NotFound();
        }
        return TypedResults.Ok(movie);
    }

    private static Results<Created<Movie>, BadRequest> AddMovie(MovieCreateDto movie)
    {
        if (string.IsNullOrEmpty(movie.Title) || movie.ReleaseYear <= 1800)
        {
            return TypedResults.BadRequest();
        }

        var movieWithId = new Movie
        {
            Id = $"movie_{Ulid.NewUlid()}",
            Title = movie.Title,
            ReleaseYear = movie.ReleaseYear
        };

        _movies.Add(movieWithId);
        return TypedResults.Created($"/movies/{movieWithId.Id}", movieWithId);
    }

    private static Results<Ok<Movie>, NotFound, BadRequest> UpdateMovie(string id, MovieCreateDto movie)
    {
        if (string.IsNullOrEmpty(id))
        {
            return TypedResults.BadRequest();
        }

        var movieToUpdate = _movies.FirstOrDefault(m => m.Id == id);
        if (movieToUpdate is null)
        {
            return TypedResults.NotFound();
        }

        movieToUpdate.Title = movie.Title;
        movieToUpdate.ReleaseYear = movie.ReleaseYear;
        return TypedResults.Ok(movieToUpdate);
    }

    private static Results<NoContent, NotFound, BadRequest> DeleteMovie(string id)
    {
        if (string.IsNullOrEmpty(id))
        {
            return TypedResults.BadRequest();
        }

        var movie = _movies.FirstOrDefault(m => m.Id == id);
        if (movie is null)
        {
            return TypedResults.NotFound();
        }

        _movies.Remove(movie);
        return TypedResults.NoContent();
    }

    private record MoviesReturnedDto
    {
        public required List<Movie> Items { get; init; }
    }

    private record MovieCreateDto
    {
        public required string Title { get; init; }
        public required int ReleaseYear { get; init; }
    }
}
