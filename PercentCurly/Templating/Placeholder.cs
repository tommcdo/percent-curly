using PercentCurly.Templating.Case;

namespace PercentCurly.Templating
{
    public class Placeholder
    {
        private static readonly ICaseStrategy NormalizedCase = new DashCaseStrategy();

        private readonly PlaceholderName name;
        private readonly ICaseStrategy caseStrategy;

        public Placeholder(PlaceholderName name, ICaseStrategy caseStrategy)
        {
            this.name = name;
            this.caseStrategy = caseStrategy;
        }

        public string GetNormalizedName()
        {
            return name.TransformCase(NormalizedCase);
        }

        public string WriteValue(Word[] value)
        {
            return caseStrategy.Write(value);
        }
    }
}
