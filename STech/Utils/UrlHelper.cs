using Microsoft.AspNetCore.Http.Extensions;
using System.Collections.Specialized;
using System.Web;

namespace STech.Utils
{
    public class UrlHelper
    {
        public static string AddOrUpdateQueryParam(string currentUrl, string paramName, string paramValue)
        {
            UriBuilder uriBuilder = new UriBuilder(currentUrl);
            NameValueCollection queryParams = HttpUtility.ParseQueryString(uriBuilder.Query);

            queryParams.Set(paramName, paramValue);
            uriBuilder.Query = queryParams.ToString();

            return uriBuilder.ToString();
        }

        public static string AddOrUpdateQueryParams(string currentUrl, NameValueCollection updatedParams)
        {
            UriBuilder uriBuilder = new UriBuilder(currentUrl);
            NameValueCollection queryParams = HttpUtility.ParseQueryString(uriBuilder.Query);

            foreach (string key in updatedParams)
            {
                queryParams.Set(key, updatedParams[key]);
            }

            uriBuilder.Query = queryParams.ToString();

            return uriBuilder.ToString();
        }
    }
}
