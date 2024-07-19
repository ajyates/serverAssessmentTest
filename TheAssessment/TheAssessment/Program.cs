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

builder.Services.AddRateLimiter(options =>
{
});

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

app.UseRateLimiter();
//app.UseAuthentication();
//app.UseAuthorization();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

var _groupEndpoints = app.MapGroup("/api");

// _groupEndpoints.MapPost("/auth/signup", () => { });
// _groupEndpoints.MapPost("/auth/login", () => { });


_groupEndpoints.MapGet("/notes", async (NotesService db) => 
        TypedResults.Ok(db.GetAsync())).WithName("GetNotes")
    .WithOpenApi(x => new OpenApiOperation(x)
    {
        Summary = "Get All Notes",
        Description = "Returns all the notes.",
        Tags = new List<OpenApiTag> { new() { Name = "The Notes" } }
    });

_groupEndpoints.MapGet("/notes/{id}", Results<Ok<Note>, NotFound> (NotesService db, string id) =>
        db.GetAsync(id) is { } note 
            ? TypedResults.Ok(note.Result) 
            : TypedResults.NotFound()
    )
    .WithName("GetNoteById")
    .WithOpenApi(x => new OpenApiOperation(x)
    {
        Summary = "Get Note By Id",
        Description = "Returns information about selected note from the database.",
        Tags = new List<OpenApiTag> { new() { Name = "The Notes" } }
    });


_groupEndpoints.MapPost("/notes", Results<Ok<string>, BadRequest<string>> (Note newNote, NotesService db) =>
        {
            var result = db.CreateAsync(newNote);
            if (result.IsCompletedSuccessfully)
                return TypedResults.Ok("Note created");
            else
                return TypedResults.BadRequest("Not created");
        }
    )
    .WithName("CreateNote")
    .WithOpenApi(x => new OpenApiOperation(x)
    {
        Summary = "Create a new note",
        Description = "Returns information about selected note from the database.",
        Tags = new List<OpenApiTag> { new() { Name = "The Notes" } }
    });


// _groupEndpoints.MapPost("/notes", () => { });


_groupEndpoints.MapPut("/notes/{id}", Results<Ok<string>, BadRequest<string>> (string id, Note newNote, NotesService db) =>
        {
            //Make sure the ID is valid before we make an attempt.
            var exists = db.GetAsync(id);
            if (exists.Result == null || exists.Result.Id == null)
            {
                return TypedResults.BadRequest("Note does not exist");
            }
            
            var result = db.UpdateAsync(id, newNote);
            if (result.IsCompletedSuccessfully)
                return TypedResults.Ok("Note updated");
            else
                return TypedResults.BadRequest("Nothing updated");
        }
    )
    .WithName("CreateNote")
    .WithOpenApi(x => new OpenApiOperation(x)
    {
        Summary = "Create a new note",
        Description = "Returns information about selected note from the database.",
        Tags = new List<OpenApiTag> { new() { Name = "The Notes" } }
    });



_groupEndpoints.MapDelete("/notes/{id}", Results<Ok<string>, NotFound> (NotesService db, string id) =>
    {
        var result = db.RemoveAsync(id);
        if (result.IsCompletedSuccessfully)
            return TypedResults.Ok("Deleted successfullly.");
        else
            return TypedResults.NotFound();
    })
    .WithName("DeleteNoteById")
    .WithOpenApi(x => new OpenApiOperation(x)
    {
        Summary = "Delete Note By Id",
        Description = "Deletes a note with the specified ID.",
        Tags = new List<OpenApiTag> { new() { Name = "The Notes" } }
    });
//
// _groupEndpoints.MapPost("/notes/{id}/share", () => { });
// _groupEndpoints.MapGet("/search{query}", () => { });
_groupEndpoints.MapGet("/search{query}", Results<Ok<Note>, NotFound> (NotesService db, string query) =>
        db.GetAsync(query) is { } note 
            ? TypedResults.Ok(note.Result) 
            : TypedResults.NotFound()
    )
    .WithName("FindNotes")
    .WithOpenApi(x => new OpenApiOperation(x)
    {
        Summary = "Search notes",
        Description = "Returns information about found notes based on the search parameters.",
        Tags = new List<OpenApiTag> { new() { Name = "The Notes" } }
    });


app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}

