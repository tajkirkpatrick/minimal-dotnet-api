using Scalar.AspNetCore;
using WebApplication1.Movies;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(opts =>
    {
        opts.WithTitle("Movies API");
        opts.WithDefaultHttpClient(ScalarTarget.Python, ScalarClient.Requests);
    });
}

app.UseHttpsRedirection();

app.MapMoviesEndpoints();

app.Run();
