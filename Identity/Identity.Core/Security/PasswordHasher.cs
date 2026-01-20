using System.Security.Cryptography;

namespace Identity.Core.Security;

public class PasswordHasher : IPasswordHasher
{
    private const int SaltSize = 16;
    private const int HashSize = 32;
    private const int Iterations = 100000;

    public (string, string) HashPassword(string password)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(password);
        var saltBytes = RandomNumberGenerator.GetBytes(SaltSize);
        var hashBytes = Rfc2898DeriveBytes.Pbkdf2(
            password,
            saltBytes,
            Iterations,
            HashAlgorithmName.SHA512,
            HashSize);
        return (Convert.ToBase64String(hashBytes), Convert.ToBase64String(saltBytes));
    }

    public bool VerifyPassword(string providedPassword, string hash, string salt)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(providedPassword);
            ArgumentException.ThrowIfNullOrWhiteSpace(hash);
            ArgumentException.ThrowIfNullOrWhiteSpace(salt);
            var saltBytes = Convert.FromBase64String(salt);
            var originalHashBytes = Convert.FromBase64String(hash);
            var testHashBytes = Rfc2898DeriveBytes.Pbkdf2(
                providedPassword,
                saltBytes,
                Iterations,
                HashAlgorithmName.SHA512,
                HashSize);
            return CryptographicOperations.FixedTimeEquals(originalHashBytes, testHashBytes);
        }
}
