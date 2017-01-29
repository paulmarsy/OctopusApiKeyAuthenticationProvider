namespace OctopusApiKeyAuthenticationProvider.ApiKeys
{
    public interface IApiKeyLookup
    {
        bool ValidateApiKey(string expectedUserId, string apikey);
    }
}
