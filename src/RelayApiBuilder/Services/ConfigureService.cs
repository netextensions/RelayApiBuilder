using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NetExtensions.RelayApiEntities;
using Newtonsoft.Json;

namespace NetExtensions.Services
{
    public abstract class ConfigureService
    {
        protected readonly DbContextOptions<RelayApiContext> Options;

        protected ConfigureService(DbContextOptions<RelayApiContext> options)
        {
            Options = options;
        }

        protected async Task AddResourceConfig(object data, ResourceHandlerConfiguration resourceHandlerConfigurations)
        {
            var json = JsonConvert.SerializeObject(data);
            var configData = new ResourceConfigData
            {
                Hash = json.GetMd5Hash(), DelayInMs = resourceHandlerConfigurations.DelayInMs, HttpStatusCode = resourceHandlerConfigurations.HttpStatusCode,
                InvalidToken = resourceHandlerConfigurations.InvalidToken, Body = json
            };
            await using var context = new RelayApiContext(Options);
            await context.ResourceConfigData.AddAsync(configData).ConfigureAwait(false);
            await context.SaveChangesAsync().ConfigureAwait(false);
        }
    }
}