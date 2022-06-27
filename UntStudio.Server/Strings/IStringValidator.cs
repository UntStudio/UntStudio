using System;

namespace UntStudio.Server.Strings;

public interface IStringValidator : IValidator
{
    IStringValidator MinCharacters(int count);

    IStringValidator MaxCharacters(int count);

    IStringValidator ShouldBeEqualToCharactersLenght(int count);

    IStringValidator ContentNotNullOrWhiteSpace();

    IStringValidator ThrowIfFailed(Type type);

    IStringValidator ThrowIfFailed(string message);

    IStringValidator ThrowIfFailed(Exception exception);

    IStringValidator Return(out IStringValidator self);
}
