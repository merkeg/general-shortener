using System;
using System.Collections.Generic;
using System.Security.Claims;
using general_shortener.Models.Authentication;
using Microsoft.Extensions.Configuration;
using Claim = general_shortener.Models.Authentication.Claim;

namespace general_shortener.Services.Authentication
{
    /// <inheritdoc />
    public class ApiKeyAuthenticationManager : IApiAuthenticationManager
    {

        /// <summary>
        /// Master token with all permissions
        /// </summary>
        public ApiKeyModel MasterToken { get; set; }
        
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="configuration"></param>
        public ApiKeyAuthenticationManager(IConfiguration configuration)
        {
            string token = configuration.GetValue<string>("MasterToken");
            this.MasterToken = new ApiKeyModel()
            {
                Claims = Enum.GetValues<Claim>(),
                Token = token,
                Description = "Master token"
            };
        }
        
        /// <summary>
        /// Verify a key
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public ApiKeyModel VerifyKey(string token)
        {
            if (this.MasterToken.Token == token)
            {
                return this.MasterToken;
            }
            
            // TODO: Add possibility for other tokens
            return null; 
        }
    }
}