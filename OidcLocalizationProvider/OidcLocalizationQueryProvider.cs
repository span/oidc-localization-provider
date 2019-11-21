using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.WebUtilities;

namespace Net.DanielKvist.OidcLocalization
{
    /// <summary>
    /// This provider tries to read the ui_locales optional OIDC parameter from
    /// a request. If it succeeds, a cookie is issued setting the requested locale.
    /// </summary>
    /// <remarks>Checks both standard request url and the returnUrl parameter for the ui_locales parameter</remarks>
    public class OidcLocalizationQueryProvider : QueryStringRequestCultureProvider
    {
        private static string ParameterName { get; } = "ui_locales";

        /// <inheritdoc />
        public override Task<ProviderCultureResult> DetermineProviderCultureResult(HttpContext httpContext)
        {
            if (httpContext == null)
            {
                throw new ArgumentNullException(nameof(httpContext));
            }

            var query = httpContext.Request.Query;
            var exists = query.TryGetValue(ParameterName, out var culture);

            if (!exists)
            {
                // The ui_locales parameter may may be set in both the regular query but also part of the
                // return url query for oidc requests
                exists = query.TryGetValue("returnUrl", out var returnUrl);

                if (exists)
                {
                    var cultureFromReturnUrl = GetCultureFromUrl(returnUrl);
                    if(string.IsNullOrEmpty(cultureFromReturnUrl))
                    {
                        return NullProviderCultureResult;
                    }

                    culture = cultureFromReturnUrl;
                }
            }

            var providerResultCulture = ParseDefaultParameterValue(culture);

            // Issue a language cookie for coming requests
            if (!string.IsNullOrEmpty(culture.ToString()))
            {
                var cookie = httpContext.Request.Cookies[".AspNetCore.Culture"];
                var newCookieValue = CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture));

                if (string.IsNullOrEmpty(cookie) || cookie != newCookieValue)
                {
                    httpContext.Response.Cookies.Append(".AspNetCore.Culture", newCookieValue);
                }
            }

            return Task.FromResult(providerResultCulture);
        }

        private static string GetCultureFromUrl(string url)
        {
            var request = url.ToArray()[0];
            var uri = new Uri($"http://neededtoparseuri{request}");
            var query = QueryHelpers.ParseQuery(uri.Query);
            var requestCulture = query.FirstOrDefault(t => t.Key == ParameterName).Value;

            return requestCulture.ToString();
        }

        private static ProviderCultureResult ParseDefaultParameterValue(string value)
        {
            return !string.IsNullOrWhiteSpace(value) ?
                new ProviderCultureResult(value, value) :
                null;
        }
    }
}
