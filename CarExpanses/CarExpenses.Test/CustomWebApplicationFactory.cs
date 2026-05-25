using CarExpenses.DAL;
using Google;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Data.Common;

namespace CarExpenses.Test;

public class CustomWebApplicationFactory<TProgram>
: WebApplicationFactory<TProgram> where TProgram : class
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services => {
            var dbContextDescriptor = services.SingleOrDefault(
                d => d.ServiceType ==
                    typeof(IDbContextOptionsConfiguration<CarExpesesDbContext>));

            services.Remove(dbContextDescriptor);

            var dbConnectionDescriptor = services.SingleOrDefault(
                d => d.ServiceType ==
                    typeof(DbConnection));

            services.Remove(dbConnectionDescriptor);

            services.AddDbContext<CarExpesesDbContext>(options =>
                { options.UseInMemoryDatabase("CarExpesesDbTest"); });
            }
        );

        builder.ConfigureAppConfiguration((context, config) => { 
            var testSettings = new Dictionary<string, string> { 
                ["ConnectionStrings:DefaultConnection"] = "CarExpesesDbTest", 
                ["GoogleClientId"] = "test-google-client-id", 
                ["GoogleClientSecret"] = "test-google-client-secret" }; 
            config.AddInMemoryCollection(testSettings); 
        });

        builder.ConfigureTestServices(services =>
        {
            services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = TestAuthHandler.SchemeName;
                    options.DefaultChallengeScheme = TestAuthHandler.SchemeName;
                    options.DefaultScheme = TestAuthHandler.SchemeName;
                })
                .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(
                    TestAuthHandler.SchemeName,
                    _ => { });
        });

        builder.UseEnvironment("Development");
    }

    public HttpClient CreateAuthenticatedClient(string? roles = null, int userId = 1)
    {
        var client = CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });

        client.DefaultRequestHeaders.Add(TestAuthHandler.UserIdHeader, userId.ToString());
        if (!string.IsNullOrWhiteSpace(roles))
        {
            client.DefaultRequestHeaders.Add(TestAuthHandler.RolesHeader, roles);
        }

        return client;
    }
}
