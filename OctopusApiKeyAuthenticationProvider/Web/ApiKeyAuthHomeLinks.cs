using Octopus.Server.Extensibility.HostServices.Web;
using System.Collections.Generic;

namespace OctopusApiKeyAuthenticationProvider.Web
{
    public class ApiKeyAuthHomeLinks : IHomeLinksContributor
    {
        public IDictionary<string, string> GetLinksToContribute()
        {
            return new Dictionary<string, string>
           {
               {ApiKeyAuthConstants.ExtensionId, ApiKeyAuthConstants.AuthenticateUri }
           };
        }
    }
}
