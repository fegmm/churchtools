using Fegmm.ChurchTools;
using Microsoft.Kiota.Http.HttpClientLibrary;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Extension methods for setting up ChurchTools client in an <see cref="IServiceCollection"/>.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds the <see cref="ChurchToolsClient"/> to the specified <see cref="IServiceCollection"/> 
    /// following the Kiota dependency injection pattern.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the services to.</param>
    /// <param name="configureOptions">An action to configure the <see cref="ChurchToolsOptions"/>.</param>
    /// <param name="configureClient">An action to configure the internal used <see cref="HttpClient"/>.</param>
    /// <returns>The <see cref="IServiceCollection"/>.</returns>
    public static IServiceCollection AddChurchToolsClient(this IServiceCollection services,
        Action<ChurchToolsOptions, IServiceProvider>? configureOptions = null,
        Action<HttpClient>? configureClient = null)
    {
        ArgumentNullException.ThrowIfNull(services);

        configureOptions ??= (_, _) => { };
        configureClient ??= _ => { };

        services.AddOptions<ChurchToolsOptions>()
            .Configure(configureOptions);

        services.AddKiotaHandlers();

        services.AddHttpClient<ChurchToolsClientFactory>()
            .ConfigureHttpClient(configureClient)
            .AttachKiotaHandlers();

        services.AddTransient(sp => sp.GetRequiredService<ChurchToolsClientFactory>().GetClient());

        return services;
    }

    private static void AddKiotaHandlers(this IServiceCollection services)
    {
        var kiotaHandlers = KiotaClientFactory.GetDefaultHandlerActivatableTypes();

        foreach (var handler in kiotaHandlers)
        {
            services.AddTransient(handler);
        }
    }

    private static void AttachKiotaHandlers(this IHttpClientBuilder builder)
    {
        var kiotaHandlers = KiotaClientFactory.GetDefaultHandlerActivatableTypes();

        foreach (var handler in kiotaHandlers)
        {
            builder.AddHttpMessageHandler((sp) => (DelegatingHandler)sp.GetRequiredService(handler));
        }
    }
}
