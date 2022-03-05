using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

public static class StartupSetup
{
    public static void AddDbContext(this IServiceCollection services, string accountEndpoint, string accountKey) =>
        services.AddDbContext<ModStatsContext>(c =>
            c.UseCosmos(accountEndpoint, accountKey, databaseName: "ICModsStatistics"));
    
}
