using Microsoft.AspNetCore.HttpLogging;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddHttpLogging(opts => opts.LoggingFields = HttpLoggingFields.RequestProperties);
builder.Logging.AddFilter("Microsoft.AspNetCore.HttpLogging", LogLevel.Information);
var app = builder.Build();

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

var people = new List<Person>
{
    new("Tom", "Hanks"),
    new("Denzel", "Washington"),
    new("Leondardo", "DiCaprio"),
    new("Al", "Pacino"),
    new("Morgan", "Freeman"),
};
app.MapGet("/person/{name}", (string name) => people.Where(p => p.FirstName.StartsWith(name)));

app.MapGet("/fruit", () => Fruit.All);

var getFruit = (string id) => Fruit.All[id];
app.MapGet("/fruit/{id}", getFruit);

app.MapPost("/fruit/{id}", Handlers.AddFruit);

Handlers handlers = new();
app.MapPut("/fruit/{id}", handlers.ReplaceFruit);

app.MapDelete("/fruit/{id}", DeleteFruit);

app.Run();

void DeleteFruit(string id)
{
    Fruit.All.Remove(id);
}

record Fruit(string Name, int Stock)
{
    public static readonly Dictionary<string, Fruit> All = new();
};

class Handlers
{
    public void ReplaceFruit(string id, Fruit fruit)
    {
        Fruit.All[id] = fruit;
    }

    public static void AddFruit(string id, Fruit fruit)
    {
        Fruit.All.Add(id, fruit);
    }
}

public record Person(string FirstName, string LastName);
