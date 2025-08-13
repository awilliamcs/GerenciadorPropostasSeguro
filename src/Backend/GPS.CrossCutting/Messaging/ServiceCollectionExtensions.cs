using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Security.Authentication;

namespace GPS.CrossCutting.Messaging
{
    public static class ServiceCollectionExtensions
    {
       
        public static IServiceCollection AddMassTransitWithRabbitMq(this IServiceCollection services, IConfiguration configuration, Action<IBusRegistrationConfigurator>? configureConsumers = null)
        {
            services.AddMassTransit(x =>
            {
                configureConsumers?.Invoke(x);

                x.UsingRabbitMq((context, cfg) =>
                {
                    var rabbitMqConfig = configuration.GetSection("RabbitMq");

                    var host = rabbitMqConfig.GetValue<string>("Host") ?? "localhost";
                    var port = rabbitMqConfig.GetValue<ushort>("Port", 5672);
                    var virtualHost = rabbitMqConfig.GetValue<string>("VirtualHost", "/");
                    var username = rabbitMqConfig.GetValue<string>("Username") ?? "guest";
                    var password = rabbitMqConfig.GetValue<string>("Password") ?? "guest";
                    var useSsl = rabbitMqConfig.GetValue<bool>("UseSsl", false);

                    cfg.Host(host, port, virtualHost, h =>
                    {
                        h.Username(username);
                        h.Password(password);

                        if (useSsl)
                        {
                            h.UseSsl(s =>
                            {
                                s.Protocol = SslProtocols.Tls12;
                                s.ServerName = host;
                            });
                        }

                        var heartbeat = rabbitMqConfig.GetValue<ushort>("RequestedHeartbeat", 60);
                        var requestTimeout = rabbitMqConfig.GetValue<TimeSpan>("RequestTimeout", TimeSpan.FromSeconds(30));

                        h.Heartbeat(heartbeat);
                        h.RequestedConnectionTimeout(requestTimeout);
                    });

                    cfg.ConfigureEndpoints(context);
                });
            });

            services.Configure<MassTransitHostOptions>(options =>
            {
                options.WaitUntilStarted = true;
                options.StartTimeout = TimeSpan.FromSeconds(30);
                options.StopTimeout = TimeSpan.FromMinutes(1);
            });

            services.AddScoped<IClienteMQ, MassTransitMQClient>();
            return services;
        }
    }
}
