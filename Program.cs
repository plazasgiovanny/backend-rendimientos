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

// Tabla de 30 dias calculada para promediar ~$13/dia (evidencia citada en
// el documento academico: $10.000 al 33% E.A.). Offset -05:00 fijo (Colombia).
var thirtyDayHistory = new[]
{
    new DailyReturn("2026-07-08T09:58:00-05:00", 14),
    new DailyReturn("2026-07-09T10:03:00-05:00", 11),
    new DailyReturn("2026-07-10T10:02:00-05:00", 16),
    new DailyReturn("2026-07-11T09:59:00-05:00", 10),
    new DailyReturn("2026-07-12T10:07:00-05:00", 13),
    new DailyReturn("2026-07-13T09:55:00-05:00", 15),
    new DailyReturn("2026-07-14T10:01:00-05:00", 12),
    new DailyReturn("2026-07-15T09:58:00-05:00", 14),
    new DailyReturn("2026-07-16T10:03:00-05:00", 11),
    new DailyReturn("2026-07-17T10:02:00-05:00", 16),
    new DailyReturn("2026-07-18T09:59:00-05:00", 10),
    new DailyReturn("2026-07-19T10:07:00-05:00", 13),
    new DailyReturn("2026-07-20T09:55:00-05:00", 15),
    new DailyReturn("2026-07-21T10:01:00-05:00", 12),
    new DailyReturn("2026-07-22T09:58:00-05:00", 14),
    new DailyReturn("2026-07-23T10:03:00-05:00", 11),
    new DailyReturn("2026-07-24T10:02:00-05:00", 16),
    new DailyReturn("2026-07-25T09:59:00-05:00", 10),
    new DailyReturn("2026-07-26T10:07:00-05:00", 13),
    new DailyReturn("2026-07-27T09:55:00-05:00", 15),
    new DailyReturn("2026-07-28T10:01:00-05:00", 12),
    new DailyReturn("2026-07-29T09:58:00-05:00", 14),
    new DailyReturn("2026-07-30T10:03:00-05:00", 11),
    new DailyReturn("2026-07-31T10:02:00-05:00", 16),
    new DailyReturn("2026-08-01T09:59:00-05:00", 10),
    new DailyReturn("2026-08-02T10:07:00-05:00", 13),
    new DailyReturn("2026-08-03T09:55:00-05:00", 15),
    new DailyReturn("2026-08-04T10:01:00-05:00", 12),
    new DailyReturn("2026-08-05T09:58:00-05:00", 14),
    new DailyReturn("2026-08-06T10:03:00-05:00", 11),
};

// No expone accumulatedReturns: eso lo calcula el Store del frontend a
// partir del historial, para mantener la consistencia maestro-detalle.
app.MapGet("/api/account", () => new AccountInfo(10000m, 33m));

// Devuelve los ultimos N dias del dataset (orden ascendente). Un "days"
// fuera de {7,15,30} cae al fallback de 7 con un warning en el log.
app.MapGet("/api/history", (int days = 7) =>
{
    var validDays = days is 7 or 15 or 30 ? days : 7;
    if (validDays != days)
    {
        app.Logger.LogWarning(
            "Dataset de {Days} dias no soportado, se devuelve el de 7.", days);
    }
    return thirtyDayHistory.TakeLast(validDays).ToArray();
});

app.Run();

record AccountInfo(decimal TotalBalance, decimal AnnualRatePercentage);
record DailyReturn(string DateTime, decimal Amount);
