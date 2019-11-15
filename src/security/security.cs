
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using stock_dotnet.utils.env;
using stock_dotnet.utils.Exceptions;

namespace stock_dotnet.security
{

    public class Security
    {
        private IMemoryCache _cache;
        private readonly IHttpClientFactory _clientFactory;
     
        private readonly IEnv _env ;

        public Security(IMemoryCache cache, IHttpClientFactory clientFactory, IEnv env)
        {
            _cache = cache;
            _clientFactory = clientFactory;
            _env = env;
        }

        public async Task<User> Validate(string token)
        {
            User user;
            token = token?.Replace("bearer ", "");
            if(token=="") throw new AuthorizationException("Unauthorized");
            if (!_cache.TryGetValue(key : token, out user))
            {
                user =await GetRemote(token);
                if(user==null) throw new AuthorizationException("Unauthorized");
                _cache.Set(token, user, new MemoryCacheEntryOptions() // Keep in cache for this time, reset time if accessed.
                     .SetSlidingExpiration(TimeSpan.FromMinutes(10)));
            }


            return user;
        }

        private async Task<User> GetRemote(string token)
        {
            var request = new HttpRequestMessage(HttpMethod.Get,
            _env.SecurityServerURL + "/v1/users/current");
            request.Headers.Add("Authorization", "bearer " + token);
            var client = _clientFactory.CreateClient();

            var response = await client.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                return  await response.Content
                    .ReadAsAsync<User>();
            }
            throw new AuthorizationException("Unauthorized");               
        }
    }
}