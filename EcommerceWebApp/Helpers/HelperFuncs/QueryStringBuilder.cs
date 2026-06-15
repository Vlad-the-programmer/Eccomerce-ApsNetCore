namespace EcommerceWebApp.Helpers.HelperFuncs
{
    public static class QueryStringBuilder
    {
        public static string BuildQueryString(params (string key, string? value)[] parameters)
        {
            var validParams = parameters
                .Where(p => !string.IsNullOrEmpty(p.value))
                .Select(p => $"{Uri.EscapeDataString(p.key)}={Uri.EscapeDataString(p.value!)}");

            return validParams.Any() ? $"?{string.Join("&", validParams)}" : "";
        }
    }
}
