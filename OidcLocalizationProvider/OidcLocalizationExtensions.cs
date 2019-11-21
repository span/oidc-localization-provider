using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Net.DanielKvist.OidcLocalization
{
    public static class OidcLocalizationExtensions
    {
        public static IServiceCollection AddOidcLocalization(this IServiceCollection services)
        {
            services.Configure<RequestLocalizationOptions>(options =>
            {
                options.RequestCultureProviders.Prepend(new OidcLocalizationQueryProvider());
            });

            return services;
        }
    }
}
