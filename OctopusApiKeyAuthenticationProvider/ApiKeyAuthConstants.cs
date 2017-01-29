namespace OctopusApiKeyAuthenticationProvider
{
    public static class ApiKeyAuthConstants
    {
        public const string Title = "API Key Authentication";
        public const string SingletonId = "authentication-apikey";
        public const string ExtensionId = "ApiKeyAuth";
        public static readonly string AuthenticateUri = $"/api/users/authenticate/{ExtensionId}/{{apiKey}}";
    }
}
