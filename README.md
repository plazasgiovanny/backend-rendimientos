# Backend Rendimientos — Actividad Sumativa U3 (Arquitectura Frontend)

Backend mínimo (C# / .NET 9, minimal API) que simula el servidor real detrás del módulo **Rendimientos**. Existe para que el prototipo de React (`prototipo-rendimientos/`, repo hermano) haga una llamada HTTP real en vez de leer datos mock en memoria — así el patrón Flux se implementa fiel al ejemplo oficial de Facebook (`flux-async`), donde el Store reacciona a datos que llegan de un servidor real, no de una función local.

## Cómo correrlo

```bash
dotnet run
```

Queda escuchando en `http://localhost:5193`. CORS está habilitado solo para `http://localhost:5173` (el dev server de Vite del frontend).

## Endpoints

| Método | Ruta | Respuesta |
|---|---|---|
| `GET` | `/api/account` | `{ "totalBalance": 10000, "annualRatePercentage": 33 }` |
| `GET` | `/api/history?days=7` | `[{ "dateTime": "2026-07-31T10:02:00-05:00", "amount": 16 }, ...]` (últimos 7 días, suman $91) |

`days` acepta 7, 15 o 30, y cada valor devuelve un dataset real y distinto. Los tres se recortan de una misma tabla de 30 días (2026-07-08 a 2026-08-06) calculada para promediar ~$13/día: los últimos 7 suman $91, los últimos 15 suman $193 y los 30 completos suman $389. Un `days` fuera de {7, 15, 30} cae al fallback de 7 días con un warning en el log.

`dateTime` lleva el offset de Colombia (`-05:00`) explícito, no una fecha suelta — así el cliente nunca tiene que adivinar en qué zona horaria interpretar el dato.

Nota: `/api/account` **no** devuelve `accumulatedReturns` — ese valor lo calcula el Store del frontend a partir del historial recibido, para mantener la consistencia maestro-detalle por diseño (ver `prototipo-rendimientos/src/stores/returnsStore.ts`).

## Alcance

Mínimo a propósito (dado el plazo de la entrega): solo estos dos endpoints, sin base de datos, sin simulación de latencia/errores de red (a diferencia del ejemplo oficial `flux-async`, que sí las incluye). Los datos son una tabla de 30 días calculada para reproducir la evidencia citada en el documento académico del equipo (~$13/día para $10.000 al 33% E.A.), no los 7 valores fijos que había antes.

El código (records, propiedades, variables) está en inglés siguiendo convención estándar de desarrollo, coordinado con los tipos del frontend (`AccountInfo`, `DailyReturn`) para que el contrato JSON entre ambos repos use las mismas claves.
