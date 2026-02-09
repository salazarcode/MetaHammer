using MetaHammer.Domain.Interfaces.Repositories;
using MetaHammer.Persistence.Neo4j;
using MetaHammer.Application.Features.Types;
using MetaHammer.API.Seeders;
using MetaHammer.API.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

// Neo4j Configuration
var neo4jUri = builder.Configuration.GetValue<string>("Neo4j:Uri") ?? "bolt://localhost:7687";
var neo4jUser = builder.Configuration.GetValue<string>("Neo4j:User") ?? "neo4j";
var neo4jPass = builder.Configuration.GetValue<string>("Neo4j:Password") ?? "password";

builder.Services.AddSingleton(new Neo4jContext(neo4jUri, neo4jUser, neo4jPass));
builder.Services.AddScoped<IMetaTypeRepository, MetaTypeRepository>();
builder.Services.AddScoped<IMetaObjectRepository, MetaObjectRepository>();
builder.Services.AddScoped<MetaTypeSeeder>();

// MediatR
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(CreateMetaType).Assembly));

builder.Services.AddControllers();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", b => b.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseExceptionHandler();
app.UseCors("AllowAll");
app.UseAuthorization();
app.MapControllers();

// Seed Data
using (var scope = app.Services.CreateScope())
{
    var seeder = scope.ServiceProvider.GetRequiredService<MetaTypeSeeder>();
    await seeder.SeedAsync();
}

app.Run();
