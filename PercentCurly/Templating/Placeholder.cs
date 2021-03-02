using PercentCurly.Templating.Case;

namespace PercentCurly.Templating
{
    public class Placeholder
    {
        private readonly PlaceholderName name;
        private readonly ICaseStrategy caseStrategy;

        public Placeholder(PlaceholderName name, ICaseStrategy caseStrategy)
        {
            this.name = name;
            this.caseStrategy = caseStrategy;
        }
    }
}
