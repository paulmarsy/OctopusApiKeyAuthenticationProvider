using System.Collections.Generic;
using Octopus.Server.Extensibility.HostServices.Web;
using OctopusApiKeyAuthenticationProvider.Configuration;

namespace OctopusApiKeyAuthenticationProvider.Web
{
    public class ApiKeyAuthHomeLinks : IHomeLinksContributor
    {
        private readonly IApiKeyAuthConfigurationStore _configurationStore;

        public ApiKeyAuthHomeLinks(IApiKeyAuthConfigurationStore configurationStore)
        {
            _configurationStore = configurationStore;
        }

        public IDictionary<string, string> GetLinksToContribute()
        {
            var linksToContribute = new Dictionary<string, string>();

            if (_configurationStore.GetIsEnabled())
                if (_configurationStore.GetRequireUserIdInRequest())
                    linksToContribute.Add(ApiKeyAuthExtension.Id, ApiKeyAuthExtension.AuthenticateUri + ApiKeyAuthExtension.UserIdQueryString);
                else
                    linksToContribute.Add(ApiKeyAuthExtension.Id, ApiKeyAuthExtension.AuthenticateUri);

            return linksToContribute;
        }
    }
}