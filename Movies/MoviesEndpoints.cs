using WebApplication1.Data;
using WebApplication1.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;


namespace WebApplication1.Movies;
public static class MoviesEndpoints
{
    public static void MapMoviesEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/movies");

        group.MapGet("/", async (ApplicationDbContext _context) => await GetMoviesAsync(_context)).WithDescription("Get all movies");
        group.MapGet("/{id}", async (ApplicationDbContext _context, string id) => await GetMovieByIdAsync(_context, id)).WithDescription("Get movie by id");
        group.MapPost("/", async (ApplicationDbContext _context, MovieCreateDto movie) => await AddMovieAsync(_context, movie)).WithDescription("Add a new movie");
        group.MapPut("/{id}", async (ApplicationDbContext _context, string id, [FromBody] MovieCreateDto movie) =>
        {
            return await UpdateMovieAsync(_context, id, movie);
        }).WithDescription("Update a movie");
        group.MapDelete("/{id}", async (ApplicationDbContext _context, string id) => await DeleteMovieAsync(_context, id)).WithDescription("Delete a movie");
    }


    private static async Task<Ok<ReturnedMoviesDto>> GetMoviesAsync(ApplicationDbContext _context)
    {
        var movies = await _context.Movies.ToListAsync();
        return TypedResults.Ok(new ReturnedMoviesDto { Items = movies });
    }

    private static async Task<Results<Ok<Movie>, NotFound>> GetMovieByIdAsync(ApplicationDbContext _context, string id)
    {
        var movie = await _context.Movies.SingleOrDefaultAsync(m => m.Id == id);
        if (movie is null)
        {
            return TypedResults.NotFound();
        }
        return TypedResults.Ok(movie);
    }


    private static async Task<Results<Created<Movie>, BadRequest>> AddMovieAsync(ApplicationDbContext _context, MovieCreateDto movie)
    {
        var newMovie = new Movie()
        {
            Id = $"movie_{Ulid.NewUlid()}",
            Name = movie.Title,
            Year = movie.ReleaseYear,
            Watched = false,
        };
        _context.Movies.Add(newMovie);
        await _context.SaveChangesAsync();
        return TypedResults.Created($"/movies/{newMovie.Id}", newMovie);
    }

    private static async Task<Results<Ok<Movie>, NotFound>> UpdateMovieAsync(ApplicationDbContext _context, string id, MovieCreateDto movie)
    {
        var movieToUpdate = await _context.Movies.FindAsync(id);
        if (movieToUpdate is null)
        {
            return TypedResults.NotFound();
        }
        movieToUpdate.Name = movie.Title;
        movieToUpdate.Year = movie.ReleaseYear;
        movieToUpdate.Watched = movie.Watched;
        await _context.SaveChangesAsync();
        return TypedResults.Ok(movieToUpdate);
    }

    private static async Task<Results<NotFound, NoContent>> DeleteMovieAsync(ApplicationDbContext _context, string id)
    {
        var movie = await _context.Movies.FindAsync(id);
        if (movie is null)
        {
            return TypedResults.NotFound();
        }
        _context.Movies.Remove(movie);
        await _context.SaveChangesAsync();
        return TypedResults.NoContent();
    }

    private record ReturnedMoviesDto
    {
        public List<Movie>? Items { get; init; }
    }

    private record MovieCreateDto
    {
        public required string Title { get; init; }
        public required int ReleaseYear { get; init; }
        public required bool Watched { get; init; }

    }
}
