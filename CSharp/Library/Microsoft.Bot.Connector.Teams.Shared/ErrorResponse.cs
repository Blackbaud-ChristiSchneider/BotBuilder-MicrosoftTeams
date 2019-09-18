// 9/17/19 Copied from https://github.com/microsoft/BotBuilder-V3/blob/31f4c001b8b1d7c218abbc5a1406c4c4bf654de3/CSharp/Library/Microsoft.Bot.Connector.Shared/ConnectorAPI/Models/ErrorResponse.cs

namespace Microsoft.Bot.Connector
{
    using Newtonsoft.Json;
    using System.Linq;

    /// <summary>
    /// An HTTP API response
    /// </summary>
    public partial class ErrorResponse
    {
        /// <summary>
        /// Initializes a new instance of the ErrorResponse class.
        /// </summary>
        public ErrorResponse()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the ErrorResponse class.
        /// </summary>
        /// <param name="error">Error message</param>
        public ErrorResponse(Error error = default(Error))
        {
            Error = error;
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        /// <summary>
        /// Gets or sets error message
        /// </summary>
        [JsonProperty(PropertyName = "error")]
        public Error Error { get; set; }

    }
}