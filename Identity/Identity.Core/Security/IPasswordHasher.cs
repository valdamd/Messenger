namespace Identity.Core.Security;

internal interface IPasswordHasher
{
    (string Hash, string Salt) HashPassword(string password);

    bool VerifyPassword(string providedPassword, string hash, string salt);
}
