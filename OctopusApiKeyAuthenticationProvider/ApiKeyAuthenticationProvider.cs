using Octopus.Server.Extensibility.Authentication.Extensions;
using Octopus.Server.Extensibility.Authentication.Resources;
using OctopusApiKeyAuthenticationProvider.Configuration;

namespace OctopusApiKeyAuthenticationProvider
{
public class ApiKeyAuthenticationProvider : IAuthenticationProvider
    {
        private readonly IApiKeyAuthConfigurationStore _configurationStore;
        public ApiKeyAuthenticationProvider(IApiKeyAuthConfigurationStore configurationStore)
        {
            _configurationStore = configurationStore;
        }

        public AuthenticationProviderElement GetAuthenticationProviderElement()
        {
            return new AuthenticationProviderElement
            {
                FormsLoginEnabled = false,
                IsGuestProvider = false,
                Name = IdentityProviderName
            };

        }
        public string[] GetAuthenticationUrls() => new string[0];
        public string IdentityProviderName => ApiKeyAuthConstants.Title;
        public bool IsEnabled => _configurationStore.GetIsEnabled();
        public bool SupportsPasswordManagement => false;
    }
}
