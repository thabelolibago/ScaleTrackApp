# ScaleTrackApp

ScaleTrackApp is an issue tracking system built with ASP.NET Core and Entity Framework Core.

## Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [Node.js & npm](https://nodejs.org/) (if you plan to use frontend features)
- SQLite (used by default for local development)
- Visual Studio 2022+ or VS Code

## Project Structure

- `ScaleTrackAPI/` - Main backend API project
- `Documentations/` - Sequence diagrams, entity docs, API response samples

## Setup

1. **Clone the repository**
   ```sh
   git clone <your-repo-url>
   cd ScaleTrackApp
   ```

2. **Configure environment variables**
   - Copy `.env.example` to `.env` in `ScaleTrackAPI/` and update values as needed.
   - You can also edit `appsettings.json` and `appsettings.Development.json` for DB connection and JWT settings.

3. **Install dependencies**
   ```sh
   cd ScaleTrackAPI
   dotnet restore
   ```

4. **Apply database migrations**
   ```sh
   dotnet ef database update
   ```
   This will create the SQLite database at `ScaleTrackAPI/ScaleTrack.db`.

5. **Run the API**
   ```sh
   dotnet run
   ```
   The API will be available at [http://localhost:5165](http://localhost:5165) (or as configured).

6. **API Documentation**
   - Swagger UI is available at `/swagger` when running the API.

## Development

- **Unit tests:** Add your tests in the `ScaleTrackAPI.Tests/` folder (if present).
- **Controllers:** API endpoints are in `ScaleTrackAPI/Controllers/`.
- **Services:** Business logic in `ScaleTrackAPI/Services/`.
- **Models & DTOs:** Data models and transfer objects in `ScaleTrackAPI/Models/` and `ScaleTrackAPI/DTOs/`.

## Useful Commands

- Build: `dotnet build`
- Run: `dotnet run`
- Migrate DB: `dotnet ef database update`
- Add Migration: `dotnet ef migrations add <MigrationName>`

## Troubleshooting

- If you get DB connection errors, check your `.env` or `appsettings.json`.
- For authentication, ensure JWT settings are correct and the secret key is set.

## License

MIT (or specify your license)

---

For more details, see [Documentations/API_Responses.md](Documentations/API_Responses.md) and [Documentations/Entities.md](Documentations/Entities.md)