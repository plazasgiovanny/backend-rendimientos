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

// Historial de 30 dias calculado para promediar ~$13/dia, consistente con
// la evidencia citada en el documento academico del equipo (La Nacion:
// "$10.000 al 33% E.A. genera ~$13/dia"). Ya no es un array fijo repetido
// para cualquier filtro: los ultimos 7 (31 jul - 6 ago) suman $91 (promedio
// $13.0/dia exacto), los ultimos 15 (23 jul - 6 ago) suman $193 (promedio
// $12.87) y los 30 completos suman $389 (promedio $12.97). La hora de
// acreditacion es ilustrativa (mismo estilo que el mock: ~10am con
// variacion de minutos) -- el caso de negocio solo dice "se acredita en
// dias habiles", no especifica una hora exacta real. Offset -05:00 fijo
// (Colombia, sin horario de verano) para que la hora mostrada no dependa
// de la zona horaria del navegador de quien mire la pantalla. Orden
// ascendente (mas viejo primero).
var historial30Dias = new[]
{
    new RendimientoDiario("2026-07-08T09:58:00-05:00", 14),
    new RendimientoDiario("2026-07-09T10:03:00-05:00", 11),
    new RendimientoDiario("2026-07-10T10:02:00-05:00", 16),
    new RendimientoDiario("2026-07-11T09:59:00-05:00", 10),
    new RendimientoDiario("2026-07-12T10:07:00-05:00", 13),
    new RendimientoDiario("2026-07-13T09:55:00-05:00", 15),
    new RendimientoDiario("2026-07-14T10:01:00-05:00", 12),
    new RendimientoDiario("2026-07-15T09:58:00-05:00", 14),
    new RendimientoDiario("2026-07-16T10:03:00-05:00", 11),
    new RendimientoDiario("2026-07-17T10:02:00-05:00", 16),
    new RendimientoDiario("2026-07-18T09:59:00-05:00", 10),
    new RendimientoDiario("2026-07-19T10:07:00-05:00", 13),
    new RendimientoDiario("2026-07-20T09:55:00-05:00", 15),
    new RendimientoDiario("2026-07-21T10:01:00-05:00", 12),
    new RendimientoDiario("2026-07-22T09:58:00-05:00", 14),
    new RendimientoDiario("2026-07-23T10:03:00-05:00", 11),
    new RendimientoDiario("2026-07-24T10:02:00-05:00", 16),
    new RendimientoDiario("2026-07-25T09:59:00-05:00", 10),
    new RendimientoDiario("2026-07-26T10:07:00-05:00", 13),
    new RendimientoDiario("2026-07-27T09:55:00-05:00", 15),
    new RendimientoDiario("2026-07-28T10:01:00-05:00", 12),
    new RendimientoDiario("2026-07-29T09:58:00-05:00", 14),
    new RendimientoDiario("2026-07-30T10:03:00-05:00", 11),
    new RendimientoDiario("2026-07-31T10:02:00-05:00", 16),
    new RendimientoDiario("2026-08-01T09:59:00-05:00", 10),
    new RendimientoDiario("2026-08-02T10:07:00-05:00", 13),
    new RendimientoDiario("2026-08-03T09:55:00-05:00", 15),
    new RendimientoDiario("2026-08-04T10:01:00-05:00", 12),
    new RendimientoDiario("2026-08-05T09:58:00-05:00", 14),
    new RendimientoDiario("2026-08-06T10:03:00-05:00", 11),
};

// Equivalente a getInfoCuenta() en rendimientosService.ts. NO expone
// rendimientoAcumulado: eso lo calcula el Store del frontend a partir del
// historial, para mantener la consistencia maestro-detalle por diseno.
app.MapGet("/api/cuenta", () => new InfoCuenta(10000m, 33m));

// Equivalente a getHistorial(dias). Devuelve los ultimos N elementos del
// dataset de 30 dias (los mas recientes), manteniendo el orden ascendente
// dentro de la respuesta. El frontend solo pide 7/15/30; cualquier otro
// valor cae al fallback de 7 dias con un warning en el log.
app.MapGet("/api/historial", (int dias = 7) =>
{
    var diasValidos = dias is 7 or 15 or 30 ? dias : 7;
    if (diasValidos != dias)
    {
        app.Logger.LogWarning(
            "Dataset de {Dias} dias no soportado, se devuelve el de 7.", dias);
    }
    return historial30Dias.TakeLast(diasValidos).ToArray();
});

app.Run();

record InfoCuenta(decimal SaldoTotal, decimal TasaAnualPorcentaje);
record RendimientoDiario(string FechaHora, decimal Monto);
