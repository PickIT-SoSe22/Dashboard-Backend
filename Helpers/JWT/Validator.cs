using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.IdentityModel.Tokens;

public class Validator
{
    private readonly string _secret;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="configuration"></param>
    public Validator(IConfiguration configuration)
    {
        var jwtSecret = configuration["JWT:Secret"];
        var jwtBytes = jwtSecret.Select(js => (byte) js).ToArray();
        _secret = Convert.ToBase64String(jwtBytes);
    }

    public ValidatedToken ValidateToken(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var validationParameters = GetValidationParameters();
        
        try
        {
            var principal = tokenHandler.ValidateToken(token, validationParameters, out var securityToken);

            return new ValidatedToken
            {
                Valid = true,
                Issuer = securityToken.Issuer,
                Username = principal.Identity?.Name
            };
        }
        catch (Exception exception)
        {
            return new ValidatedToken
            {
                Valid = false,
                Message = exception.Message
            };
            ;
        }
    }

    private TokenValidationParameters GetValidationParameters()
    {
        var symKey = Encoding.UTF8.GetBytes(_secret);
        return new TokenValidationParameters()
        {
            ValidateLifetime = false,
            ValidateAudience = true,
            ValidateIssuer = true,
            ValidIssuer = "PickIt_Backend_V1",
            ValidAudience = "mqtt.castrumnubis.com",
            IssuerSigningKey = new SymmetricSecurityKey(symKey),
            SignatureValidator = (token, parameters) => new JwtSecurityToken(token)
        };
    }
}