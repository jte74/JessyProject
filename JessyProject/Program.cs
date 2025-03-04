using JessyProject.Data;
using Microsoft.EntityFrameworkCore;
using Npgsql;

var builder = WebApplication.CreateBuilder(args);
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
// Configuration CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        builder => builder
            .WithOrigins(
                "http://localhost:3000",    // Dev
                "https://www.bluedistrib-challenge.fr"  // Prod
            )
            .AllowAnyMethod()
            .AllowAnyHeader()
            .WithExposedHeaders("Content-Disposition"));
});

// Configuration Contrôleurs et Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

if (builder.Environment.IsDevelopment())
{
    builder.Configuration.AddJsonFile("appsettings.Development.json", optional: true);
}

// Configuration base de données
var databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL")
    ?? throw new InvalidOperationException("DATABASE_URL non configuré");

var databaseUri = new Uri(databaseUrl);
var userInfo = databaseUri.UserInfo.Split(':');

var connectionString = new NpgsqlConnectionStringBuilder
{
    Host = databaseUri.Host,
    Port = databaseUri.Port > 0 ? databaseUri.Port : 5432, // Default PostgreSQL port
    Username = userInfo[0],
    Password = userInfo[1],
    Database = databaseUri.LocalPath.TrimStart('/'),
    SslMode = SslMode.Require,
    TrustServerCertificate = true,
    Pooling = true,
    MinPoolSize = 1,
    MaxPoolSize = 20
}.ToString();

builder.Services.AddDbContext<ClassementDbContext>(options =>
    options.UseNpgsql(
        connectionString,
        o => o.EnableRetryOnFailure(5, TimeSpan.FromSeconds(30), null)
    ));

// Configuration du port pour Render
// var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
// builder.WebHost.UseUrls($"http://*:{port}");

var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
builder.WebHost.UseUrls($"http://0.0.0.0:{port}"); 

var app = builder.Build();

// Middleware
app.UseCors("AllowFrontend");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseDeveloperExceptionPage();
}

app.UseRouting();
app.UseAuthorization();
app.MapControllers();

// Health check minimal
app.MapGet("/health", () => Results.Ok(new { status = "Healthy", timestamp = DateTime.UtcNow }));

// Appliquer les migrations au démarrage
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ClassementDbContext>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

    try
    {
        // Vérifier si la migration a déjà été appliquée
        if (!db.Database.GetAppliedMigrations().Any(m => m == "20250301104915_InitialCreate"))
        {
            logger.LogInformation("Application des migrations...");
            await db.Database.MigrateAsync();
        }
        else
        {
            logger.LogInformation("Migrations déjà à jour");
        }
    }
    catch (Npgsql.PostgresException ex) when (ex.SqlState == "42P07") // Gestion des tables existantes
    {
        logger.LogWarning("La table existe déjà - Mise à jour de l'historique des migrations");
        
        // Ajouter manuellement la migration dans l'historique
        await db.Database.ExecuteSqlRawAsync(
            @"INSERT INTO ""__EFMigrationsHistory"" (""MigrationId"", ""ProductVersion"") 
            VALUES ('20250301104915_InitialCreate', '8.0.5') 
            ON CONFLICT (""MigrationId"") DO NOTHING");
    }
}

app.Run();
