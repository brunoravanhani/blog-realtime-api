using BlogRealtime.Domain.Cryptography;
using Isopoh.Cryptography.Argon2;

namespace BlogRealtime.Infra.Cryptography;

internal class Argon2CryptographyHelper : ICryptographyHelper
{
    public string HashPassword(string password)
    {
        return Argon2.Hash(password);
    }

    public bool VerifyPassword(string password, string hash)
    {
        return Argon2.Verify(hash, password);
    }
}
