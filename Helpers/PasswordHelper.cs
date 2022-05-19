using System.Security.Cryptography;
using System.Text;


public static class PasswordHelper
{
    public static byte[] Hash(string value, IEnumerable<byte> salt)
    {
        return Hash(Encoding.UTF8.GetBytes(value), salt);
    }

    public static byte[] Hash(IEnumerable<byte> value, IEnumerable<byte> salt)
    {
        var saltedValue = value.Concat(salt).ToArray();

        return new SHA256Managed().ComputeHash(saltedValue);
    }

    public static bool ConfirmPassword(IEnumerable<byte> passwordHash, string password, IEnumerable<byte> salt)
    {
        var generatedPasswordHash = Hash(password, salt);
        return passwordHash.SequenceEqual(generatedPasswordHash);
    }
}