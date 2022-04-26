using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

builder.Services.AddDbContext<HomeworkDbContext>(options => options.UseInMemoryDatabase("HomeworkDB"));


app.MapGet("/homeworks", async (HomeworkDbContext db) => await db.Homeworks.ToListAsync());

app.MapGet("/homeworks/{id}", async (HomeworkDbContext db, int id) => await db.Homeworks.FindAsync(id) is Homework homework ? Results.Ok(homework) : Results.NotFound());

app.MapGet("/homeworks/finished", async (HomeworkDbContext db) => await db.Homeworks.Where(t => t.IsFinish).ToListAsync());


app.MapPost("/homeworks", async (HomeworkDbContext db, Homework homework) =>
{
    db.Homeworks.Add(homework);
    await db.SaveChangesAsync();
    return Results.Created($"/homeworks/{homework.Id}", homework);
});

app.MapPut("/homeworks/{id}", async (HomeworkDbContext db, Homework homework, int id) =>
{
    var homeworks = await db.Homeworks.FindAsync(id);

    if(homeworks == null)
        return Results.NotFound();

    homeworks.Name = homework.Name;
    homeworks.IsFinish = homework.IsFinish;
    await db.SaveChangesAsync();
    return Results.NoContent();
});

app.MapDelete("/homeworks/{id}", async (HomeworkDbContext db, int id) =>
{
    var homeworks = await db.Homeworks.FindAsync(id);

    if (homeworks == null)
        return Results.NotFound();

    db.Homeworks.Remove(homeworks);
    await db.SaveChangesAsync();
    return Results.NoContent();
});

app.Run();

class Homework
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public bool IsFinish { get; set; }

}

class HomeworkDbContext : DbContext
{
    public HomeworkDbContext(DbContextOptions<HomeworkDbContext> options) : base(options)
    {

    }
    public DbSet<Homework> Homeworks => Set<Homework>();

}


