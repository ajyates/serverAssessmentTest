using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.OpenApi.Models;
using TheAssessment.Models;
using TheAssessment.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.Configure<NotesDatabaseSettings>(
    builder.Configuration.GetSection("NotesDatabase"));

builder.Services.AddSingleton<NotesService>();
builder.Services.AddSingleton<UsersService>();

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

//app.UseAuthentication();
//app.UseAuthorization();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

var _groupEndpoints = app.MapGroup("/api");

// _groupEndpoints.MapPost("/auth/signup", () => { });
// _groupEndpoints.MapPost("/auth/login", () => { });

// _groupEndpoints.MapHealthChecks("/notes").RequireAuthorization();
_groupEndpoints.MapGet("/notes", async (NotesService db) => 
        TypedResults.Ok(db.GetAsync())).WithName("GetNotes")
    .WithOpenApi(x => new OpenApiOperation(x)
    {
        Summary = "Get All Notes",
        Description = "Returns all the notes..",
        Tags = new List<OpenApiTag> { new() { Name = "The Notes" } }
    });

// _groupEndpoints.MapGet("/notes/{id}", () => { });
// _groupEndpoints.MapPost("/notes", () => { });
// _groupEndpoints.MapPut("/notes/{id}", () => { });
// _groupEndpoints.MapDelete("/notes/{id}", () => { });
//
// _groupEndpoints.MapPost("/notes/{id}/share", () => { });
// _groupEndpoints.MapGet("/search{query}", () => { });


app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}

