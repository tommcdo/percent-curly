using System.Collections.Generic;
using System.Linq;
using PercentCurly.FileSystem;
using PercentCurly.Templating.Case;

namespace PercentCurly.Templating;

public class CaseTransformingPlaceholderResolver : IPlaceholderResolver
{
    private readonly IEnumerable<ICaseStrategy> caseStrategies;
    private readonly IDictionary<string, Word[]> placeholders;

    public CaseTransformingPlaceholderResolver(IEnumerable<ICaseStrategy> caseStrategies)
    {
        this.caseStrategies = caseStrategies;
        placeholders = new Dictionary<string, Word[]>();
    }

    public void Register(string name, string value)
    {
        var placeholder = caseStrategies.Detect(name);
        var words = caseStrategies.Read(value);
        placeholders[placeholder.GetNormalizedName()] = words;
    }

    public string Resolve(string name)
    {
        var placeholder = caseStrategies.Detect(name);
        var value = placeholders[placeholder.GetNormalizedName()];
        return placeholder.WriteValue(value);
    }
}

public static class CaseStrategiesExtensions
{
    public static Word[] Read(this IEnumerable<ICaseStrategy> caseStrategies, string name)
    {
        var strategy = caseStrategies.First(strategy => strategy.Detect(name));
        return strategy.Read(name);
    }
    public static Placeholder Detect(this IEnumerable<ICaseStrategy> caseStrategies, string name)
    {
        var strategy = caseStrategies.First(strategy => strategy.Detect(name));
        var placeholderName = new PlaceholderName(strategy.Read(name));
        return new Placeholder(placeholderName, strategy);
    }
}
