using Dapper;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using System.Data;
using TodoApi;

var builder = WebApplication.CreateBuilder(args);

//swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//cors
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()    // מתיר לכל מקור לגשת
              .AllowAnyMethod()    // מתיר לכל שיטה (GET, POST, PUT, DELETE וכו') לעבוד
              .AllowAnyHeader();   // מתיר לכל כותרת להיות נשלחת
    });
});

// הגדרת החיבור למסד נתונים כ- service
builder.Services.AddScoped<IDbConnection>((services) =>
    new MySqlConnection(builder.Configuration.GetConnectionString("practicodeSQL")));


var app = builder.Build();

//מאפשר גישה לכל המתודות שלי
app.UseCors("AllowAll");

// הפעלת Swagger בהתאם לסביבה
if (app.Environment.IsDevelopment())
{
    app.UseSwagger(); // יוצר את המסמכים של Swagger
    app.UseSwaggerUI(options => // יוצר את הממשק של Swagger UI
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
        options.RoutePrefix = string.Empty; // מציג את Swagger ב-root של האפליקציה
    });
}

//swagger
app.UseSwagger();
app.UseSwaggerUI();

// מחרוזת חיבור
var connectionString = builder.Configuration.GetConnectionString("practicodeSQL");

// קבלת כל הפריטים
app.MapGet("/items", async (IDbConnection db) =>
{
    var items = await db.QueryAsync<Item>("SELECT * FROM items");
    return Results.Json(items);
});

// הוספת פריט חדש
app.MapPost("/items", async (IDbConnection db, [FromBody] ItemDto itemDto) =>
{
    string Name = itemDto.Name;
    try
    {
        if (string.IsNullOrWhiteSpace(Name))
            return Results.BadRequest(new { message = "Name is required." });

        var sql = @"
        INSERT INTO items (Name, IsComplete) 
        VALUES (@Name, false);
        SELECT LAST_INSERT_ID();";

        var id = await db.QuerySingleAsync<int>(sql, new { Name });
        return Results.Ok(new { Id = id, Message = "Item added successfully." });
    }
    catch (Exception ex)
    {

        return Results.BadRequest(new { message = ex.Message });
    }
});

// עדכון פריט קיים
app.MapPut("/items/{id}", async (IDbConnection db, int id, [FromBody] ItemDto itemDto) =>
{

    if (id == 0 || itemDto == null)
        return Results.BadRequest(new { message = "Invalid data." });
  
    var sql = @"
        UPDATE items 
        SET IsComplete = @IsComplete 
        WHERE Id = @id";

    var result = await db.ExecuteAsync(sql, new {  Id = id , itemDto.IsComplete});

    if (result > 0)
    {
        return Results.Ok(new { message = "Item updated successfully." });
    }
    else
    {
        return Results.NotFound(new { message = "Item not found." });
    }
});

//מחיקת פריט
app.MapDelete("/items/{id}", async (IDbConnection db, int id) =>
{
    if (id <= 0)
        return Results.BadRequest(new { message = "Invalid id." });

    var sql = @"DELETE FROM items WHERE Id = @id;";

    var result = await db.ExecuteAsync(sql, new { Id = id });

    if (result > 0)
    {
        return Results.Ok(new { message = "Item deleted successfully." });
    }
    else
    {
        return Results.NotFound(new { message = "Item not found." });
    }
});

// ברירת מחדל
app.MapGet("/", () => "Welcome to the Todo API!");
app.Run();

//מיפויים
public record ItemDto(string Name, bool IsComplete);