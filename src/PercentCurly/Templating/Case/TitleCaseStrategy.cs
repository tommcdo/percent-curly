namespace PercentCurly.Templating.Case
{
    public class TitleCaseStrategy : DelimiterCaseStrategy
    {
        public TitleCaseStrategy() : base(" ") {}

        protected override string EachWord(Word word) => word.Capitalize();
    }
}
