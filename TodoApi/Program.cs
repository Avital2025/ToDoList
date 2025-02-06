

using Microsoft.EntityFrameworkCore;
using TodoApi;
using Swashbuckle.AspNetCore.SwaggerGen;
using Swashbuckle.AspNetCore.SwaggerUI;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// הגדרת Swagger לפני builder.Build()
builder.Services.AddDbContext<ToDoDbContext>(options =>
    options.UseMySql("server=localhost;user=root;password=aA1795aA;database=ToDoDB",
    Microsoft.EntityFrameworkCore.ServerVersion.Parse("8.0.41-mysql")));

builder.Services.AddEndpointsApiExplorer();

// הוספת Swagger
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "ToDo API",
        Version = "v1",
        Description = "A simple ToDo API to manage tasks."
    });
});

// הגדרת CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigins", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build(); // עכשיו מבצעים את ה-build אחרי שהוספנו את כל השירותים

// הפעלת Swagger UI
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "ToDo API v1");
    options.RoutePrefix = string.Empty;  // בחר להציג את Swagger UI בכתובת הראשית
});

// הפעלת מדיניות CORS
app.UseCors("AllowSpecificOrigins");

// הגדרת שאר ה-EndPoints
app.MapGet("/items", async (ToDoDbContext dbContext) =>
{
    return await dbContext.Items.ToListAsync();
});

app.MapPost("/items", async (Item item, ToDoDbContext dbContext) =>
{
    dbContext.Items.Add(item);
    await dbContext.SaveChangesAsync();
    return Results.Created($"/items/{item.Id}", item);
});

app.MapPut("/items/{id}", async (int id, Item updatedItem, ToDoDbContext dbContext) =>
{
    var item = await dbContext.Items.FindAsync(id);
    if (item is null) return Results.NotFound();

    item.Name = updatedItem.Name;
    item.IsComplete = updatedItem.IsComplete;
    await dbContext.SaveChangesAsync();

    return Results.Ok(item);
});

app.MapDelete("/items/{id}", async (int id, ToDoDbContext dbContext) =>
{
    var item = await dbContext.Items.FindAsync(id);
    if (item is null) return Results.NotFound();

    dbContext.Items.Remove(item);
    await dbContext.SaveChangesAsync();

    return Results.NoContent();
});

app.Run();
