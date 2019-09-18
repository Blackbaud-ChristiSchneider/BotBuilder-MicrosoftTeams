// 9/17/19 Copied from https://github.com/microsoft/BotBuilder-V3/blob/3f2cedafa1cda718cd116292a5e5ce67b1cceea0/CSharp/Library/Microsoft.Bot.Connector.Shared/JwtConfig.cs

namespace Microsoft.Bot.Connector
{
    using System;
    using Microsoft.IdentityModel.Tokens;

    /// <summary>
    /// Configuration for JWT tokens
    /// </summary>
    public static class JwtConfig
    {
        /// <summary>
        /// TO CHANNEL FROM BOT: Login URL template string. Bot developer may specify
        /// which tenant to obtain an access token from. By default, the channels only
        /// accept tokens from "botframework.com". For more details see https://aka.ms/bots/tenant-restriction
        /// </summary>
        public const string ToChannelFromBotLoginUrlTemplate = "https://login.microsoftonline.com/{0}/oauth2/v2.0/token";

        /// <summary>
        /// Bot framework authority for Microsoft Application authentication
        /// </summary>
        public const string ConvergedAppAuthority = "https://login.microsoftonline.com/{0}";

        /// <summary>
        /// TO CHANNEL FROM BOT: OAuth scope to request
        /// </summary>
        public const string ToChannelFromBotOAuthScope = "https://api.botframework.com";

        /// <summary>
        /// TO BOT FROM CHANNEL: OpenID metadata document for tokens coming from MSA
        /// </summary>
        public const string ToBotFromChannelOpenIdMetadataUrl = "https://login.botframework.com/v1/.well-known/openidconfiguration";

        /// <summary>
        /// TO BOT FROM CHANNEL: Token validation parameters when connecting to a bot
        /// </summary>
        public static readonly TokenValidationParameters ToBotFromChannelTokenValidationParameters =
            new TokenValidationParameters()
            {
                ValidateIssuer = true,
                ValidIssuers = new[] { "https://api.botframework.com" },
                // Audience validation takes place in JwtTokenExtractor
                ValidateAudience = false,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.FromMinutes(5),
                RequireSignedTokens = true
            };

        /// <summary>
        /// TO BOT FROM CHANNEL: Allowed token signing algorithms
        /// </summary>
        public static readonly string[] ToBotFromChannelAllowedSigningAlgorithms = new[] { "RS256", "RS384", "RS512" };

        /// <summary>
        /// TO BOT FROM EMULATOR: OpenID metadata document for tokens coming from MSA
        /// </summary>
        public const string ToBotFromEmulatorOpenIdMetadataUrl = "https://login.microsoftonline.com/common/v2.0/.well-known/openid-configuration";

        /// <summary>
        /// TO BOT FROM EMULATOR: Token validation parameters when connecting to a channel
        /// </summary>
        public static readonly TokenValidationParameters ToBotFromEmulatorTokenValidationParameters =
            new TokenValidationParameters()
            {
                ValidateIssuer = true,
                ValidIssuers = new[] {
                    "https://sts.windows.net/d6d49420-f39b-4df7-a1dc-d59a935871db/",                    // Auth v3.1, 1.0 token
                    "https://login.microsoftonline.com/d6d49420-f39b-4df7-a1dc-d59a935871db/v2.0",      // Auth v3.1, 2.0 token
                    "https://sts.windows.net/f8cdef31-a31e-4b4a-93e4-5f571e91255a/",                    // Auth v3.2, 1.0 token
                    "https://login.microsoftonline.com/f8cdef31-a31e-4b4a-93e4-5f571e91255a/v2.0"       // Auth v3.2, 2.0 token
                },
                // Audience validation takes place in JwtTokenExtractor
                ValidateAudience = false,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.FromMinutes(5),
                RequireSignedTokens = true
            };
    }
}