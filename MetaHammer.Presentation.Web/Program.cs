using Infrastructure.Repository.Neo4j;
using Infrastructure.Repository.Neo4j.Configuration;
using Infrastructure.Repository.Neo4j.Interfaces;
using MetaHammer.Domain.Interfaces.Repositories;
using MetaHammer.Presentation.Web.Components;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.Configure<Neo4JOptions>(builder.Configuration.GetSection("Neo4J"));
builder.Services.AddSingleton<INeo4JDataAccess, Neo4JDataAccess>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();