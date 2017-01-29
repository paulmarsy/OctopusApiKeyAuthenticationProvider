using Autofac;
using Nancy;
using Octopus.Server.Extensibility.Authentication.Extensions;
using Octopus.Server.Extensibility.Extensions;
using Octopus.Server.Extensibility.Extensions.Infrastructure.Configuration;
using Octopus.Server.Extensibility.HostServices.Web;
using OctopusApiKeyAuthenticationProvider.ApiKeys;
using OctopusApiKeyAuthenticationProvider.Configuration;
using OctopusApiKeyAuthenticationProvider.Web;

namespace OctopusApiKeyAuthenticationProvider
{
    [OctopusPlugin(Title, "Paul Marston")]
    public class ApiKeyAuthExtension : IOctopusExtension
    {
        public const string Title = "API Key Authentication";
        public const string Id = "ApiKeyAuth";
        public static readonly string AuthenticateUri = "/api/users/{id}/authenticate/apikey/{apikey}";

        public void Load(ContainerBuilder builder)
        {
            // ApiKeys
            builder.RegisterType<ApiKeyLookup>()
               .As<IApiKeyLookup>()
               .InstancePerDependency();

            // Configuration
            builder.RegisterType<ApiKeyAuthConfigurationMapping>()
                .As<IConfigurationDocumentMapper>()
                .InstancePerDependency();

            builder.RegisterType<ApiKeyAuthConfigurationStore>()
                .As<IApiKeyAuthConfigurationStore>()
                .As<IHasConfigurationSettings>()
                .InstancePerDependency();

            builder.RegisterType<ApiKeyAuthConfigureCommands>()
                .As<IContributeToConfigureCommand>()
                .InstancePerDependency();

            // Web
            builder.RegisterType<ApiKeyAuthenticationModule>()
                .As<NancyModule>()
                .InstancePerDependency();

            builder.RegisterType<ApiKeyAuthHomeLinks>()
                .As<IHomeLinksContributor>()
                .InstancePerDependency();

            builder.RegisterType<ApiKeyCredentialValidator>()
                .As<IDoesBasicAuthentication>()
                .InstancePerDependency();

            // Extension
            builder.RegisterType<ApiKeyAuthenticationProvider>()
                .As<IAuthenticationProvider>()
                .InstancePerDependency();
        }
    }
}