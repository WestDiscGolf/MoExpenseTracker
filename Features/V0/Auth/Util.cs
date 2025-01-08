using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

using MoExpenseTracker.Core;
using MoExpenseTracker.Models;

namespace MoExpenseTracker.Features.V0.Auth;

class AuthUtil
{
    private readonly int SaltSize = 16; // 16 bytes
    private readonly int HashSize = 32; // 32 bytes
    private readonly int Iterations = 100000;
    private readonly HashAlgorithmName HashAlgorithmName = HashAlgorithmName.SHA512;
    private readonly int NonceSize = 64;

    public string Hash(string rawPassword)
    {
        byte[] salt = RandomNumberGenerator.GetBytes(SaltSize);

        byte[] hash = Rfc2898DeriveBytes.Pbkdf2(
            rawPassword, salt, Iterations, HashAlgorithmName, HashSize);

        var passwordHash = $"{Convert.ToHexString(hash)}-{Convert.ToHexString(salt)}";
        // System.Console.WriteLine(passwordHash.Length);
        return passwordHash;
    }

    public bool Compare(string rawPassword, string hashedPassword)
    {
        var passordParts = hashedPassword.Split("-");
        byte[] hash = Convert.FromHexString(passordParts[0]);
        byte[] salt = Convert.FromHexString(passordParts[1]);

        byte[] rawPasswordHash = Rfc2898DeriveBytes.Pbkdf2(
            rawPassword, salt, Iterations, HashAlgorithmName, HashSize);

        // don't do hash.SequenceEqual(rawPasswordHash)

        return CryptographicOperations.FixedTimeEquals(hash, rawPasswordHash);
    }

    public string GenerateJwt(AppConfig config, User user)
    {
        var secret = Encoding.UTF8.GetBytes(config.Key);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity([
                new Claim("id", user.Id.ToString()),
                // new Claim(ClaimTypes.Email, user.Email),
                new Claim("nonce", Convert.ToHexString(
                    RandomNumberGenerator.GetBytes(NonceSize)))
            ]),
            Expires = DateTime.UtcNow.AddSeconds(config.Expires),
            Issuer = config.Issuer,
            Audience = config.Audience,
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(secret),
                SecurityAlgorithms.HmacSha512Signature)
        };

        // using System.IdentityModel.Tokens.Jwt;
        // var tokenHandler = new JwtSecurityTokenHandler();
        // var jwtToken = tokenHandler.CreateToken(tokenDescriptor);
        // var stringJwtToken = tokenHandler.WriteToken(jwtToken);

        var tokenHandler = new JsonWebTokenHandler();
        var stringJwtToken = tokenHandler.CreateToken(tokenDescriptor);

        return stringJwtToken!;
    }
}