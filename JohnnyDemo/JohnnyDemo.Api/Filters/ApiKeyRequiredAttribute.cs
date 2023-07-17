using JohnnyDemo.Api.Options;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;

namespace JohnnyDemo.Api.Filters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class ApiKeyRequiredAttribute : Attribute, IAuthorizationFilter
    {
        private const string API_KEY_HEADER_NAME = "X-API-Key";

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var submittedApiKey = GetSubmittedApiKey(context.HttpContext);

            var apiKey = GetApiKeyList(context.HttpContext);

            if (!IsApiKeyValid(apiKey, submittedApiKey))
            {
                context.Result = new UnauthorizedResult();
            }
        }

        private static string? GetSubmittedApiKey(HttpContext context)
        {
            return context.Request.Headers[API_KEY_HEADER_NAME];
        }

        private static HashSet<string> GetApiKeyList(HttpContext context)
        {
            var authOptions = context.RequestServices.GetRequiredService<IOptions<AuthOptions>>();

            return authOptions.Value.ApiKeys;
        }

        private static bool IsApiKeyValid(HashSet<string> apiKeyList, string? submittedApiKey)
        {
            if (string.IsNullOrEmpty(submittedApiKey)) return false;

            if (apiKeyList.Contains(submittedApiKey.ToLower())) return true;

            return false;
        }
    }
}
