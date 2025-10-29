using Microsoft.EntityFrameworkCore;
using ScaleTrackAPI.Database;

namespace ScaleTrackAPI.Extensions
{
    public static class DatabaseServiceExtensions
    {
        public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration config)
        {
            var connectionString = Environment.GetEnvironmentVariable("DB_CONNECTION")
                                   ?? config.GetConnectionString("DefaultConnection")
                                   ?? "Data Source=Database/ScaleTrack.db";

            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlite(connectionString));

            return services;
        }
    }
}
