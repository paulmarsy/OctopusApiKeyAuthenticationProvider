using System;
using System.Collections.Generic;
using Octopus.Diagnostics;
using Octopus.Server.Extensibility.Extensions.Infrastructure.Configuration;
using Octopus.Server.Extensibility.HostServices.Web;

namespace OctopusApiKeyAuthenticationProvider.Configuration
{
    public class ApiKeyAuthConfigureCommands : IContributeToConfigureCommand
    {
        private readonly Lazy<IApiKeyAuthConfigurationStore> _configurationStore;
        private readonly ILog _log;
        private readonly IWebPortalConfigurationStore _webPortalConfigurationStore;

        public ApiKeyAuthConfigureCommands(ILog log, Lazy<IApiKeyAuthConfigurationStore> configurationStore, IWebPortalConfigurationStore webPortalConfigurationStore)
        {
            _log = log;
            _configurationStore = configurationStore;
            _webPortalConfigurationStore = webPortalConfigurationStore;
        }

        public IEnumerable<ConfigureCommandOption> GetOptions()
        {
            yield return new ConfigureCommandOption("apiKeyAuthEnabled=", "If the API Key Auth extension is enabled", v =>
            {
                var isEnabled = bool.Parse(v);
                _configurationStore.Value.SetIsEnabled(isEnabled);
                _log.Info($"{ApiKeyAuthExtension.Id} IsEnabled set to: {isEnabled}");

                if (isEnabled && _webPortalConfigurationStore.GetForceSSL() == false && _webPortalConfigurationStore.GetListenPrefixes().ToLower().Contains("http://"))
                    _log.Warn($"API Key user authentication extension was enabled on an instance including listening prefixes that are not using https.");
            });
            yield return new ConfigureCommandOption("apiKeyAuthRequireUserIdInRequest=", "If the user id (user name for forms) is required to successfully authenticate.", v =>
            {
                var requireUserIdInRequest = bool.Parse(v);
                _configurationStore.Value.SetRequireUserIdInRequest(requireUserIdInRequest);
                _log.Info($"{ApiKeyAuthExtension.Id} RequireUserIdInRequest set to: {requireUserIdInRequest}");
            });
            yield return new ConfigureCommandOption("apiKeyAuthAllowServiceAccountLogin=", "If service accounts can authenticate using the API Key Auth extension.", v =>
            {
                var allowServiceAccountLogin = bool.Parse(v);
                _configurationStore.Value.SetAllowServiceAccountLogin(allowServiceAccountLogin);
                _log.Info($"{ApiKeyAuthExtension.Id} AllowServiceAccountLogin set to: {allowServiceAccountLogin}");
            });
            yield return new ConfigureCommandOption("apiKeyAuthAllowFormsAuthentication=", "If service accounts can authenticate using the API Key Auth extension.", v =>
            {
                var allowFormsAuthentication = bool.Parse(v);
                _configurationStore.Value.SetAllowFormsAuthentication(allowFormsAuthentication);
                _log.Info($"{ApiKeyAuthExtension.Id} AllowFormsAuthentication set to: {allowFormsAuthentication}");
            });
        }
    }
}