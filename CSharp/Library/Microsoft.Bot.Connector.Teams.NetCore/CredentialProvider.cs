// 9/17/19 Copied from https://github.com/microsoft/BotBuilder-V3/blob/master/CSharp/Library/Microsoft.Bot.Connector.Shared/CredentialProvider.cs

#pragma warning disable CS1591

#if !NET45
using Microsoft.Bot.Connector.Authentication;
using Microsoft.Extensions.Configuration;
#endif
using System;
using System.Threading.Tasks;

#if NET45
using System.Configuration;
#endif

namespace Microsoft.Bot.Connector
{
    public interface ICredentialProvider
    {
        /// <summary>
        /// Validate AppId
        /// </summary>
        /// <param name="appId"></param>
        /// <returns>true if it is a valid AppId for the controller</returns>
        Task<bool> IsValidAppIdAsync(string appId);

        /// <summary>
        /// Get the app password for a given bot appId, if it is not a valid appId, return Null
        /// </summary>
        /// <param name="appId">bot appid</param>
        /// <returns>password or null for invalid appid</returns>
        Task<string> GetAppPasswordAsync(string appId);

        /// <summary>
        /// Checks if bot authentication is disabled.
        /// </summary>
        /// <returns>true if bot authentication is disabled.</returns>
        Task<bool> IsAuthenticationDisabledAsync();
    }

    public interface ICredentialProviderTypeFactory
    {
        Type CredentialProviderType { get; }
    }

#if DEBUG
    public class NoCredentialProvider : ICredentialProvider
    {
        public Task<bool> IsValidAppIdAsync(string appId)
        {
            return Task.FromResult(true);
        }

        public Task<string> GetAppPasswordAsync(string appId)
        {
            return Task.FromResult("password");
        }

        public Task<bool> IsAuthenticationDisabledAsync()
        {
            return Task.FromResult(false);
        }
    }
#endif

    public class SimpleCredentialProvider : ICredentialProvider
    {
        public string AppId { get; set; }

        public string Password { get; set; }

        public Task<bool> IsValidAppIdAsync(string appId)
        {
            return Task.FromResult(appId == AppId);
        }

        public Task<string> GetAppPasswordAsync(string appId)
        {
            return Task.FromResult((appId == this.AppId) ? this.Password : null);
        }

        public Task<bool> IsAuthenticationDisabledAsync()
        {
            return Task.FromResult(string.IsNullOrEmpty(AppId));
        }
    }

    /// <summary>
    /// Static credential provider which has the appid and password static
    /// </summary>
    public sealed class StaticCredentialProvider : SimpleCredentialProvider
    {
        public StaticCredentialProvider(string appId, string password)
        {
            this.AppId = appId;
            this.Password = password;
        }
    }

    /// <summary>
    /// Credential provider which uses config settings to lookup appId and password
    /// </summary>
    public sealed class SettingsCredentialProvider : SimpleCredentialProvider
    {
        public SettingsCredentialProvider(IConfiguration config, string appIdSettingName = null, string appPasswordSettingName = null)
        {
            var appIdKey = appIdSettingName ?? MicrosoftAppCredentials.MicrosoftAppIdKey;
            var passwordKey = appPasswordSettingName ?? MicrosoftAppCredentials.MicrosoftAppPasswordKey;
            this.AppId = SettingsUtils.GetAppSettings(config, appIdKey);
            this.Password = SettingsUtils.GetAppSettings(config, passwordKey);
        }
    }

    public static class SettingsUtils
    {
        public static string GetAppSettings(IConfiguration config, string key)
        {
            return config.GetValue<string>(key) ?? Environment.GetEnvironmentVariable(key, EnvironmentVariableTarget.Process);
        }
    }

    /// <summary>
    /// Credential provider which uses <see cref="Microsoft.Extensions.Configuration.IConfiguration"/> to lookup appId and password
    /// </summary>
    public sealed class ConfigurationCredentialProvider : SimpleCredentialProvider
    {
        public ConfigurationCredentialProvider(IConfiguration configuration,
            string appIdSettingName = null,
            string appPasswordSettingName = null)
        {
            var appIdKey = appIdSettingName ?? MicrosoftAppCredentials.MicrosoftAppIdKey;
            var passwordKey = appPasswordSettingName ?? MicrosoftAppCredentials.MicrosoftAppPasswordKey;
            this.AppId = configuration.GetSection(appIdKey)?.Value;
            this.Password = configuration.GetSection(passwordKey)?.Value;
        }
    }
}