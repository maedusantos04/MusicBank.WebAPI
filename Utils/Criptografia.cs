using System.Security.Cryptography;
using System.Text;

namespace MusicBank.WebAPI.Utils;

public class Criptografia
{
    public static string GerarHash(string senha)
    {
        return BCrypt.Net.BCrypt.HashPassword(senha);
    }

    public static bool CompararHash(string senhaInformada, string senhaBanco)
    {
        if (string.IsNullOrEmpty(senhaInformada) || string.IsNullOrEmpty(senhaBanco))
            return false;

        // Se for hash BCrypt (formato $2...)
        if (senhaBanco.StartsWith("$2", StringComparison.Ordinal))
        {
            try
            {
                return BCrypt.Net.BCrypt.Verify(senhaInformada, senhaBanco);
            }
            catch (BCrypt.Net.SaltParseException)
            {
                return false;
            }
        }

        // Se for hash SHA-256 gerada no SQL Server (HASHBYTES 'SHA2_256')
        using var sha256 = SHA256.Create();
        var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(senhaInformada));
        var hashHex = Convert.ToHexString(bytes);
        return string.Equals(hashHex, senhaBanco, StringComparison.OrdinalIgnoreCase);
    }
}
