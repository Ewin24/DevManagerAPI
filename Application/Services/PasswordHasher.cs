namespace Application.Services;

using System.Security.Cryptography;
using System.Text;

/// <summary>
/// Helper compartido de hashing de contraseñas (esquema HMACSHA512).
/// Reproduce exactamente el esquema histórico de AuthService y Seeder.sql:
/// salt = new HMACSHA512().Key; hash = hmac.ComputeHash(UTF8(password)).
/// </summary>
public static class PasswordHasher
{
    public static (byte[] Hash, byte[] Salt) HashPassword(string password)
    {
        using var hmac = new HMACSHA512();
        var salt = hmac.Key;
        var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
        return (hash, salt);
    }

    public static bool VerifyPassword(string password, byte[] storedHash, byte[] storedSalt)
    {
        using var hmac = new HMACSHA512(storedSalt);
        var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
        return computedHash.SequenceEqual(storedHash);
    }
}