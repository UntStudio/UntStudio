namespace UntStudio.Server.Repositories;

public interface IHashesVerifierRepository
{
    bool Verify(byte[] bytes);

    string GetHashFrom(string file);
}