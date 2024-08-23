using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using SupportDesk.Application.Contracts.Infraestructure.Security;
using SupportDesk.Infrastructure.Security;
using Xunit;

namespace SupportDesk.Infrastructure.UnitTests.Security
{
    public class TokenGeneratorTests
    {
        private readonly string _tokenSecret = "ThisIsASecretKeyForJwtTestPurposeOnly32";
        private readonly double _tokenLifetimeInMinutes = 60;
        private readonly string _issuer = "SupportDesk";
        private readonly string _audience = "SupportDeskUsers";

        private TokenGenerator CreateTokenGenerator()
        {
            return new TokenGenerator(_tokenSecret, _tokenLifetimeInMinutes, _issuer, _audience);
        }

        [Fact]
        public void GenerateToken_Should_Return_Valid_JWT_With_Correct_Claims()
        {
            // Arrange
            var tokenGenerator = CreateTokenGenerator();
            var request = new TokenGenerationRequest
            {
                UserId = Guid.NewGuid(),
                Email = "testuser@test.com",
                IsSupervisor = true,
                IsAdmin = false,
                CustomClaims = []
            };

            // Act
            var token = tokenGenerator.GenerateToken(request);
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_tokenSecret);

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = _issuer,
                ValidAudience = _audience,
                IssuerSigningKey = new SymmetricSecurityKey(key)
            };

            // Assert
            tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);

            var jwtToken = validatedToken as JwtSecurityToken;
            Assert.NotNull(jwtToken);

            Assert.Equal(_issuer, jwtToken.Issuer);
            Assert.Equal(_audience, jwtToken.Audiences.FirstOrDefault());

            var claims = jwtToken.Claims.ToList();
            Assert.Contains(claims, c => c.Type == JwtRegisteredClaimNames.Sub && c.Value == request.Email);
            Assert.Contains(claims, c => c.Type == JwtRegisteredClaimNames.Email && c.Value == request.Email);
            Assert.Contains(claims, c => c.Type == "userid" && c.Value == request.UserId.ToString());
            Assert.Contains(claims, c => c.Type == "issupervisor" && c.Value.Equals(request.IsSupervisor.ToString(), StringComparison.OrdinalIgnoreCase));
            Assert.Contains(claims, c => c.Type == "isadmin" && c.Value.Equals(request.IsAdmin.ToString(), StringComparison.OrdinalIgnoreCase));
        }

        [Fact]
        public void GenerateToken_Should_Include_Custom_Claims()
        {
            // Arrange
            var tokenGenerator = CreateTokenGenerator();
            var request = new TokenGenerationRequest
            {
                UserId = Guid.NewGuid(),
                Email = "testuser@test.com",
                IsSupervisor = false,
                IsAdmin = false,
                CustomClaims = {
                    { "custom_claim_1", "CustomValue1" },
                    { "custom_claim_2", 1234 } // Número entero como custom claim
                }
            };

            // Act
            var token = tokenGenerator.GenerateToken(request);
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_tokenSecret);

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = _issuer,
                ValidAudience = _audience,
                IssuerSigningKey = new SymmetricSecurityKey(key)
            };

            // Assert
            tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);

            var jwtToken = validatedToken as JwtSecurityToken;
            Assert.NotNull(jwtToken);

            var claims = jwtToken.Claims.ToList();
            Assert.Contains(claims, c => c.Type == "custom_claim_1" && c.Value == "CustomValue1");
            Assert.Contains(claims, c => c.Type == "custom_claim_2" && c.Value == "1234");
        }

        [Fact]
        public void GenerateToken_Should_Have_Correct_Expiration()
        {
            // Arrange
            var tokenGenerator = CreateTokenGenerator();
            var request = new TokenGenerationRequest
            {
                UserId = Guid.NewGuid(),
                Email = "testuser@test.com",
                IsSupervisor = true,
                IsAdmin = true,
                CustomClaims = []
            };

            // Act
            var token = tokenGenerator.GenerateToken(request);
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadJwtToken(token);

            // Assert
            var expectedExpiration = DateTime.UtcNow.AddMinutes(_tokenLifetimeInMinutes);
            Assert.True(jwtToken.ValidTo > DateTime.UtcNow);
            Assert.True(jwtToken.ValidTo <= expectedExpiration);
        }

        [Fact]
        public void RefreshToken_Should_Return_New_Valid_JWT()
        {
            // Arrange
            var tokenGenerator = CreateTokenGenerator();
            var request = new TokenGenerationRequest
            {
                UserId = Guid.NewGuid(),
                Email = "testuser@test.com",
                IsSupervisor = true,
                IsAdmin = false,
                CustomClaims = []
            };

            // Act
            var originalToken = tokenGenerator.GenerateToken(request);
            var refreshedToken = tokenGenerator.RefreshToken(originalToken);

            Assert.NotNull(refreshedToken);

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_tokenSecret);
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = _issuer,
                ValidAudience = _audience,
                IssuerSigningKey = new SymmetricSecurityKey(key)
            };

            tokenHandler.ValidateToken(refreshedToken, validationParameters, out SecurityToken validatedToken);
            var jwtToken = validatedToken as JwtSecurityToken;
            Assert.NotNull(jwtToken);

            var originalJwtToken = tokenHandler.ReadJwtToken(originalToken);

            // Assert that all original claims are present in the refreshed token
            foreach (var originalClaim in originalJwtToken.Claims)
            {
                Assert.Contains(jwtToken.Claims, c =>
                    c.Type == originalClaim.Type &&
                    c.Value == originalClaim.Value
                );
            }
        }

        [Fact]
        public void RefreshToken_Should_Return_Null_For_Invalid_Token()
        {
            var tokenGenerator = CreateTokenGenerator();
            var invalidToken = "invalidTokenValue";

            var refreshedToken = tokenGenerator.RefreshToken(invalidToken);

            Assert.Null(refreshedToken);
        }
    }
}
