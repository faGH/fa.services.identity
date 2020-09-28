using Microsoft.Extensions.DependencyInjection;

namespace FrostAura.Services.Identity.Core.Extensions
{
    /// <summary>
    /// Service collection extensions.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Add FrostAura services for the core layer.
        /// </summary>
        /// <param name="services">Application services.</param>
        /// <returns>Services.</returns>
        public static IServiceCollection AddFrostAuraCore(this IServiceCollection services)
        {
            return services;
        }
    }
}
