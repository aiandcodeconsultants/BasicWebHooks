using System.Collections.Immutable;
using BasicWebHooks.Core;
using BasicWebHooks.Core.Extensions;
using Microsoft.Extensions.Configuration;

namespace BasicWebHooks.Configuration;

public class WebHookTypesAndTargetsFromConfiguration : IWebHookTypeReader, IWebHookTargetReader
{
    private readonly ImmutableList<WebHookType> WebHookTypes;
    private IEnumerable<WebHookTarget> WebHookTargets => WebHookTypes.SelectMany(type => type.Targets);

    public WebHookTypesAndTargetsFromConfiguration(IConfiguration configuration)
        => WebHookTypes = (configuration.GetSection("WebHooks")
                                        .Get<WebHookConfiguration>()
                ?? throw new InvalidOperationException("configuration is missing required \"WebHooks\" section"))
            .Select((x, i) => new WebHookType
            {
                Id = i + 1,
                Name = x.Key,
                Targets = x.Value.Select((y, j) => new WebHookTarget
                {
                    Id = (i * 100) + j + 1,
                    WebHookTypeId = i + 1,
                    InvokerType = y.InvokerType,
                    ParametersJson = y.ParametersJson,
                }).ToList(),
            })
            .SetParentReferences()
            .ToImmutableList();

    public ValueTask<WebHookTarget?> GetTargetById(long id, CancellationToken cancellationToken = default)
        => ValueTask.FromResult(WebHookTargets.FirstOrDefault(t => t.Id == id));

    public ValueTask<List<WebHookTarget>> ListTargets(CancellationToken cancellationToken = default)
        => ValueTask.FromResult(WebHookTargets.ToList());
    
    public async ValueTask<List<WebHookTarget>> ListTargetsByTypeId(long id, CancellationToken cancellationToken = default)
        => (await GetTypeById(id, cancellationToken))?.Targets ?? [];
    
    public ValueTask<WebHookType?> GetTypeById(long id, CancellationToken cancellationToken = default)
        => ValueTask.FromResult(WebHookTypes.Find(t => t.Id == id));

    public ValueTask<List<WebHookType>> ListTypes(CancellationToken cancellationToken = default)
        => ValueTask.FromResult(WebHookTypes.ToList());
}