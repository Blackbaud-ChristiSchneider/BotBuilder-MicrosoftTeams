// 9/17/19 Copied from https://github.com/microsoft/BotBuilder-V3/blob/master/CSharp/Library/Microsoft.Bot.Connector.NetFramework/BotAuthentication.cs

#pragma warning disable CS1591

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
// using System.Web.Http.Controllers;
// using System.Web.Http.Filters;
using Newtonsoft.Json.Linq;
using Microsoft.Bot.Schema;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Bot.Connector.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Microsoft.Bot.Connector
{
    /// <summary>
    ///
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
    public class BotAuthentication : ActionFilterAttribute, IFilterFactory
    {
        /// <summary>
        /// Microsoft AppId for the bot
        /// </summary>
        /// <remarks>
        /// Needs to be used with MicrosoftAppPassword.  Ignored if CredentialProviderType is specified.
        /// </remarks>
        public string MicrosoftAppId { get; set; }

        /// <summary>
        /// Microsoft AppPassword for the bot (needs to be used with MicrosoftAppId)
        /// </summary>
        /// <remarks>
        /// Needs to be used with MicrosoftAppId. Ignored if CredentialProviderType is specified.
        /// </remarks>
        public string MicrosoftAppPassword { get; set; }

        /// <summary>
        /// Name of Setting in web.config which has the Microsoft AppId for the bot
        /// </summary>
        /// <remarks>
        /// Needs to be used with MicrosoftAppPasswordSettingName. Ignored if CredentialProviderType is specified.
        /// </remarks>
        public string MicrosoftAppIdSettingName { get; set; }

        /// <summary>
        /// Name of Setting in web.config which has the Microsoft App Password for the bot
        /// </summary>
        /// <remarks>
        /// Needs to be used with MicrosoftAppIdSettingName. Ignored if CredentialProviderType is specified.
        /// </remarks>
        public string MicrosoftAppPasswordSettingName { get; set; }

        /// <summary>
        ///
        /// </summary>
        /// <value></value>
        public bool DisableEmulatorTokens { get; set; }

        /// <summary>
        /// Type which implements ICredentialProvider interface to allow multiple bot AppIds to be registered for the same endpoint
        /// </summary>
        public Type CredentialProviderType { get; set; }

        /// <summary>
        ///
        /// </summary>
        /// <value></value>
        public virtual string OpenIdConfigurationUrl { get; set; } = JwtConfig.ToBotFromChannelOpenIdMetadataUrl;

        private readonly IConfiguration _configuration;

        public BotAuthentication(IConfiguration configuration, ICredentialProviderTypeFactory credentialProviderTypeFactory = null)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            CredentialProviderType = credentialProviderTypeFactory?.CredentialProviderType;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="actionContext"></param>
        /// <param name="next"></param>
        /// <returns></returns>
        public override async Task OnActionExecutionAsync(ActionExecutingContext actionContext, ActionExecutionDelegate next)
        {
            var provider = this.GetCredentialProvider();
            var botAuthenticator = new BotAuthenticator(provider, GetOpenIdConfigurationUrl(), DisableEmulatorTokens);
            try
            {
                var identityToken = await botAuthenticator.AuthenticateAsync(actionContext.HttpContext.Request, GetActivities(actionContext), CancellationToken.None);
                // the request is not authenticated, fail with 401.
                if (!identityToken.Authenticated)
                {
                    actionContext.Result = new UnauthorizedResult();
                    // actionContext.Response = BotAuthenticator.GenerateUnauthorizedResponse(actionContext.Request, "BotAuthenticator failed to authenticate incoming request!");
                    return;
                }

            }
            catch // (Exception e)
            {
                actionContext.Result = new UnauthorizedResult();
                // actionContext.Response = BotAuthenticator.GenerateUnauthorizedResponse(actionContext.Request, $"Failed authenticating incoming request: {e.ToString()}");
                return;
            }

            await base.OnActionExecutionAsync(actionContext, next);
            await next();

        }

        private IList<Activity> GetActivities(ActionExecutingContext actionContext)
        {
            var activties = actionContext.ActionArguments.Select(t => t.Value).OfType<Activity>().ToList();
            if (activties.Any())
            {
                return activties;
            }
            else
            {
                var objects =
                    actionContext.ActionArguments.Where(t => t.Value is JObject || t.Value is JArray)
                        .Select(t => t.Value).ToArray();
                if (objects.Any())
                {
                    activties = new List<Activity>();
                    foreach (var obj in objects)
                    {
                        activties.AddRange((obj is JObject) ? new Activity[] { ((JObject)obj).ToObject<Activity>() } : ((JArray)obj).ToObject<Activity[]>());
                    }
                }
            }
            return activties;
        }

        private ICredentialProvider GetCredentialProvider()
        {
            ICredentialProvider credentialProvider = null;
            if (CredentialProviderType != null)
            {
                // if we have a credentialprovider type
                credentialProvider = Activator.CreateInstance(CredentialProviderType) as ICredentialProvider;
                if (credentialProvider == null)
                    throw new ArgumentNullException($"The CredentialProviderType {CredentialProviderType.Name} couldn't be instantiated with no params or doesn't implement ICredentialProvider");
            }
            else if (MicrosoftAppId != null && MicrosoftAppPassword != null)
            {
                // if we have raw values
                credentialProvider = new StaticCredentialProvider(MicrosoftAppId, MicrosoftAppPassword);

            }
            else
            {
                // if we have setting name, or there is no parameters at all default to default setting name
                credentialProvider = new SettingsCredentialProvider(_configuration, MicrosoftAppIdSettingName, MicrosoftAppPasswordSettingName);
            }
            return credentialProvider;
        }

        private string settingsOpenIdConfigurationurl => SettingsUtils.GetAppSettings(_configuration, "BotOpenIdMetadata");

        public bool IsReusable => true;

        private string GetOpenIdConfigurationUrl()
        {
            var settingsUrl = settingsOpenIdConfigurationurl;
            return  string.IsNullOrEmpty(settingsUrl) ? this.OpenIdConfigurationUrl : settingsUrl;
        }

        public IFilterMetadata CreateInstance(IServiceProvider serviceProvider)
        {
            var configuration = serviceProvider.GetService(typeof(IConfiguration)) as IConfiguration;
            var credentialProvider = serviceProvider.GetService(typeof(ICredentialProviderTypeFactory)) as ICredentialProviderTypeFactory;
            return new BotAuthentication(configuration, credentialProvider);
        }
    }
}