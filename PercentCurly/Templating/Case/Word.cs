namespace PercentCurly.Templating.Case
{
    public class Word
    {
        private readonly string value;

        public Word(string value)
        {
            this.value = value.ToLowerInvariant();
        }

        public string Lowercase() => value;

        public string Uppercase() => value.ToUpperInvariant();

        public string Capitalize() => value.Substring(0, 1).ToUpperInvariant() + value.Substring(1);
    }
}
