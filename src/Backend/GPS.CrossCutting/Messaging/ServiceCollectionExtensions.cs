using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using System.Security.Authentication;

namespace GPS.CrossCutting.Messaging
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adiciona MassTransit com RabbitMQ e descobre automaticamente os consumers no assembly especificado.
        /// Esta é a abordagem recomendada quando os consumers estão em um assembly diferente do Program.cs.
        /// </summary>
        /// <param name="services">Coleção de serviços</param>
        /// <param name="configuration">Configuração da aplicação</param>
        /// <param name="assemblies">Assemblies onde procurar por consumers</param>
        /// <returns>Coleção de serviços configurada</returns>
        public static IServiceCollection AddMassTransitWithRabbitMqFromAssemblies(
            this IServiceCollection services, 
            IConfiguration configuration,
            params Assembly[] assemblies)
        {
            return services.AddMassTransitWithRabbitMq(configuration, x => 
            {
                // Adiciona todos os consumers encontrados nos assemblies especificados
                foreach (var assembly in assemblies)
                {
                    x.AddConsumers(assembly);
                }
            });
        }

        /// <summary>
        /// Adiciona MassTransit com RabbitMQ e descobre automaticamente os consumers no assembly que contém o tipo especificado.
        /// Use esta sobrecarga quando precisar especificar explicitamente um assembly diferente.
        /// </summary>
        /// <typeparam name="TAssemblyMarker">Tipo que será usado para identificar o assembly onde buscar os consumers</typeparam>
        /// <param name="services">Coleção de serviços</param>
        /// <param name="configuration">Configuração da aplicação</param>
        /// <returns>Coleção de serviços configurada</returns>
        public static IServiceCollection AddMassTransitWithRabbitMq<TAssemblyMarker>(this IServiceCollection services, IConfiguration configuration)
        {
            var assembly = typeof(TAssemblyMarker).Assembly;
            return services.AddMassTransitWithRabbitMq(configuration, x => x.AddConsumers(assembly));
        }

        /// <summary>
        /// Adiciona MassTransit com RabbitMQ e permite configuração manual dos consumers.
        /// </summary>
        /// <param name="services">Coleção de serviços</param>
        /// <param name="configuration">Configuração da aplicação</param>
        /// <param name="configureConsumers">Ação para configurar consumers manualmente</param>
        /// <returns>Coleção de serviços configurada</returns>
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
