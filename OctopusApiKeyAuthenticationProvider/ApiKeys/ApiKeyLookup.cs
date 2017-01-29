using Octopus.Client;
using Octopus.Client.Exceptions;
using Octopus.Client.Model;
using Octopus.Server.Extensibility.HostServices.Web;

namespace OctopusApiKeyAuthenticationProvider.ApiKeys
{
    public class ApiKeyLookup : IApiKeyLookup
    {
        private readonly IWebPortalConfigurationStore _webPortalConfigurationStore;

        public ApiKeyLookup(IWebPortalConfigurationStore webPortalConfigurationStore)
        {
            _webPortalConfigurationStore = webPortalConfigurationStore;
        }
        public bool ValidateApiKey(string expectedUserId, string apikey)
        {
            if (!apikey.StartsWith("API-"))
                apikey = $"API-{apikey}";

            // TODO: Support multiple listen prefixes (split on ; or ,), or provide URI in extension config
            var endpoint = new OctopusServerEndpoint(_webPortalConfigurationStore.GetListenPrefixes(), apikey);
            var repository = new OctopusRepository(endpoint);

            UserResource apiClientUser = null;
            try
            {
                // Calls /api/users/me.. if the API Key is not valid this will throw
                apiClientUser = repository.Client.Get<UserResource>(repository.Client.RootDocument.Links["CurrentUser"]);
            }
            catch (OctopusSecurityException)
            {
                return false;
            }

            if (apiClientUser == null || !apiClientUser.IsRequestor)
                return false;

            if (apiClientUser.Id != expectedUserId)
                return false;

            return true;
        }
    }
}
