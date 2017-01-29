using Octopus.Server.Extensibility.Authentication.Extensions;
using Octopus.Server.Extensibility.Authentication.Storage.User;
using OctopusApiKeyAuthenticationProvider.Configuration;
using OctopusApiKeyAuthenticationProvider.ApiKeys;
using Octopus.Data.Storage.User;

namespace OctopusApiKeyAuthenticationProvider.Web
{
    public class ApiKeyCredentialValidator : IDoesBasicAuthentication
    {
        private readonly IApiKeyAuthConfigurationStore _configurationStore;
        private readonly IApiKeyLookup _apiKeyLookup;
        private readonly IUserStore _userStore;

        public ApiKeyCredentialValidator(IApiKeyAuthConfigurationStore configurationStore, IApiKeyLookup apiKeyLookup, IUserStore userStore)
        {
            _configurationStore = configurationStore;
            _apiKeyLookup = apiKeyLookup;
            _userStore = userStore;
        }

        public int Priority => 75;

        public AuthenticationUserCreateOrUpdateResult ValidateCredentials(string username, string password)
        {
            if (!_configurationStore.GetIsEnabled() || !_configurationStore.GetAllowFormsAuthentication())
                return new AuthenticationUserCreateOrUpdateResult();

            var user = _userStore.GetByUsername(username);
            if (user != null && _apiKeyLookup.ValidateApiKey(user.Id, password))
                return new AuthenticationUserCreateOrUpdateResult(user);

            return new AuthenticationUserCreateOrUpdateResult();
        }
    }
}
