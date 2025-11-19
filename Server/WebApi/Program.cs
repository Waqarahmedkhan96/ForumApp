using EfcRepositories;
using RepositoryContracts;
using System.Text.Json;                     // JSON settings
using System.Text.Json.Serialization;      // ReferenceHandler

// explicit usings
using EfAppContext = EfcRepositories.AppContext; // alias for DB context

// WebApi.GlobalExceptionHandler
using WebApi.GlobalExceptionHandler;

var builder = WebApplication.CreateBuilder(args);

// Controllers + JSON settings
builder.Services.AddControllers()
    .AddJsonOptions(o =>
    {
        o.JsonSerializerOptions.PropertyNameCaseInsensitive = true;              // ignore case
        o.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase; // camelCase
        o.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;   // avoid cycles
    });

// Register EF DbContext (SQLite app.db)
builder.Services.AddScoped<EfAppContext>(); // use alias, avoid System.AppContext

// Swagger (Swashbuckle)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// DI (Now Efc Repositories)
builder.Services.AddScoped<IUserRepository, EfcUserRepository>();        // users from DB
builder.Services.AddScoped<IPostRepository, EfcPostRepository>();        // posts from DB
builder.Services.AddScoped<ICommentRepository, EfcCommentRepository>();  // comments from DB

// Register Global Exception Middleware
builder.Services.AddTransient<GlobalExceptionHandlerMiddleware>();

var app = builder.Build();

//plug middleware in early so it catches downstream exceptions
app.UseMiddleware<GlobalExceptionHandlerMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(); // Swagger
}

app.UseHttpsRedirection();
app.MapControllers();
app.Run();
