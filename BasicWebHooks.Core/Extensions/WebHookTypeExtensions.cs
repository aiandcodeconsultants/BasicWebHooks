namespace BasicWebHooks.Core.Extensions;

public static class WebHookTypeExtensions
{
    public static IEnumerable<WebHookType> SetParentReferences(this IEnumerable<WebHookType> types)
    {
        foreach (var type in types)
        {
            foreach (var target in type.Targets)
            {
                target.Type = type;
            }
            yield return type;
        }
    }
}