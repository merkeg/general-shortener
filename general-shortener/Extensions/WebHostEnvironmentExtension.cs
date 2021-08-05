using System;
using Microsoft.AspNetCore.Hosting;

namespace general_shortener.Extensions
{
    /// <summary>
    /// Extension class
    /// </summary>
    public static class WebHostEnvironmentExtension
    {

        /// <summary>
        /// Is testing env
        /// </summary>
        /// <param name="env"></param>
        /// <returns></returns>
        public static bool IsTesting(this IWebHostEnvironment env)
        {
            return env.EnvironmentName.Equals("Testing", StringComparison.OrdinalIgnoreCase);
        }
    }
}