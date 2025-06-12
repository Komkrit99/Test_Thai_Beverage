using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using Backend.Data;
using Backend.Models;

var builder = WebApplication.CreateBuilder(args);

// Configure services

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:3000", "http://frontend:3000")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

// Add Entity Framework with In-Memory Database for testing
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseInMemoryDatabase("ExamDatabase"));

var app = builder.Build();

// Seed the database with sample data
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    context.Database.EnsureCreated();
    
    if (!context.ExamQuestions.Any())
    {
        context.ExamQuestions.AddRange(
            new ExamQuestion
            {
                QuestionNumber = 1,
                QuestionText = "ข้อใดต่อไปนี้คือคำตอบที่ถูกต้อง",
                Option1 = "3",
                Option2 = "5",
                Option3 = "9",
                Option4 = "11",
                CorrectAnswer = 1,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new ExamQuestion
            {
                QuestionNumber = 2,
                QuestionText = "x + 2 = 4 จงหาค่า x",
                Option1 = "1",
                Option2 = "2",
                Option3 = "3",
                Option4 = "4",
                CorrectAnswer = 2,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }
        );
        context.SaveChanges();
    }
}

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    //app.UseSwagger();
    //app.UseSwaggerUI();
}

app.UseCors("AllowFrontend");

app.UseAuthorization();

app.MapControllers();

// Add a simple health check endpoint
app.MapGet("/", () => "Backend API is running!");

app.MapGet("/api/health", () => new { status = "healthy", timestamp = DateTime.UtcNow });

app.Run();