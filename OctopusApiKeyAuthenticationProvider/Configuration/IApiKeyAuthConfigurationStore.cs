using Octopus.Server.Extensibility.Extensions.Infrastructure.Configuration;

namespace OctopusApiKeyAuthenticationProvider.Configuration
{
    public interface IApiKeyAuthConfigurationStore : IExtensionConfigurationStore
    {
        bool GetAllowServiceAccountLogin();

        void SetAllowServiceAccountLogin(bool allowServiceAccountLogin);

        bool GetAllowFormsAuthentication();

        void SetAllowFormsAuthentication(bool allowFormsAuthentication);
    }
}