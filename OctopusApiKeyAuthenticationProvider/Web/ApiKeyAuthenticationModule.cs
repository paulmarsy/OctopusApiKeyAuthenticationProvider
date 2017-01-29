using Nancy;
using Nancy.Responses;
using Octopus.Data.Storage.User;
using Octopus.Diagnostics;
using Octopus.Server.Extensibility.Authentication.HostServices;
using Octopus.Server.Extensibility.Extensions.Infrastructure.Web.Api;
using Octopus.Time;
using OctopusApiKeyAuthenticationProvider.Configuration;
using OctopusApiKeyAuthenticationProvider.ApiKeys;
using Octopus.Server.Extensibility.HostServices.Web;

namespace OctopusApiKeyAuthenticationProvider
{
    public class ApiKeyAuthenticationModule : NancyModule
    {
        private readonly IAuthCookieCreator _authCookieCreator;
        private readonly IApiKeyAuthConfigurationStore _configurationStore;
        private readonly ILog _log;
        private readonly IInvalidLoginTracker _loginTracker;
        private readonly IApiActionResponseCreator _responseCreator;
        private readonly ISleep _sleep;
        private readonly IUserStore _userStore;
        private readonly IApiKeyLookup _apiKeyLookup;
        private readonly IWebPortalConfigurationStore _webPortalConfigurationStore;

        public ApiKeyAuthenticationModule(ILog log, IInvalidLoginTracker loginTracker, IAuthCookieCreator authCookieCreator, IApiActionResponseCreator responseCreator, IApiKeyAuthConfigurationStore configurationStore, IUserStore userStore, ISleep sleep, IApiKeyLookup apiKeyLookup, IWebPortalConfigurationStore webPortalConfigurationStore)
        {
            _log = log;
            _loginTracker = loginTracker;
            _authCookieCreator = authCookieCreator;
            _responseCreator = responseCreator;
            _configurationStore = configurationStore;
            _userStore = userStore;
            _sleep = sleep;
            _apiKeyLookup = apiKeyLookup;
            _webPortalConfigurationStore = webPortalConfigurationStore;

            InitializeRoute();
        }

        private void InitializeRoute()
        {
            if (!_configurationStore.GetIsEnabled())
                return;

            Get[ApiKeyAuthExtension.AuthenticateUri] = parameters =>
            {
                string id = parameters.id;
                string apiKey = parameters.apikey;
                if (string.IsNullOrWhiteSpace(id) || string.IsNullOrWhiteSpace(apiKey))
                    return new NotFoundResponse();

                var user = _userStore.GetById(id);
                if (user == null)
                    return new NotFoundResponse();

                var action = _loginTracker.BeforeAttempt(user.Username, Context.Request.UserHostAddress);
                if (action == InvalidLoginAction.Ban)
                    return _responseCreator.BadRequest("You have had too many failed login attempts in a short period of time. Please try again later.");

                if (!user.IsActive || (user.IsService && !_configurationStore.GetAllowServiceAccountLogin()))
                    return FailLoginAttempt(user.Username, action);

                if (!_apiKeyLookup.ValidateApiKey(user.Id, apiKey))
                    return FailLoginAttempt(user.Username, action);

                _loginTracker.RecordSucess(user.Username, Context.Request.UserHostAddress);

                return new RedirectResponse(GetRedirectLocation())
                    .WithCookie(_authCookieCreator.CreateAuthCookie(Context, user.IdentificationToken, true));
            };
        }

        private Response FailLoginAttempt(string username, InvalidLoginAction action)
        {
            _loginTracker.RecordFailure(username, Context.Request.UserHostAddress);
            if (action == InvalidLoginAction.Slow)
                _sleep.For(1000);

            return _responseCreator.Unauthorized(Request);
        }

        private string GetRedirectLocation()
        {
            var whitelist = _webPortalConfigurationStore.GetTrustedRedirectUrls();
            if (Request.Query["redirectTo"].HasValue && Requests.IsLocalUrl(Request.Query["redirectTo"].Value, whitelist))
                return Request.Query["redirectTo"].Value;

            if (Request.Query["redirectTo"].HasValue)
                _log.WarnFormat("Prevented potential Open Redirection attack on API Key Auth request, to the non-local url {0}", Request.Query["redirectTo"].Value);

            return Request.Url.BasePath ?? "/";
        }
    }
}