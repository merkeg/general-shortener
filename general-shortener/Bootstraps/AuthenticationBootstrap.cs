using System;
using general_shortener.Models.Authentication;
using general_shortener.Services.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;

namespace general_shortener.Bootstraps
{
    /// <summary>
    /// Bootstrap class for authentication and authorization
    /// </summary>
    public static class AuthenticationBootstrap
    {

        /// <summary>
        /// Add Authentication and authorization configuration to the service init
        /// </summary>
        /// <param name="services"></param>
        public static void AddAuthenticationBootstrap(this IServiceCollection services)
        {
            services.AddAuthentication("Bearer")
                .AddScheme<ApiAuthenticationHandlerOptions, ApiAuthenticationHandler>("Bearer", null);
            services.AddSingleton<IApiAuthenticationManager, ApiKeyAuthenticationManager>();

            services.AddAuthorization(config =>
            {
                foreach (var claim in Enum.GetNames<Claim>())
                {
                    config.AddPolicy(claim, builder => builder.RequireClaim(claim));
                }
            });
        }
    }
}