namespace BlogRealtime.Domain.Cryptography;

public interface ICryptographyHelper
{
    string HashPassword(string password);
    bool VerifyPassword(string password, string hash);
}
