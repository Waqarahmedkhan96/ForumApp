using FileRepositories;
using RepositoryContracts;

// explicit usings
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

// WebApi.GlobalExceptionHandler
using WebAPI.GlobalExceptionHandler; 

var builder = WebApplication.CreateBuilder(args);

// MVC Controllers
builder.Services.AddControllers();

// Controllers + JSON settings
//builder.Services.AddControllers()
 //   .AddJsonOptions(o =>
   // {
     //   o.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
       // o.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
    //});

// Swagger (Swashbuckle)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// DI (file repositories)
builder.Services.AddScoped<IUserRepository, UserFileRepository>();
builder.Services.AddScoped<IPostRepository, PostFileRepository>();
builder.Services.AddScoped<ICommentRepository, CommentFileRepository>();

// register global exception middleware
builder.Services.AddTransient<GlobalExceptionHandlerMiddleware>();

var app = builder.Build();

//plug middleware in early so it catches downstream exceptions
app.UseMiddleware<GlobalExceptionHandlerMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(); // /swagger
}

app.UseHttpsRedirection();
app.MapControllers();
app.Run();
