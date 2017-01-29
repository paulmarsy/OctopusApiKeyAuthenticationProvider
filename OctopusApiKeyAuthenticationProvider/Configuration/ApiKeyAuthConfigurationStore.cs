using System.Collections.Generic;
using Octopus.Configuration;
using Octopus.Data.Storage.Configuration;
using Octopus.Server.Extensibility.Extensions.Infrastructure.Configuration;

namespace OctopusApiKeyAuthenticationProvider.Configuration
{
    public class ApiKeyAuthConfigurationStore : IApiKeyAuthConfigurationStore, IHasConfigurationSettings
    {
        public const string SingletonId = "authentication-apikey";

        private readonly IConfigurationStore _configurationStore;
        private readonly IKeyValueStore _settings;

        public ApiKeyAuthConfigurationStore(IConfigurationStore configurationStore, IKeyValueStore settings)
        {
            _configurationStore = configurationStore;
            _settings = settings;
        }

        public bool GetIsEnabled() => _configurationStore.Get<ApiKeyAuthConfiguration>(SingletonId)?.IsEnabled ?? false;

        public void SetIsEnabled(bool isEnabled)
        {
            var doc = _configurationStore.Get<ApiKeyAuthConfiguration>(SingletonId) ?? MoveSettingsToDatabase();
            doc.IsEnabled = isEnabled;
            _configurationStore.Update(doc);
        }

        public bool GetRequireUserIdInRequest() => _configurationStore.Get<ApiKeyAuthConfiguration>(SingletonId)?.RequireUserIdInRequest ?? false;

        public void SetRequireUserIdInRequest(bool requireUserIdInRequest)
        {
            var doc = _configurationStore.Get<ApiKeyAuthConfiguration>(SingletonId) ?? MoveSettingsToDatabase();
            doc.RequireUserIdInRequest = requireUserIdInRequest;
            _configurationStore.Update(doc);
        }

        public bool GetAllowServiceAccountLogin() => _configurationStore.Get<ApiKeyAuthConfiguration>(SingletonId)?.AllowServiceAccountLogin ?? false;

        public void SetAllowServiceAccountLogin(bool allowServiceAccountLogin)
        {
            var doc = _configurationStore.Get<ApiKeyAuthConfiguration>(SingletonId) ?? MoveSettingsToDatabase();
            doc.AllowServiceAccountLogin = allowServiceAccountLogin;
            _configurationStore.Update(doc);
        }

        public bool GetAllowFormsAuthentication() => _configurationStore.Get<ApiKeyAuthConfiguration>(SingletonId)?.AllowFormsAuthentication ?? false;

        public void SetAllowFormsAuthentication(bool allowFormsAuthentication)
        {
            var doc = _configurationStore.Get<ApiKeyAuthConfiguration>(SingletonId) ?? MoveSettingsToDatabase();
            doc.AllowFormsAuthentication = allowFormsAuthentication;
            _configurationStore.Update(doc);
        }

        public string ConfigurationSetName => ApiKeyAuthExtension.Title;

        public IEnumerable<ConfigurationValue> GetConfigurationValues()
        {
            yield return new ConfigurationValue($"Octopus.{ApiKeyAuthExtension.Id}.IsEnabled", GetIsEnabled().ToString(), GetIsEnabled(), "Is Enabled");
            yield return new ConfigurationValue($"Octopus.{ApiKeyAuthExtension.Id}.AllowServiceAccountLogin", GetAllowServiceAccountLogin().ToString(), GetIsEnabled(), "Allow Service Account Login");
            yield return new ConfigurationValue($"Octopus.{ApiKeyAuthExtension.Id}.AllowFormsAuthentication", GetAllowFormsAuthentication().ToString(), GetIsEnabled(), "Allow Forms Authentication");
        }

        private ApiKeyAuthConfiguration MoveSettingsToDatabase()
        {
            var doc = new ApiKeyAuthConfiguration(ApiKeyAuthExtension.Id, "Paul Marston")
            {
                IsEnabled = _settings.Get($"Octopus.{ApiKeyAuthExtension.Id}.IsEnabled", false),
                AllowServiceAccountLogin = _settings.Get($"Octopus.{ApiKeyAuthExtension.Id}.AllowServiceAccountLogin", false),
                AllowFormsAuthentication = _settings.Get($"Octopus.{ApiKeyAuthExtension.Id}.AllowFormsAuthentication", false)
            };

            _configurationStore.Create(doc);

            _settings.Remove($"Octopus.{ApiKeyAuthExtension.Id}.IsEnabled");
            _settings.Remove($"Octopus.{ApiKeyAuthExtension.Id}.AllowServiceAccountLogin");
            _settings.Remove($"Octopus.{ApiKeyAuthExtension.Id}.AllowFormsAuthentication");
            _settings.Save();

            return doc;
        }
    }
}