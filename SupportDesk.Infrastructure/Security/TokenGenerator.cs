using Microsoft.IdentityModel.Tokens;
using SupportDesk.Application.Contracts.Infraestructure.Security;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SupportDesk.Infrastructure.Security;

public class TokenGenerator : ITokenGenerator
{
    private readonly string _tokenSecret;
    private readonly TimeSpan _tokenLifetime;
    private readonly string _issuer;
    private readonly string _audience;

    public TokenGenerator(
        string tokenSecret, 
        double tokenLifetimeInMinutes, 
        string issuer, 
        string audience)
    {
        _tokenSecret = tokenSecret;
        _tokenLifetime = TimeSpan.FromMinutes(tokenLifetimeInMinutes);
        _issuer = issuer;
        _audience = audience;
    }

    public string GenerateToken(
        TokenGenerationRequest request, 
        CancellationToken cancellationToken = default)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(_tokenSecret);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(JwtRegisteredClaimNames.Sub, request.Email),
            new(JwtRegisteredClaimNames.Email, request.Email),
            new("userid", request.UserId.ToString()),
            new("isadmin", request.IsAdmin.ToString().ToLower()),
            new("trusted_member", "true"),
        };

        foreach (var claimPair in request.CustomClaims)
        {
            var value = claimPair.Value;
            string valueType;

            // Determinar el tipo de claim basado en el valor
            switch (value)
            {
                case bool b:
                    valueType = ClaimValueTypes.Boolean;
                    claims.Add(new Claim(claimPair.Key, b.ToString().ToLower(), valueType));
                    break;
                case int i:
                    valueType = ClaimValueTypes.Integer32;
                    claims.Add(new Claim(claimPair.Key, i.ToString(), valueType));
                    break;
                case double d:
                    valueType = ClaimValueTypes.Double;
                    claims.Add(new Claim(claimPair.Key, d.ToString(System.Globalization.CultureInfo.InvariantCulture), valueType));
                    break;
                default:
                    valueType = ClaimValueTypes.String;
                    claims.Add(new Claim(claimPair.Key, value.ToString()!, valueType));
                    break;
            }
        }

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.Add(_tokenLifetime),
            Issuer = _issuer,
            Audience = _audience,
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);

        var jwt = tokenHandler.WriteToken(token);

        return jwt;
    }

    public string? RefreshToken(
        string token, 
        CancellationToken cancellationToken = default)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(_tokenSecret);

        // Validate the existing token
        var validationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = false, // We want to allow expired tokens to be refreshed
            ValidateIssuerSigningKey = true,
            ValidIssuer = _issuer,
            ValidAudience = _audience,
            IssuerSigningKey = new SymmetricSecurityKey(key)
        };

        try
        {
            tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);
            var jwtToken = validatedToken as JwtSecurityToken;

            // Extract claims
            var claims = jwtToken.Claims.ToList();

            // Generate a new token with a new expiration
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.Add(_tokenLifetime), // Extend the expiration
                Issuer = _issuer,
                Audience = _audience,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var newToken = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(newToken);
        }
        catch (Exception)
        {
            // Handle the validation failure
            return null;
        }
    }
}
