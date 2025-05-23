
using BackendGeems.Application;
using BackendGeems.Infraestructure;

var builder = WebApplication.CreateBuilder(args);

// Agregar Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("http://localhost:8080")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IGEEMSRepo, GEEMSRepo>();
builder.Services.AddScoped<IQueryPago,QueryPago>();
builder.Services.AddScoped<ISalarioBruto, SalarioBruto>();

var app = builder.Build();

// Usar Swagger en desarrollo
app.UseSwagger();
app.UseSwaggerUI();

app.UseCors();
app.MapControllers();
app.Run();
