using System;
using System.Globalization;
using Nancy;
using Nancy.Cookies;
using Nancy.Responses;
using Octopus.Client;
using Octopus.Client.Model;
using Octopus.Data.Storage.User;
using Octopus.Diagnostics;
using Octopus.Server.Extensibility.Authentication.HostServices;
using Octopus.Server.Extensibility.Extensions.Infrastructure.Web.Api;
using Octopus.Time;
using OctopusApiKeyAuthenticationProvider.Configuration;
using Octopus.Server.Extensibility.HostServices.Web;

namespace OctopusApiKeyAuthenticationProvider
{
    public class ApiKeyAuthenticationModule : NancyModule
    {
        private readonly ILog _log;
        private readonly IInvalidLoginTracker _loginTracker;
        private readonly IApiActionResponseCreator _responseCreator;
        private readonly IApiKeyAuthConfigurationStore _configurationStore;
        private readonly IUserStore _userStore;
        private readonly ISleep _sleep;
        private readonly IAuthCookieCreator _authCookieCreator;
        private readonly IWebPortalConfigurationStore _webPortalConfigurationStore;

        public ApiKeyAuthenticationModule(ILog log, IInvalidLoginTracker loginTracker, IAuthCookieCreator authCookieCreator, IApiActionResponseCreator responseCreator, IApiKeyAuthConfigurationStore configurationStore, IUserStore userStore, ISleep sleep, IWebPortalConfigurationStore webPortalConfigurationStore)
        {
            _log = log;
            _loginTracker = loginTracker;
            _authCookieCreator = authCookieCreator;
            _responseCreator = responseCreator;
            _configurationStore = configurationStore;
            _userStore = userStore;
            _sleep = sleep;
            _webPortalConfigurationStore = webPortalConfigurationStore;

            InitializeRoute();
        }

        private void InitializeRoute()
        {
            Get[ApiKeyAuthConstants.AuthenticateUri] = parameters =>
            {
                try
                {
                    if (!_configurationStore.GetIsEnabled())
                        return new RedirectResponse(Request.Url.BasePath ?? "/");

                    string apiKey = parameters.apiKey;
                    if (string.IsNullOrWhiteSpace(apiKey))
                        return new RedirectResponse(Request.Url.BasePath ?? "/");

                    if (!apiKey.StartsWith("API-"))
                        apiKey = $"API-{apiKey}";

                    _log.Info($"API Auth Login with {apiKey}");
                    var endpoint = new OctopusServerEndpoint(_webPortalConfigurationStore.GetListenPrefixes(), apiKey);
                    var repository = new OctopusRepository(endpoint);
                    var currentUser = repository.Client.Get<UserResource>(repository.Client.RootDocument.Links["CurrentUser"]);
                    if (currentUser == null)
                        return FailedLoginAttempt(null, "API key authenticated API call did not return a user");
                    if (!currentUser.IsRequestor)
                        return FailedLoginAttempt(currentUser.Username, "API key user is not requestor");

                    var action = _loginTracker.BeforeAttempt(currentUser.Username, Context.Request.UserHostAddress);
                    if (action == InvalidLoginAction.Ban)
                        return  _responseCreator.BadRequest("You have had too many failed login attempts in a short period of time. Please try again later.");

                    _log.Info($"Found user via API call {currentUser.DisplayName} ({currentUser.Id})");

                    var user = _userStore.GetById(currentUser.Id);
                    _log.Info($"Translated API user {currentUser.Id} to data model user {user.Id}");
                    if (!user.IsActive)
                        return FailedLoginAttempt(user.Username, action, "Account is not active");
                    if (user.IsService && !_configurationStore.GetIsServiceLoginEnabled())
                        return FailedLoginAttempt(user.Username, "Account is a service account and IsServiceLoginEnabled is not enabled");

                    _loginTracker.RecordSucess(user.Username, Context.Request.UserHostAddress);
                    var cookie = _authCookieCreator.CreateAuthCookie(Context, user.IdentificationToken, true);

                    return new RedirectResponse(Request.Url.BasePath ?? "/")
                        .WithCookie(cookie)
                        .WithCookie(new NancyCookie("s", Guid.NewGuid().ToString(), true, false, DateTime.MinValue))
                        .WithCookie(new NancyCookie("n", Guid.NewGuid().ToString(), true, false, DateTime.MinValue))
                        .WithHeader("Expires", DateTime.UtcNow.AddYears(1).ToString("R", DateTimeFormatInfo.InvariantInfo));
                }
                catch (Exception ex)
                {
                    return FailedLoginAttempt(ex.Message);
                }
            };
        }

        private Response FailedLoginAttempt(string username, InvalidLoginAction? action, string reason)
        {
            if (username == null)
            {
                _log.Warn($"API Key login failed: {reason}");
            }
            else
            {
                _log.Warn($"API Key login attempt to account {username} failed: {reason}");
                _loginTracker.RecordFailure(username, Context.Request.UserHostAddress);
            }
            if (action.HasValue && action.Value == InvalidLoginAction.Slow)
                _sleep.For(1000);

            return _responseCreator.Unauthorized(Request);
        }

        private Response FailedLoginAttempt(string username, string reason) => FailedLoginAttempt(username, null, reason);
        private Response FailedLoginAttempt(string reason) => FailedLoginAttempt(null, null, reason);
    }
}
