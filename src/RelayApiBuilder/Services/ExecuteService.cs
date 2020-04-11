using System.Linq;
using System.Net;
using System.Threading.Tasks;
using LanguageExt;
using Microsoft.EntityFrameworkCore;
using NetExtensions.RelayApiEntities;
using Newtonsoft.Json;

namespace NetExtensions.Services
{
    public abstract class ExecuteService
    {
        protected readonly DbContextOptions<RelayApiContext> Options;

        protected ExecuteService(DbContextOptions<RelayApiContext> options)
        {
            Options = options;
        }

        protected async Task<Option<ResourceResponse>> GetResourceResponse(object data)
        {
            var json = JsonConvert.SerializeObject(data);
            var md5Hash = json.GetMd5Hash();
            await using var context = new RelayApiContext(Options);
            var firstOrDefaultAsync = await context.ResourceConfigData.Where(d => d.Hash == md5Hash).OrderBy(x => x.Id).FirstOrDefaultAsync();
            if (firstOrDefaultAsync == null)
                return Option<ResourceResponse>.None;

            context.ResourceConfigData.Remove(firstOrDefaultAsync);
            await context.SaveChangesAsync();
            await Task.Delay(firstOrDefaultAsync.DelayInMs);
            var status = firstOrDefaultAsync.HttpStatusCode ?? Option<HttpStatusCode>.None;
            var invalidToken = string.IsNullOrWhiteSpace(firstOrDefaultAsync.InvalidToken) ? Option<string>.None : firstOrDefaultAsync.InvalidToken;
            return status.Match(s => new ResourceResponse {StatusCode = s, InvalidToken = invalidToken.Match(t => t, () => Option<string>.None)},
                () => invalidToken.Match(t => new ResourceResponse {StatusCode = Option<HttpStatusCode>.None, InvalidToken = t}, () => Option<ResourceResponse>.None));
        }
    }
}