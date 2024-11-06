using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace WebApplication5.Authorization
{
    public sealed class TokenGenerator
    {
        public string Create(int id)
        {
            var claims = new List<Claim>
                {
                    new Claim("id", id.ToString()),
                };
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("YourSecretKeyThatIsVeryVeryLong12345"));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var tokenDescriptor = new JwtSecurityToken(
                issuer: "localhost.com",
                audience: "localhost.com",
                claims: claims,
                expires: DateTime.Now.AddMinutes(10),
                signingCredentials: credentials);
            var token = new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
            return token;
        }
    }
}
