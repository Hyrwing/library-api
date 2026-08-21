using LibraryApi.Infrastructure.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace LibraryApi.Api.Tests.Infrastructure;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly string _databaseName = $"TestDb-{Guid.NewGuid()}";

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            services.RemoveAll<LibraryDbContext>();
            services.RemoveAll<DbContextOptions<LibraryDbContext>>();

            var optionsConfigurations = services
                .Where(d => d.ServiceType.IsGenericType
                    && d.ServiceType.Name == "IDbContextOptionsConfiguration`1"
                    && d.ServiceType.GenericTypeArguments[0] == typeof(LibraryDbContext))
                .ToList();

            foreach (var descriptor in optionsConfigurations)
                services.Remove(descriptor);

            services.AddDbContext<LibraryDbContext>(options =>
                options.UseInMemoryDatabase(_databaseName));
        });
        builder.UseEnvironment("Development");
    }
}
