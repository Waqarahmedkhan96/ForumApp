using BlazorApp.Components;

using BlazorApp.Services.HttpUserService;
using BlazorApp.Services.HttpPostService;
using BlazorApp.Services;
using BlazorApp.Services.Interfaces;
// For Auth
using Microsoft.AspNetCore.Components.Authorization;
using BlazorApp.Services.Auth;

var builder = WebApplication.CreateBuilder(args);

// Razor Components
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// HTTP Client base URL must point to my Web API project.
builder.Services.AddScoped(sp => new HttpClient
{
    // IMPORTANT: include trailing slash so relative endpoints combine correctly (e.g., "auth/login")
    BaseAddress = new Uri("https://localhost:5235/")
});

// DI registrations for your services
builder.Services.AddScoped<IUserService, HttpUserService>();
builder.Services.AddScoped<IPostService, HttpPostService>();
builder.Services.AddScoped<ICommentService, HttpCommentService>();

// ---- Blazor authentication/authorization wiring ----
builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<SimpleAuthProvider>(); // allow direct injection
builder.Services.AddScoped<AuthenticationStateProvider>(sp =>
    sp.GetRequiredService<SimpleAuthProvider>());  // share same instance

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseStatusCodePagesWithReExecute("/not-found", createScopeForErrors: true);

app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode(); // interactive server-side

app.Run();
