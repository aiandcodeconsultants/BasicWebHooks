using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Options;

namespace BasicWebHooks.EF;

/// <summary>
/// A factory for creating <see cref="BasicWebHooksDbContext"/>.
/// </summary>
public class BasicWebHooksDbContextFactory : IDesignTimeDbContextFactory<BasicWebHooksDbContext>
{
    /// <inheritdoc />
    public BasicWebHooksDbContext CreateDbContext(string[] args)
    {
        var builder = new DbContextOptionsBuilder<BasicWebHooksDbContext>();
        
        // use LocalDb by default for migrations
        var connectionString = "Server=(localdb)\\mssqllocaldb;Database=BasicWebHooks;Trusted_Connection=True;MultipleActiveResultSets=true";
        
        _ = builder.UseSqlServer(
            connectionString,
            options => options
                        .MigrationsAssembly(GetType().Assembly.FullName)
                        .UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery));

        return new BasicWebHooksDbContext(builder.Options);
    }
}
