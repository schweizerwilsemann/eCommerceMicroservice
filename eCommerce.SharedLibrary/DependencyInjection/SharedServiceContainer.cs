
using eCommerce.SharedLibrary.Middleware;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace eCommerce.SharedLibrary.DependencyInjection
{
    public static class SharedServiceContainer
    {
        public static IServiceCollection AddSharedServices<IContext>(this IServiceCollection services, IConfiguration config, string fileName) where IContext : DbContext
        {
            // add generic database context
            services.AddDbContext<IContext> (option => 
                option.UseSqlServer(config.GetConnectionString("eCommerceConnection"), sqlserverOption => 
                sqlserverOption.EnableRetryOnFailure()
            ));

            // config serilog logging
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.Debug()
                .WriteTo.Console()
                .WriteTo.File(path: $"{ fileName }-.text",
                restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Information,
                outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {message: lj}{NewLine} {Exception}", 
                rollingInterval: RollingInterval.Day)
                
                .CreateLogger();
            // add Jwt scheme authentication
            JwtAuthenticationScheme.AddJwtAuthenticationScheme(services, config);

            return services;
        }

        public static IApplicationBuilder UseSharedPolicies(this IApplicationBuilder app)
        {
            // use global exception
            app.UseMiddleware<GlobalException>();

            // Register middleware to block all outsiders api calls
            app.UseMiddleware<ListenToOnlyApiGateway>();
            return app;
        }
    }
}
