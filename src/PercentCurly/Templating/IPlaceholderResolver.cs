namespace PercentCurly.FileSystem;

public interface IPlaceholderResolver
{
    string Resolve(string placeholderName);
}
