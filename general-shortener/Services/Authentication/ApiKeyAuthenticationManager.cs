using System;
using System.Collections.Generic;
using System.Security.Claims;
using general_shortener.Models.Authentication;
using Claim = general_shortener.Models.Authentication.Claim;

namespace general_shortener.Services.Authentication
{
    /// <inheritdoc />
    public class ApiKeyAuthenticationManager : IApiAuthenticationManager
    {
        /// <summary>
        /// Verify a key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public ApiKeyModel VerifyKey(string key)
        {
            if (key == "aaa")
            {
                return new ApiKeyModel()
                {
                    Claims = Enum.GetValues<Claim>(),
                    Key = "aaa"
                };
            }
            return null; 
        }
    }
}