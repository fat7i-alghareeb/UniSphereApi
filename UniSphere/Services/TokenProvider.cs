

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using UniSphere.Api.DTOs.Auth;
using UniSphere.Api.Settings;
using JwtRegisteredClaimNames = System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames;

namespace UniSphere.Api.Services;

public sealed class TokenProvider(IOptions<JwtAuthOptions> options)
{

    private readonly JwtAuthOptions _jwtAuthOptions = options.Value;

    public AccessTokensDto Create(TokenRequest tokenRequest)
    {

        string accessToken = CreateAccessToken(tokenRequest);
        string refreshToken = CreateRefreshToken();
        return new AccessTokensDto(accessToken, refreshToken);
    }
    private string CreateAccessToken(TokenRequest tokenRequest)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtAuthOptions.Key));      
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
        List<Claim> claims =
        [
            new("studentId", tokenRequest.StudentId?.ToString() ?? ""),
            new("adminId", tokenRequest.AdminId?.ToString() ?? ""),
            new("superAdminId", tokenRequest.SuperAdminId?.ToString() ?? ""),
            new("professorId", tokenRequest.ProfessorId?.ToString() ?? ""),
            ..tokenRequest.Roles.Select(role => new Claim(ClaimTypes.Role, role)),
            //\new(JwtRegisteredClaimNames.Email, tokenRequest.Email),

        ];
        var tokenDescriptor = new SecurityTokenDescriptor
        {

            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(_jwtAuthOptions.ExpirationInMinutes),
            Issuer = _jwtAuthOptions.Issuer,
            Audience = _jwtAuthOptions.Audience,
            SigningCredentials = credentials
        };
        var handler = new JsonWebTokenHandler();
        string accessToken = handler.CreateToken(tokenDescriptor);
        return accessToken;
    }

    private string CreateRefreshToken()
    {
     
        byte[] randomNumber = RandomNumberGenerator.GetBytes(32);
        return Convert.ToBase64String(randomNumber);
    }
}

