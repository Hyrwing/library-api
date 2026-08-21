using FluentValidation;
using LibraryApi.Api.Endpoints;
using LibraryApi.Api.Infrastructure;
using LibraryApi.Application.Services;
using LibraryApi.Application.Validators;
using LibraryApi.Infrastructure;
using LibraryApi.Infrastructure.Persistence;
using LibraryApi.Infrastructure.Seeds;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();
builder.Services.AddHttpLogging(o => { });

builder.Logging.ClearProviders();
builder.Logging.AddConsole();
if (!builder.Environment.IsDevelopment())
    builder.Logging.AddJsonConsole();

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddScoped<BookService>();
builder.Services.AddScoped<LoanService>();
builder.Services.AddValidatorsFromAssemblyContaining<CreateBookRequestValidator>();

var app = builder.Build();

app.UseMiddleware<CorrelationIdMiddleware>();
app.UseExceptionHandler();
app.UseHttpLogging();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapBooks();
app.MapLoans();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<LibraryDbContext>();
    if (db.Database.IsSqlite())
        await db.Database.MigrateAsync();
    else
        await db.Database.EnsureCreatedAsync();
}
await DataSeeder.SeedAsync(app.Services);

app.Run();

public partial class Program { }
