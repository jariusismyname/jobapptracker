using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Setup the In-Memory Database
builder.Services.AddDbContext<AppDb>(opt => opt.UseInMemoryDatabase("JobTracker"));

var app = builder.Build();
app.UseDefaultFiles();
app.UseStaticFiles();

// API Endpoints
// GET: Retrieve all job applications
app.MapGet("/api/jobs", async (AppDb db) => 
    await db.Jobs.ToListAsync());

// POST: Add a new job application
app.MapPost("/api/jobs", async (JobApp job, AppDb db) => {
    db.Jobs.Add(job);
    await db.SaveChangesAsync();
    return Results.Created($"/api/jobs/{job.Id}", job);
});

// DELETE: Remove a job application
app.MapDelete("/api/jobs/{id}", async (int id, AppDb db) => {
    if (await db.Jobs.FindAsync(id) is JobApp job) {
        db.Jobs.Remove(job);
        await db.SaveChangesAsync();
        return Results.NoContent();
    }
    return Results.NotFound();
});

app.Run();

// Data Models
public class JobApp {
    public int Id { get; set; }
    public string? Company { get; set; }
    public string? Role { get; set; }
    public string? Status { get; set; } 
}

public class AppDb : DbContext {
    public AppDb(DbContextOptions<AppDb> options) : base(options) { }
    public DbSet<JobApp> Jobs => Set<JobApp>();
}