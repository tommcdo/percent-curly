namespace PercentCurly.Templating.Case
{
    public interface ICaseStrategy
    {
        bool Detect(string name);
        Word[] SplitToWords(string name);
        string Apply(Word[] words);
    }
}
