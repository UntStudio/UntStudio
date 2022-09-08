namespace UntStudio.Server.Strings;

public static class StringValidatorExtension
{
    public static IStringValidator Rules(this string source) => new StringValidator(source);
}
