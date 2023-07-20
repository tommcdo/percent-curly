using System.Linq;

namespace PercentCurly.Templating.Case
{
    public abstract class DelimiterCaseStrategy : ICaseStrategy
    {
        private readonly string delimiter;

        public DelimiterCaseStrategy(string delimiter)
        {
            this.delimiter = delimiter;
        }

        public bool Detect(string name)
        {
            return name.Contains(delimiter);
        }

        public Word[] Read(string name)
        {
            return name.Split(delimiter).Select(word => new Word(word)).ToArray();
        }

        public string Write(Word[] words)
        {
            return string.Join(delimiter, words.Select(EachWord).ToArray());
        }

        protected abstract string EachWord(Word word);

        protected string FirstWord(Word word) => EachWord(word);
    }
}
