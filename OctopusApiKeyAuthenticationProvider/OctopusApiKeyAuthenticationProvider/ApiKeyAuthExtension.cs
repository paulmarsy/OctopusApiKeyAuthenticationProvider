using Autofac;
using Octopus.Server.Extensibility.Authentication.Extensions;
using Octopus.Server.Extensibility.Extensions;
using Octopus.Server.Extensibility.HostServices.Web;
using Octopus.Server.Extensibility.Extensions.Infrastructure.Configuration;
using OctopusApiKeyAuthenticationProvider.Configuration;
using Nancy;

namespace OctopusApiKeyAuthenticationProvider
{
    [OctopusPlugin(ApiKeyAuthConstants.Title, "Paul Marston")]
    public class ApiKeyAuthExtension : IOctopusExtension
    {
        public void Load(ContainerBuilder builder)
        {
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

            builder.RegisterType<ApiKeyAuthenticationProvider>()
               .As<IAuthenticationProvider>()
               .InstancePerDependency();

            builder.RegisterType<ApiKeyAuthConfiguration>()
                .AsSelf();

            builder.RegisterType<Web.ApiKeyAuthHomeLinks>()
                .As<IHomeLinksContributor>()
                .InstancePerDependency();

            builder.RegisterType<ApiKeyAuthenticationModule>()
                .AsSelf()
                .As<NancyModule>()
                .InstancePerDependency();
        }
    }
}
