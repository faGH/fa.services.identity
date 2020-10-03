using Microsoft.Extensions.DependencyInjection;

namespace FrostAura.Services.Identity.Data.Extensions
{
    /// <summary>
    /// Service collection extensions.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Add FrostAura services for the resources layer.
        /// </summary>
        /// <param name="services">Application services.</param>
        /// <returns>Services.</returns>
        public static IServiceCollection AddFrostAuraResources(this IServiceCollection services)
        {
            return services;
        }
    }
}
