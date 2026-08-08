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
// historial de 7 dias que suma exactamente $321. La hora de acreditacion
// es ilustrativa (mismo estilo que el mock: ~10am con variacion de
// minutos) -- el caso de negocio solo dice "se acredita en dias habiles",
// no especifica una hora exacta real. Offset -05:00 fijo (Colombia, sin
// horario de verano) para que la hora mostrada no dependa de la zona
// horaria del navegador de quien mire la pantalla.
var historial7Dias = new[]
{
    new RendimientoDiario("2026-07-31T10:03:00-05:00", 47),
    new RendimientoDiario("2026-08-01T09:58:00-05:00", 45),
    new RendimientoDiario("2026-08-02T10:01:00-05:00", 44),
    new RendimientoDiario("2026-08-03T09:55:00-05:00", 43),
    new RendimientoDiario("2026-08-04T10:07:00-05:00", 46),
    new RendimientoDiario("2026-08-05T09:59:00-05:00", 45),
    new RendimientoDiario("2026-08-06T10:02:00-05:00", 51),
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
record RendimientoDiario(string FechaHora, decimal Monto);
