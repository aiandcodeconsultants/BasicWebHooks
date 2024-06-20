namespace BasicWebHooks.Core;
public interface IWebHookTargetReader
{
    ValueTask<WebHookTarget?> GetTargetById(long id, CancellationToken cancellationToken = default);
    ValueTask<List<WebHookTarget>> ListTargets(CancellationToken cancellationToken = default);
    ValueTask<List<WebHookTarget>> ListTargetsByTypeId(long id, CancellationToken cancellationToken = default);
}