using general_shortener.Models.Authentication;
using Microsoft.AspNetCore.Authorization;

namespace general_shortener.Attributes
{
    /// <inheritdoc />
    public class TypedAuthorize : AuthorizeAttribute
    {
        /// <inheritdoc />
        public TypedAuthorize(Claim claim) : base(claim.ToString())
        {
            
        }
    }
}