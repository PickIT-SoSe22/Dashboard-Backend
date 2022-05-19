using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;


public class Generator
{
    private readonly string _secret;

    public Generator(IConfiguration configuration)
    {
        var jwtSecret = configuration["JWT:Secret"];
        var jwtBytes = jwtSecret.Select(js => (byte) js).ToArray();
        _secret = Convert.ToBase64String(jwtBytes);
    }

    public string GenerateToken(string username, int expireTime = 30)
    {
        var symKey = Encoding.UTF8.GetBytes(_secret);
        var tokenHandler = new JwtSecurityTokenHandler();

        var utcNow = DateTime.UtcNow;
        var tokenDesc = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Name, username)
            }),
            Expires = utcNow.AddMinutes(expireTime),
            SigningCredentials =
                new SigningCredentials(new SymmetricSecurityKey(symKey), SecurityAlgorithms.HmacSha256Signature),
            Issuer = "HCI_ToDo",
            Audience = "todo.peasantpath.com"
        };
        var sToken = tokenHandler.CreateToken(tokenDesc);
        var token = tokenHandler.WriteToken(sToken);

        return token;
    }
}