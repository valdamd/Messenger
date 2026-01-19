using System.Security.Cryptography;

namespace Identity.Core.Security;

public class PasswordHasher : IPasswordHasher
{
    private const int SaltSize = 16;
    private const int HashSize = 32;
    private const int Iterations = 100000;

    public string HashPassword(string password)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(password);

        var salt = RandomNumberGenerator.GetBytes(SaltSize);
        var hash = Rfc2898DeriveBytes.Pbkdf2(
            password,
            salt,
            Iterations,
            HashAlgorithmName.SHA512,
            HashSize);

        return $"{Convert.ToBase64String(salt)}:{Convert.ToBase64String(hash)}";
    }

    public bool VerifyPassword(string hashedPassword, string providedPassword)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(hashedPassword);
        ArgumentException.ThrowIfNullOrWhiteSpace(providedPassword);

        var parts = hashedPassword.Split(':');
        if (parts.Length != 2)
        {
            return false;
        }

        var salt = Convert.FromBase64String(parts[0]);
        var originalHash = Convert.FromBase64String(parts[1]);

        var testHash = Rfc2898DeriveBytes.Pbkdf2(
            providedPassword,
            salt,
            Iterations,
            HashAlgorithmName.SHA512,
            HashSize);

        return CryptographicOperations.FixedTimeEquals(originalHash, testHash);
    }
}
