using System.Net;
using System.Threading.Tasks;

namespace NetExtensions.RelayApiEntities
{
    public class ResourceHandlerConfiguration : ResourceConfig
    {
        public ResourceHandlerConfiguration(int delayInMs, HttpStatusCode? statusCode, string invalidToken, Task task = null)
        {
            InvalidToken = invalidToken;
            DelayInMs = delayInMs;
            if (statusCode.HasValue && statusCode != 0 && statusCode.Value != System.Net.HttpStatusCode.OK) HttpStatusCode = statusCode;
            Task = task;
        }

        public Task Task { get; }
    }
}