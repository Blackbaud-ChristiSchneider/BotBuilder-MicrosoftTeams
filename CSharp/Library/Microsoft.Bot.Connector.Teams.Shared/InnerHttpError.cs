// 9/17/19 Copied from https://github.com/microsoft/BotBuilder-V3/blob/31f4c001b8b1d7c218abbc5a1406c4c4bf654de3/CSharp/Library/Microsoft.Bot.Connector.Shared/ConnectorAPI/Models/InnerHttpError.cs

namespace Microsoft.Bot.Connector
{
    using Newtonsoft.Json;
    using System.Linq;

    /// <summary>
    /// Object representing inner http error
    /// </summary>
    public partial class InnerHttpError
    {
        /// <summary>
        /// Initializes a new instance of the InnerHttpError class.
        /// </summary>
        public InnerHttpError()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the InnerHttpError class.
        /// </summary>
        /// <param name="statusCode">HttpStatusCode from failed request</param>
        /// <param name="body">Body from failed request</param>
        public InnerHttpError(int? statusCode = default(int?), object body = default(object))
        {
            StatusCode = statusCode;
            Body = body;
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        /// <summary>
        /// Gets or sets httpStatusCode from failed request
        /// </summary>
        [JsonProperty(PropertyName = "statusCode")]
        public int? StatusCode { get; set; }

        /// <summary>
        /// Gets or sets body from failed request
        /// </summary>
        [JsonProperty(PropertyName = "body")]
        public object Body { get; set; }

    }
}