namespace UntStudio.Server.Strings;

public interface IValidator
{
    bool Failed { get; }

    bool Success { get; }
}