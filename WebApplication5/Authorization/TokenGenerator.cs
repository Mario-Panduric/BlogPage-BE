using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace WebApplication5.Authorization
{
    public sealed class TokenGenerator
    {
        public string Create(int id, string name, string email)
        {
            var claims = new List<Claim>
                {
                    new Claim("id", id.ToString()),
                    new Claim("name", name.ToString()),
                    new Claim("email", email.ToString()),
                };
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("YourSecretKeyThatIsVeryVeryLong12345"));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var tokenDescriptor = new JwtSecurityToken(
                issuer: "localhost.com",
                audience: "localhost.com",
                claims: claims,
                expires: DateTime.Now.AddHours(10),
                signingCredentials: credentials);
            var token = new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
            return token;
        }
    }
}
