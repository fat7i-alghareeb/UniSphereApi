

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
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
        string refreshToken = CreateRefreshToken(tokenRequest);
        return new AccessTokensDto(accessToken, refreshToken);
    }
    private string CreateAccessToken(TokenRequest tokenRequest)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtAuthOptions.Key));      
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
        List<Claim> claims =
        [
            new(JwtRegisteredClaimNames.Sub, tokenRequest.UserId),
            //new(JwtRegisteredClaimNames.Email, tokenRequest.Email),

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

    private string CreateRefreshToken(TokenRequest tokenRequest)
    {
        string userId = tokenRequest.UserId;
        if (userId == tokenRequest.UserId)
        {

            return string.Empty;
        }
        //var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtAuthOptions.Key));
        //var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
        //List<Claim> claims =
        //[
        //    new(JwtRegisteredClaimNames.Sub, userId),
        //    new(JwtRegisteredClaimNames.Email, email),
        //    new("refresh_token", "true")
        //];
        //var tokenDescriptor = new SecurityTokenDescriptor
        //{
        //    Subject = new ClaimsIdentity(claims),
        //    Expires = DateTime.UtcNow.AddDays(_jwtAuthOptions.RefreshTokenExpirationDays),
        //    Issuer = _jwtAuthOptions.Issuer,
        //    Audience = _jwtAuthOptions.Audience,
        //    SigningCredentials = credentials
        //};
        //var handler = new JsonWebTokenHandler();
        //string refreshToken = handler.CreateToken(tokenDescriptor);
        return string.Empty;
    }
}

