namespace PercentCurly.Templating.Case
{
    public interface ICaseStrategy
    {
        bool Detect(string name);
        Word[] Read(string name);
        string Write(Word[] words);
    }
}
