namespace Identity.Core.Services;

public static class IdentityErrors
{
    public static readonly IdentityError EmailAlreadyExists = new(
        "Identity.EmailAlreadyExists",
        "Пользователь с таким email уже существует.");

    public static readonly IdentityError UserNotFound = new(
        "Identity.UserNotFound",
        "Пользователь с таким email не найден.");

    public static readonly IdentityError InvalidPassword = new(
        "Identity.InvalidPassword",
        "Неверный пароль.");

    public static readonly IdentityError InvalidRefreshToken = new(
        "Identity.InvalidRefreshToken",
        "Refresh-токен недействителен или истёк.");
}
