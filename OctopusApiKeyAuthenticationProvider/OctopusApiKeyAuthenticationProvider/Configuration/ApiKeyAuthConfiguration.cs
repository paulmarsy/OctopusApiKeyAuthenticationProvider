using Octopus.Server.Extensibility.Extensions.Infrastructure.Configuration;

namespace OctopusApiKeyAuthenticationProvider.Configuration
{
    public  class ApiKeyAuthConfiguration : ExtensionConfigurationDocument
    {
        protected ApiKeyAuthConfiguration()
        {
        }

        public ApiKeyAuthConfiguration(string name, string extensionAuthor) : base(name, extensionAuthor)
        {
            Id = ApiKeyAuthConstants.SingletonId;
        }
        public bool IsEnabled { get; set; }
        public bool IsServiceLoginEnabled { get; set; }
    }
}
