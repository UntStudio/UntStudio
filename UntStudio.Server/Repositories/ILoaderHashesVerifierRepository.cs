namespace UntStudio.Server.Repositories
{
    public interface ILoaderHashesVerifierRepository
    {
        bool Verify(byte[] bytes);

        string GetHashFrom(string file);
    }
}