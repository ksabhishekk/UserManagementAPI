var users = new Dictionary<int, User>();
var nextId = 1;

app.MapGet("/users", () => users.Values);

app.MapGet("/users/{id:int}", (int id) =>
    users.TryGetValue(id, out var u) ? Results.Ok(u) : Results.NotFound());

app.MapPost("/users", (User user) =>
{
    var created = user with { Id = nextId++ };
    users[created.Id] = created;
    return Results.Created($"/users/{created.Id}", created);
});

app.MapPut("/users/{id:int}", (int id, User user) =>
{
    if (!users.ContainsKey(id)) return Results.NotFound();
    users[id] = user with { Id = id };
    return Results.NoContent();
});

app.MapDelete("/users/{id:int}", (int id) =>
    users.Remove(id) ? Results.NoContent() : Results.NotFound());

public record User(int Id, string Name, string Email, string Department);