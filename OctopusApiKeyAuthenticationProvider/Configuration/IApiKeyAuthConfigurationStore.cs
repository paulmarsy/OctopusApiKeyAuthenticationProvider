using Octopus.Server.Extensibility.Extensions.Infrastructure.Configuration;

namespace OctopusApiKeyAuthenticationProvider.Configuration
{
    public interface IApiKeyAuthConfigurationStore : IExtensionConfigurationStore
    {
        bool GetIsServiceLoginEnabled();

        void SetIsServiceLoginEnabled(bool isServiceLoginEnabled);
    }
}
