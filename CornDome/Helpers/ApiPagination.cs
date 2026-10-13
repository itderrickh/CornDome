using Microsoft.AspNetCore.WebUtilities;

namespace CornDome.Helpers
{
    public static class ApiPagination
    {
        public const int DefaultLimit = 50;
        public const int MaxLimit = 2000;
        public const int DefaultOffset = 0;

        public static string? GetNextUrl(
            HttpRequest request,
            int total,
            int limit,
            int offset)
        {
            if (offset + limit >= total)
                return null;

            var query = request.Query
                .ToDictionary(
                    item => item.Key,
                    item => item.Value.ToString());

            query["limit"] = limit.ToString();
            query["offset"] = (offset + limit).ToString();

            var baseUrl =
                $"{request.Scheme}://{request.Host}{request.PathBase}{request.Path}";

            return QueryHelpers.AddQueryString(baseUrl, query);
        }
    }
}