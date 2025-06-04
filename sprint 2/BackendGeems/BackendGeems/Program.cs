using BackendGeems.Application;
using BackendGeems.Infraestructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("http://localhost:8080")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IGEEMSRepo, GEEMSRepo>();
builder.Services.AddScoped<IGEEMSPagoRepo, GEEMSPagoRepo>();
builder.Services.AddScoped<IGEEMSHorasRepo, GEEMSHorasRepo>();
builder.Services.AddScoped<IQueryPago, QueryPago>();
builder.Services.AddScoped<ISalarioBruto, SalarioBruto>();
builder.Services.AddScoped<IGenerarPago, GenerarPago>();
builder.Services.AddScoped<IQueryHoras, QueryHoras>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseCors();
app.MapControllers();
app.Run();