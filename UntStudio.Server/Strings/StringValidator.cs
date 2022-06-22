using System;

namespace UntStudio.Server.Strings;

public sealed class StringValidator : IStringValidator
{
    private readonly string content;

    private bool success;



    public StringValidator(string content)
    {
        this.content = content;
        success = true;
    }



    public bool Success => success == true;

    public bool Failed => Success == false;



    public IStringValidator ContentNotNullOrWhiteSpace()
    {
        if (string.IsNullOrWhiteSpace(this.content))
        {
            success = false;
        }

        return this;
    }

    public IStringValidator MinCharacters(int count)
    {
        if (this.content.Length < count)
        {
            success = false;
        }

        return this;
    }

    public IStringValidator MaxCharacters(int count)
    {
        if (this.content.Length > count)
        {
            success = false;
        }

        return this;
    }

    public IStringValidator ShouldBeEqualToCharactersLenght(int count)
    {
        if (this.content.Length != count)
        {
            success = false;
        }

        return this;
    }

    public IStringValidator Return(out IStringValidator self)
    {
        self = this;
        return this;
    }

    public IStringValidator ThrowIfFailed(Type type)
    {
        if (type == null)
        {
            throw new ArgumentNullException(nameof(type));
        }

        ThrowIfFailed(type.Name);

        return this;
    }

    public IStringValidator ThrowIfFailed(string message)
    {
        if (message == null)
        {
            throw new ArgumentNullException(nameof(message));
        }

        return Failed
            ? ThrowIfFailed(new FailedStringValidationException(message))
            : this;
    }

    public IStringValidator ThrowIfFailed(Exception exception)
    {
        if (exception == null)
        {
            throw new ArgumentNullException(nameof(exception));
        }

        return Failed
            ? throw exception
            : this;
    }
}
