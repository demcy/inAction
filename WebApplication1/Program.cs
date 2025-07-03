using Microsoft.AspNetCore.HttpLogging;
using System.Collections.Concurrent;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddHttpLogging(opts => opts.LoggingFields = HttpLoggingFields.RequestProperties);
builder.Logging.AddFilter("Microsoft.AspNetCore.HttpLogging", LogLevel.Information);
var app = builder.Build();

var people = new List<Person>
{
    new("Tom", "Hanks"),
    new("Denzel", "Washington"),
    new("Leondardo", "DiCaprio"),
    new("Al", "Pacino"),
    new("Morgan", "Freeman"),
};

var _fruit = new ConcurrentDictionary<string, Fruit>();

// if (app.Environment.IsDevelopment())
// {
//     app.UseHttpLogging();
// }

// if (!app.Environment.IsDevelopment())
// {
//     app.UseExceptionHandler("/error");
// }



app.UseWelcomePage("/");
// app.UseDeveloperExceptionPage();
app.UseStaticFiles();
app.UseRouting();

app.MapGet("/", () => "Hello World!");
app.MapGet("/error", () => "Sorry, an error occurred");


app.MapGet("/person/{name}", (string name) => people.Where(p => p.FirstName.StartsWith(name, StringComparison.OrdinalIgnoreCase)));

// app.MapGet("/fruit", () => Fruit.All);
app.MapGet("/fruit", () => _fruit);

// var getFruit = (string id) => Fruit.All[id];
// app.MapGet("/fruit/{id}", getFruit);
app.MapGet("/fruit/{id}", (string id) => _fruit.TryGetValue(id, out var fruit)
    ? TypedResults.Ok(fruit)
    : Results.NotFound());

// app.MapPost("/fruit/{id}", Handlers.AddFruit);
app.MapPost("/fruit/{id}", (string id, Fruit fruit) => _fruit.TryAdd(id, fruit)
    ? TypedResults.Created($"/fruit/{id}", fruit)
    : Results.BadRequest(new
    {
        id = "A fruit with this id already exists"
    })
);

// Handlers handlers = new();
// app.MapPut("/fruit/{id}", handlers.ReplaceFruit);
app.MapPut("/fruit/{id}", (string id, Fruit fruit) =>
{
    _fruit[id] = fruit;
    return Results.NoContent();
});

// app.MapDelete("/fruit/{id}", DeleteFruit);
app.MapDelete("/fruit/{id}", (string id) =>
{
    _fruit.TryRemove(id, out _);
    return Results.NoContent();
});

app.Run();

// void DeleteFruit(string id)
// {
//     Fruit.All.Remove(id);
// }

record Fruit(string Name, int Stock)
{
    public static readonly Dictionary<string, Fruit> All = new();
};

// class Handlers
// {
//     public void ReplaceFruit(string id, Fruit fruit)
//     {
//         Fruit.All[id] = fruit;
//     }

//     public static void AddFruit(string id, Fruit fruit)
//     {
//         Fruit.All.Add(id, fruit);
//     }
// }

public record Person(string FirstName, string LastName);
