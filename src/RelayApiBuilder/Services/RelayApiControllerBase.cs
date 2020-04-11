using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using LanguageExt;
using Microsoft.AspNetCore.Mvc;
using NetExtensions.RelayApiEntities;
using Newtonsoft.Json;

namespace NetExtensions.Services
{
    public abstract class RelayApiControllerBase : ControllerBase
    {
        protected async Task<ActionResult> Configure(ResourceHandlerConfiguration configuration)
        {
            await configuration.Task;
            return StatusCode((int) HttpStatusCode.OK,
                $"{configuration.DelayInMs} milliseconds delay is saved, {configuration.HttpStatusCode} HttpStatusCode is saved,{configuration.InvalidToken} token is saved");
        }

        protected async Task<ActionResult> ResponseWrapperAsync(Task<HttpResponseMessage> task)
        {
            var response = await task;
            var responseText = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            if (string.IsNullOrEmpty(responseText))
                return StatusCode((int) response.StatusCode);
            try
            {
                var deserializeObject = JsonConvert.DeserializeObject(responseText);
                if (deserializeObject == null)
                    return StatusCode((int) response.StatusCode);
                return StatusCode((int) response.StatusCode, deserializeObject);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            return StatusCode((int) response.StatusCode);
        }

        protected async Task<ActionResult> GetResponse(Option<ResourceResponse> relayResponse, Func<string, Task<ActionResult>> func, string authorizationHeader)
        {
            return await relayResponse.Match(response =>
                response.StatusCode.Match(statusCode => Task.FromResult((ActionResult) StatusCode((int) statusCode)), () => response.InvalidToken.Match(func,
                    () => response.InvalidToken.Match(func, () => func(authorizationHeader))
                )), () => func(authorizationHeader));
        }
    }
}