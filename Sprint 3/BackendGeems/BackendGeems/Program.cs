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

builder.Services.AddScoped<IGeneralRepo, GeneralRepo>();
builder.Services.AddScoped<IPagoRepo, PagoRepo>();
builder.Services.AddScoped<IHorasRepo, HorasRepo>();
builder.Services.AddScoped<IQueryPago, QueryPago>();
builder.Services.AddScoped<ISalarioBruto, SalarioBruto>();
builder.Services.AddScoped<IGenerarPago, GenerarPago>();
builder.Services.AddScoped<IQueryHoras, QueryHoras>();
builder.Services.AddScoped<ServicioCalculoPago>();
builder.Services.AddScoped<GestorPagosService>();
builder.Services.AddScoped<IQueryBeneficio, QueryBeneficio>();
builder.Services.AddScoped<IBeneficioRepo, BeneficioRepo>();
builder.Services.AddScoped<CorreoSender>();
builder.Services.AddScoped<ReporteService>();



var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseCors();
app.MapControllers();
app.Run();