using Octopus.Server.Extensibility.Extensions.Infrastructure.Configuration;

namespace OctopusApiKeyAuthenticationProvider.Configuration
{
    public class ApiKeyAuthConfiguration : ExtensionConfigurationDocument
    {
        protected ApiKeyAuthConfiguration()
        {
        }

        public ApiKeyAuthConfiguration(string name, string extensionAuthor) : base(name, extensionAuthor)
        {
            Id = ApiKeyAuthConfigurationStore.SingletonId;
        }

        public bool IsEnabled { get; set; }
        public bool RequireUserIdInRequest { get; set; }
        public bool AllowServiceAccountLogin { get; set; }
        public bool AllowFormsAuthentication { get; set; }
    }
}