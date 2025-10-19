using FileRepositories;
using RepositoryContracts;

// (explicit usings calm VS Code on preview SDKs)
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

// Controllers
builder.Services.AddControllers();

// Swagger (Swashbuckle)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// DI (file repositories)
builder.Services.AddScoped<RepositoryContracts.IUserRepository, FileRepositories.UserFileRepository>();
builder.Services.AddScoped<RepositoryContracts.IPostRepository, FileRepositories.PostFileRepository>();
builder.Services.AddScoped<RepositoryContracts.ICommentRepository, FileRepositories.CommentFileRepository>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(); // /swagger
}

app.UseHttpsRedirection();
app.MapControllers();
app.Run();
