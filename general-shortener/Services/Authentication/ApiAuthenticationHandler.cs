using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Security.Principal;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using general_shortener.Models.Authentication;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Claim = System.Security.Claims.Claim;

namespace general_shortener.Services.Authentication
{

    /// <summary>
    /// Api authentication handler options
    /// </summary>
    public class ApiAuthenticationHandlerOptions : AuthenticationSchemeOptions
    {
        
    }
    
    /// <summary>
    /// Api authentication handler
    /// </summary>
    public class ApiAuthenticationHandler : AuthenticationHandler<ApiAuthenticationHandlerOptions>
    {
        private IApiAuthenticationManager _apiKeyAuthenticationManager;
        
        /// <inheritdoc />
        public ApiAuthenticationHandler(IOptionsMonitor<ApiAuthenticationHandlerOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock, IApiAuthenticationManager apiKeyAuthenticationManager) : base(options, logger, encoder, clock)
        {
            this._apiKeyAuthenticationManager = apiKeyAuthenticationManager;
        }

        /// <inheritdoc />
        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Request.Headers.ContainsKey("Authorization"))
                return AuthenticateResult.Fail("Unauthorized");

            string authorizationHeader = Request.Headers["Authorization"];
            
            if(string.IsNullOrEmpty(authorizationHeader) || string.IsNullOrWhiteSpace(authorizationHeader))
                return AuthenticateResult.Fail("Unauthorized");
            
            if(!authorizationHeader.StartsWith("bearer", StringComparison.OrdinalIgnoreCase))
                return AuthenticateResult.Fail("Unauthorized");

            authorizationHeader = authorizationHeader.Substring("bearer".Length).Trim();
            
            if(string.IsNullOrEmpty(authorizationHeader) || string.IsNullOrWhiteSpace(authorizationHeader))
                return AuthenticateResult.Fail("Unauthorized");

            try
            {
                return this.ValidateToken(authorizationHeader);
            }
            catch (Exception e)
            {
                return AuthenticateResult.Fail("Unauthorized");
            }
        }

        /// <summary>
        /// Validates an api token
        /// </summary>
        /// <param name="key">Token to validate</param>
        /// <returns>Result of validation</returns>
        private AuthenticateResult ValidateToken(string key)
        {
            ApiKeyModel keyInformation = this._apiKeyAuthenticationManager.VerifyKey(key);

            if (keyInformation == null)
                return AuthenticateResult.Fail("Unauthorized");

            List<Claim> claims = new List<Claim>();

            foreach (var claim in keyInformation.Claims)
            {
                claims.Add(new Claim(claim.ToString(), "true"));
            }
            
            ClaimsIdentity identity = new ClaimsIdentity(claims, Scheme.Name);
            GenericPrincipal principal = new GenericPrincipal(identity, null);
            AuthenticationTicket ticket = new AuthenticationTicket(principal, Scheme.Name);
            return AuthenticateResult.Success(ticket);
        }
    }
}