using Microsoft.EntityFrameworkCore;
using FinancialServices.Api.Infrastructure.Persistence;

public static class TestHelpers
{
    public static AppDbContext CreateContext(string dbName)
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: dbName)
            .Options;
        return new AppDbContext(options);
    }
}
