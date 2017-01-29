using Octopus.Configuration;
using Octopus.Data.Storage.Configuration;
using Octopus.Server.Extensibility.Extensions.Infrastructure.Configuration;
using System.Collections.Generic;

namespace OctopusApiKeyAuthenticationProvider.Configuration
{
    public class ApiKeyAuthConfigurationStore : IApiKeyAuthConfigurationStore, IHasConfigurationSettings
    {
        public  string ConfigurationSetName => ApiKeyAuthConstants.Title;

        private readonly IConfigurationStore _configurationStore;
        private readonly IKeyValueStore _settings;
        public ApiKeyAuthConfigurationStore(IConfigurationStore configurationStore, IKeyValueStore settings)
        {
            _configurationStore = configurationStore;
            _settings = settings;
        }
        public bool GetIsEnabled()
        {
            var doc = _configurationStore.Get<ApiKeyAuthConfiguration>(ApiKeyAuthConstants.SingletonId);
            return doc?.IsEnabled ?? false;
        }

        public void SetIsEnabled(bool isEnabled)
        {
            var doc = _configurationStore.Get<ApiKeyAuthConfiguration>(ApiKeyAuthConstants.SingletonId) ?? MoveSettingsToDatabase();
            doc.IsEnabled = isEnabled;
            _configurationStore.Update(doc);
        }

        public bool GetIsServiceLoginEnabled()
        {
            var doc = _configurationStore.Get<ApiKeyAuthConfiguration>(ApiKeyAuthConstants.SingletonId);
            return doc?.IsServiceLoginEnabled ?? false;
        }

        public void SetIsServiceLoginEnabled(bool isServiceLoginEnabled)
        {
            var doc = _configurationStore.Get<ApiKeyAuthConfiguration>(ApiKeyAuthConstants.SingletonId) ?? MoveSettingsToDatabase();
            doc.IsServiceLoginEnabled = isServiceLoginEnabled;
            _configurationStore.Update(doc);
        }

        ApiKeyAuthConfiguration MoveSettingsToDatabase()
        {
            var doc = new ApiKeyAuthConfiguration(ApiKeyAuthConstants.ExtensionId, "Paul Marston")
            {
                IsEnabled = _settings.Get($"Octopus.{ApiKeyAuthConstants.ExtensionId}.IsEnabled", false),
                IsServiceLoginEnabled = _settings.Get($"Octopus.{ApiKeyAuthConstants.ExtensionId}.IsServiceLoginEnabled", false)
            };

            _configurationStore.Create(doc);

            _settings.Remove($"Octopus.{ApiKeyAuthConstants.ExtensionId}.IsEnabled");
            _settings.Remove($"Octopus.{ApiKeyAuthConstants.ExtensionId}.IsServiceLoginEnabled");
            _settings.Save();

            return doc;
        }
        public IEnumerable<ConfigurationValue> GetConfigurationValues()
        {
            yield return new ConfigurationValue($"Octopus.{ApiKeyAuthConstants.ExtensionId}.IsEnabled", GetIsEnabled().ToString(), GetIsEnabled(), "Is Enabled");
            yield return new ConfigurationValue($"Octopus.{ApiKeyAuthConstants.ExtensionId}.IsServiceLoginEnabled", GetIsServiceLoginEnabled().ToString(), GetIsServiceLoginEnabled(), "Is Service Login Enabled");

        }
    }
}