using System.Net;
using LanguageExt;

namespace NetExtensions.RelayApiEntities
{
    public class ResourceResponse
    {
        public Option<HttpStatusCode> StatusCode { get; set; }
        public Option<string> InvalidToken { get; set; }
    }
}