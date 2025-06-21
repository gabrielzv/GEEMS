var builder = WebApplication.CreateBuilder(args);

// Configuración de servicios
builder.Services.AddControllers();
builder.Services.AddScoped<CalculatorService>();

var app = builder.Build();

app.UseHttpsRedirection();

// Middleware de autenticación con clave estática
app.Use(async (context, next) =>
{
    const string staticApiKey = "Tralalerotralala";

    // Verificar la clave en el header
    if (!context.Request.Headers.TryGetValue("API-KEY", out var receivedApiKey) ||
        receivedApiKey != staticApiKey)
    {
        context.Response.StatusCode = 401; // Unauthorized
        await context.Response.WriteAsync("Clave API no válida");
        return;
    }

    await next();
});

app.MapControllers();

app.Run();