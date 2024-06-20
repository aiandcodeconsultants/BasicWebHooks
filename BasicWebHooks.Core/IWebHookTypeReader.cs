namespace BasicWebHooks.Core;

public interface IWebHookTypeReader
{
    ValueTask<WebHookType?> GetTypeById(long id, CancellationToken cancellationToken = default);
    ValueTask<List<WebHookType>> ListTypes(CancellationToken cancellationToken = default);
}