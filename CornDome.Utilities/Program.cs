using CornDome.Repository;
using CornDome.Utilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

// Build configuration
var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .Build();

// Setup DI container
var services = new ServiceCollection();

// Make IConfiguration injectable
services.AddSingleton<IConfiguration>(configuration);

// Register DbContext
services.AddDbContext<CardDatabaseContext>(options =>
{
    var connectionString = configuration.GetConnectionString("CardsDb");
    options.UseSqlite(connectionString + ";Pooling=False");
});

services.AddTransient<App>();

// Build service provider
var serviceProvider = services.BuildServiceProvider();

// Example usage
using var scope = serviceProvider.CreateScope();
var db = scope.ServiceProvider.GetRequiredService<CardDatabaseContext>();

// Run app
var app = serviceProvider.GetRequiredService<App>();
await app.RunAsync();