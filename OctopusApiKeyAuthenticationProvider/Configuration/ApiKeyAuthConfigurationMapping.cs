using System;
using Octopus.Server.Extensibility.Extensions.Infrastructure.Configuration;

namespace OctopusApiKeyAuthenticationProvider.Configuration
{
    public class ApiKeyAuthConfigurationMapping : IConfigurationDocumentMapper
    {
        public Type GetTypeToMap() => typeof(ApiKeyAuthConfiguration);
    }
}