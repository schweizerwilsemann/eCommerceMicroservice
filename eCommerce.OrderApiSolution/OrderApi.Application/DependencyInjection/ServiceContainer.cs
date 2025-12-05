using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OrderApi.Application.Services;
using Polly.Retry;
using Polly;
using eCommerce.SharedLibrary.Logs;


namespace OrderApi.Application.DependencyInjection
{
    public static class ServiceContainer
    {
        public static IServiceCollection AddApplicationService(this IServiceCollection services, IConfiguration config)
        {
            // Register Http Client
            // create DI
            services.AddHttpClient<IOrderService, OrderService>(options =>
            {
                options.BaseAddress = new Uri(config["ApiGateway:BaseAddress"]!);
                // Increase default timeout to avoid cancelling calls to downstream services
                // that may take longer than 1 second. Adjust as needed for your environment.
                options.Timeout = TimeSpan.FromSeconds(30);
            });

            // create retry strategy
            var retryStrategy = new RetryStrategyOptions()
            {
                ShouldHandle = new PredicateBuilder().Handle<TaskCanceledException>(),
                BackoffType = DelayBackoffType.Constant,
                UseJitter = true,
                MaxRetryAttempts = 3,
                Delay = TimeSpan.FromMilliseconds(500),
                OnRetry = args => {
                    string message = $"OnRetry, attemp {args.AttemptNumber} Outcome {args.Outcome}";
                    LogExceptions.LogToConsole(message);
                    LogExceptions.LogToDebugger(message);
                    return ValueTask.CompletedTask;
                }
            };


            // use strategy
            services.AddResiliencePipeline("my-retry-pipeline", builder =>
            {
                builder.AddRetry(retryStrategy);
            });

            return services;
        }
    }
}
