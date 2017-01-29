using Octopus.Server.Extensibility.Extensions.Infrastructure.Configuration;
using System;

namespace OctopusApiKeyAuthenticationProvider.Configuration
{
    public class ApiKeyAuthConfigurationMapping : IConfigurationDocumentMapper
    {
        public Type GetTypeToMap() => typeof(ApiKeyAuthConfiguration);
    }
}
