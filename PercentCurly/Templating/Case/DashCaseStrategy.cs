namespace PercentCurly.Templating.Case
{
    public class DashCaseStrategy : DelimiterCaseStrategy
    {
        public DashCaseStrategy() : base("-") {}

        protected override string EachWord(Word word) => word.Lowercase();
    }
}
