var builder = WebApplication.CreateBuilder(args);

// El frontend (Vite) corre en http://localhost:5173 por defecto.
const string CorsPolicy = "FrontendDev";
builder.Services.AddCors(options =>
{
    options.AddPolicy(CorsPolicy, policy =>
    {
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();
app.UseCors(CorsPolicy);

// Mismos datos que el mock visual de U2 (mock_rendimientos.html):
// historial de 7 dias que suma exactamente $321.
var historial7Dias = new[]
{
    new RendimientoDiario("2026-07-31", 47),
    new RendimientoDiario("2026-08-01", 45),
    new RendimientoDiario("2026-08-02", 44),
    new RendimientoDiario("2026-08-03", 43),
    new RendimientoDiario("2026-08-04", 46),
    new RendimientoDiario("2026-08-05", 45),
    new RendimientoDiario("2026-08-06", 51),
};

// Equivalente a getInfoCuenta() en rendimientosService.ts. NO expone
// rendimientoAcumulado: eso lo calcula el Store del frontend a partir del
// historial, para mantener la consistencia maestro-detalle por diseno.
app.MapGet("/api/cuenta", () => new InfoCuenta(10000m, 33m));

// Equivalente a getHistorial(dias). Solo existe el dataset de 7 dias por
// ahora, igual que en el frontend (mismo alcance minimo).
app.MapGet("/api/historial", (int dias = 7) =>
{
    if (dias != 7)
    {
        app.Logger.LogWarning(
            "Dataset de {Dias} dias no implementado aun, se devuelve el de 7.", dias);
    }
    return historial7Dias;
});

app.Run();

record InfoCuenta(decimal SaldoTotal, decimal TasaAnualPorcentaje);
record RendimientoDiario(string Fecha, decimal Monto);
