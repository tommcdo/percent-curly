namespace PercentCurly.Templating.Case
{
    public class SnakeCaseStrategy : DelimiterCaseStrategy
    {
        public SnakeCaseStrategy() : base("_") {}

        protected override string EachWord(Word word) => word.Uppercase();
    }
}
