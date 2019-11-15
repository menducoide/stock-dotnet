using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using stock_dotnet.utils.Exceptions;

namespace stock_dotnet.security
{
    public class AuthenticationHandler : AuthorizationHandler<Role>
    {
        public readonly Security _security;
        public const string JWT_KEY_HEADER_NAME = "Authorization";
        public AuthenticationHandler(Security security, IHttpContextAccessor httpContextAccessor)
        {
            _security = security;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, Role role)
        {
            await SucceedRequirement(context, role);
            return;
            // throw new AuthorizationException("Unauthorized");
        }


        private async Task SucceedRequirement(AuthorizationHandlerContext context, Role role)
        {
            bool succeed = false;
            if (context.Resource is AuthorizationFilterContext authorizationFilterContext)
            {
                string token = authorizationFilterContext.HttpContext.Request.Headers[JWT_KEY_HEADER_NAME].FirstOrDefault();
                if (!string.IsNullOrEmpty(token))
                {
                    User user = await _security.Validate(token);
                    if (!string.IsNullOrEmpty(role.Name))
                    {
                        string[] roles = role.Name.Split(",");
                        foreach (string permision in roles)
                        {
                            if (user.name.Contains(permision))
                            {
                                context.Succeed(role);
                                succeed = true;
                            }
                        }
                    }
                    else
                    {
                        context.Succeed(role);
                        succeed = true;
                    }

                }
            }
            if (!succeed) throw new AuthorizationException("Unauthorized");
        }
    }
    public class Role : IAuthorizationRequirement
    {
        public string Name { get; set; }

        public Role(string name)
        {
            Name = name;
        }
    }


}