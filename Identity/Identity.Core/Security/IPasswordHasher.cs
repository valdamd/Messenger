namespace Identity.Core.Security;

public interface IPasswordHasher
{
    (string Hash, string Salt) HashPassword(string password);

    bool VerifyPassword(string providedPassword, string hash, string salt);
}
