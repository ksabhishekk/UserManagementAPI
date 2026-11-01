using System.Collections.Concurrent;
using Microsoft.AspNetCore.Http.HttpResults;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

var users = new ConcurrentDictionary<int, User>();
var nextUserId = 0;

app.MapGet("/users", () => TypedResults.Ok(users.Values.OrderBy(user => user.Id).ToArray()))
    .WithName("GetUsers")
    .WithSummary("Get all users")
    .WithDescription("Returns all users, ordered by ID.")
    .Produces<User[]>(StatusCodes.Status200OK);

app.MapGet("/users/{id:int}", Results<Ok<User>, NotFound> (int id) =>
{
    return users.TryGetValue(id, out var user)
        ? TypedResults.Ok(user)
        : TypedResults.NotFound();
})
    .WithName("GetUserById")
    .WithSummary("Get a user by ID")
    .WithDescription("Returns the user with the specified ID.")
    .Produces<User>(StatusCodes.Status200OK)
    .Produces(StatusCodes.Status404NotFound);

app.MapPost("/users", (CreateUserRequest request) =>
{
    var id = Interlocked.Increment(ref nextUserId);
    var user = new User(id, request.Name, request.Email, request.Department);
    users[id] = user;

    return TypedResults.Created($"/users/{id}", user);
})
    .WithName("CreateUser")
    .WithSummary("Create a user")
    .WithDescription("Creates a user and returns its URL in the Location header.")
    .Produces<User>(StatusCodes.Status201Created);

app.MapPut("/users/{id:int}", Results<Ok<User>, NotFound> (int id, UpdateUserRequest request) =>
{
    while (users.TryGetValue(id, out var existingUser))
    {
        var updatedUser = existingUser with
        {
            Name = request.Name,
            Email = request.Email,
            Department = request.Department
        };

        if (users.TryUpdate(id, updatedUser, existingUser))
        {
            return TypedResults.Ok(updatedUser);
        }
    }

    return TypedResults.NotFound();
})
    .WithName("UpdateUser")
    .WithSummary("Update a user")
    .WithDescription("Replaces the name, email, and department for the specified user.")
    .Produces<User>(StatusCodes.Status200OK)
    .Produces(StatusCodes.Status404NotFound);

app.MapDelete("/users/{id:int}", Results<NoContent, NotFound> (int id) =>
    users.TryRemove(id, out _)
        ? TypedResults.NoContent()
        : TypedResults.NotFound())
    .WithName("DeleteUser")
    .WithSummary("Delete a user")
    .WithDescription("Deletes the user with the specified ID.")
    .Produces(StatusCodes.Status204NoContent)
    .Produces(StatusCodes.Status404NotFound);

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast =  Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast");

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}

sealed record User(int Id, string Name, string Email, string Department);

sealed record CreateUserRequest(string Name, string Email, string Department);

sealed record UpdateUserRequest(string Name, string Email, string Department);
