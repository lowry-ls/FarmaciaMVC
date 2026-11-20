
using Npgsql;
using FarmaciaMVC.Data;

var builder = WebApplication.CreateBuilder(args);

// Configurar los servicios MVC
builder.Services.AddControllersWithViews();
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
if (string.IsNullOrWhiteSpace(connectionString))
{
    builder.Services.AddScoped<IDashboardRepository, DemoDashboardRepository>();
}
else
{
    builder.Services.AddSingleton(_ => NpgsqlDataSource.Create(connectionString));
    builder.Services.AddScoped<IDashboardRepository, PostgresDashboardRepository>();
}

var app = builder.Build();

// Configuracion del entorno
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}


// Configuracion de la aplicacion
app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthorization();

app.MapStaticAssets();

// Ruta principal MVC
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

if (app.Environment.IsDevelopment())
{
    app.MapGet("/db-check", async (
        IConfiguration configuration,
        ILogger<Program> logger) =>
    {
        if (string.IsNullOrWhiteSpace(configuration.GetConnectionString("DefaultConnection")))
        {
            logger.LogWarning(
                "Database connection check skipped because ConnectionStrings:DefaultConnection is not configured.");

            return Results.Problem(
                title: "Database connection unavailable.",
                statusCode: StatusCodes.Status503ServiceUnavailable
            );
        }

        try
        {
            await using var dataSource = NpgsqlDataSource.Create(
                configuration.GetConnectionString("DefaultConnection")!);
            await using var connection = await dataSource.OpenConnectionAsync();

            return Results.Ok("Database connection successful.");
        }
        catch (NpgsqlException ex)
        {
            logger.LogError(ex, "Database connection check failed.");

            return Results.Problem(
                title: "Database connection unavailable.",
                statusCode: StatusCodes.Status503ServiceUnavailable
            );
        }
        catch (ArgumentException ex)
        {
            logger.LogError(ex, "Database connection string is invalid.");

            return Results.Problem(
                title: "Database connection unavailable.",
                statusCode: StatusCodes.Status503ServiceUnavailable
            );
        }
    });
}

app.Run();
