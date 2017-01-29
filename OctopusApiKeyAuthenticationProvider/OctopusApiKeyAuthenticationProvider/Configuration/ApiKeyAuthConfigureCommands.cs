using Octopus.Server.Extensibility.Extensions.Infrastructure.Configuration;
using Octopus.Diagnostics;
using System;
using System.Collections.Generic;

namespace OctopusApiKeyAuthenticationProvider.Configuration
{
    public class ApiKeyAuthConfigureCommands : IContributeToConfigureCommand
    {
        private readonly ILog _log;
        private readonly Lazy<IApiKeyAuthConfigurationStore> _configurationStore;
        public ApiKeyAuthConfigureCommands(ILog log, Lazy<IApiKeyAuthConfigurationStore> configurationStore)
        {
            _log = log;
            _configurationStore = configurationStore;
        }
        public IEnumerable<ConfigureCommandOption> GetOptions()
        {
            yield return new ConfigureCommandOption("apiKeyAuthEnabled=", "If the API Key Auth extension is enabled", v =>
            {
                var isEnabled = bool.Parse(v);
                _configurationStore.Value.SetIsEnabled(isEnabled);
                _log.Info($"ApiKeyAuthIsEnabled IsEnabled set to: {isEnabled}");
            });
            yield return new ConfigureCommandOption("apiKeyAuthServiceLoginEnabled=", "If service accounts can authenticate using the API Key Auth extension.", v =>
            {
                var isServiceLoginEnabled = bool.Parse(v);
                _configurationStore.Value.SetIsServiceLoginEnabled(isServiceLoginEnabled);
                _log.Info($"ApiKeyAuthIsEnabled IsServiceLoginEnabled set to: {isServiceLoginEnabled}");
            });
        }
    }
}
