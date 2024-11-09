using Microsoft.Extensions.DependencyInjection;
using MassTransit;
using System.Reflection;
using Board.Common.Settings;
using Microsoft.Extensions.Configuration;

namespace Board.Common.RabbitMQ
{
    public static class Extensions
    {
        public static IServiceCollection AddMassTransitWithRabbitMQ(this IServiceCollection services)
        {

            services.AddMassTransit(configure =>
            {
                configure.AddConsumers(Assembly.GetEntryAssembly());
                configure.UsingRabbitMq((context, configurator) =>
                {
                    var configuration = context.GetService<IConfiguration>();
                    var rabbitMQSettings = configuration!.GetSection(nameof(RabbitMQSettings)).Get<RabbitMQSettings>();
                    var serviceSettings = configuration.GetSection(nameof(ServiceSettings)).Get<ServiceSettings>();


                    if (rabbitMQSettings == null || string.IsNullOrEmpty(rabbitMQSettings.Host))
                    {
                        throw new Exception("RabbitMQ settings are not properly configured.");
                    }

                    configurator.Host(rabbitMQSettings.Host);
                    configurator.UseJsonSerializer();
                    configurator.ConfigureEndpoints(context, new KebabCaseEndpointNameFormatter(serviceSettings!.ServiceName, false));
                });
            });
                        // services.AddMassTransitHostedService();
                        return services;
        }
    }
}