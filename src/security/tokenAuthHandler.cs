using System;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using stock_dotnet.utils.Extensions;
namespace stock_dotnet.security
{

    public class ApiKeyAuthOpts : AuthenticationSchemeOptions
    {
    }

    public class ApiKeyAuthHandler : AuthenticationHandler<ApiKeyAuthOpts>
    {
        public readonly Security _security;
        public ApiKeyAuthHandler(Security security,IOptionsMonitor<ApiKeyAuthOpts> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock)
            : base(options, logger, encoder, clock)
        {
            _security = security;
          
        }
        private const string API_TOKEN_PREFIX = "bearer ";

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            string token = null;
            string authorization = Request.Headers["Authorization"].ToString();

            if (string.IsNullOrEmpty(authorization))
            {
                return AuthenticateResult.NoResult();
            }

            if (authorization.StartsWith(API_TOKEN_PREFIX, StringComparison.OrdinalIgnoreCase))
            {
                token = authorization.Substring(API_TOKEN_PREFIX.Length).Trim();
            }

            if (string.IsNullOrEmpty(token))
            {
                return AuthenticateResult.NoResult();
            }

            User user = await _security.Validate(token);       
            if (user==null)
            {
                return AuthenticateResult.Fail($"token {API_TOKEN_PREFIX} not match");
            }
            else
            {
                var id = new ClaimsIdentity(
                    new Claim[] { 
                        new Claim("token", token) ,
                        new Claim("permissions", user.permissions.Join(",")) ,
                        },  // not safe , just as an example , should custom claims on your own
                    Scheme.Name
                );
                ClaimsPrincipal principal = new ClaimsPrincipal(id);
                var ticket = new AuthenticationTicket(principal, new AuthenticationProperties(), Scheme.Name);
                return AuthenticateResult.Success(ticket);
            }
        }
    }

}