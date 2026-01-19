using System.Security.Cryptography;

namespace Identity.Core.Services;

public class PasswordService
{
    public (string Hash, string Salt) Hash(string password)
    {
        var salt = BCrypt.Net.BCrypt.GenerateSalt();
        var hash = BCrypt.Net.BCrypt.HashPassword(password, salt);
        return (hash, salt);
    }

    public bool Verify(string password, string hash) => BCrypt.Net.BCrypt.Verify(password, hash);
}
