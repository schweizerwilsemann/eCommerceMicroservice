using eCommerce.SharedLibrary.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProductApi.Application.Interfaces;
using ProductApi.Infrastructure.Data;
using ProductApi.Infrastructure.Repositories;

namespace ProductApi.Infrastructure.DependencyInjection
{
    public static class ServiceContainer
    {
        public static IServiceCollection AddInfrastructureService(this IServiceCollection services, IConfiguration config)
        {
            // add database connectivity
            // add authentication scheme
            SharedServiceContainer.AddSharedServices<ProductDbContext>(services, config, config["My serilog: FileName"]!);


            // create dependency injection
            services.AddScoped<IProduct, ProductRepository>();
            return services;
        }

        public static IApplicationBuilder UseInfrastructurePolicy (this IApplicationBuilder app)
        {
            // register middleware : Global Exception - Internal error ,  Listen to only API Gateway - Block all ousider calls
            SharedServiceContainer.UseSharedPolicies(app);
            return app;
        }
    }
}
