using System.Collections.Generic;
using System.Linq;
using PercentCurly.Templating.Case;

namespace PercentCurly.Templating
{
    public class PlaceholderRegistry
    {
        private readonly IEnumerable<ICaseStrategy> caseStrategies;
        private readonly ISet<PlaceholderName> placeholderNames;

        public PlaceholderRegistry(IEnumerable<ICaseStrategy> caseStrategies)
        {
            this.caseStrategies = caseStrategies;
            this.placeholderNames = new HashSet<PlaceholderName>();
        }

        public Placeholder Register(string name)
        {
            var caseStrategy = caseStrategies.First(strategy => strategy.Detect(name));
            var placeholderName = new PlaceholderName(caseStrategy.SplitToWords(name));
            placeholderNames.Add(placeholderName);
            return new Placeholder(placeholderName, caseStrategy);
        }
    }
}
