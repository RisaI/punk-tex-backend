using System;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;

using punk_tex_backend.Models;

namespace punk_tex_backend.Utils
{
    public static class JWT
    {
        private static JwtSecurityTokenHandler Handler;
        static JWT() {
            Handler = new JwtSecurityTokenHandler();
        }

        public static JwtSecurityToken Create(User user) {
            var claims = new Claim[] {
                new Claim("uid", user.ID.ToString())
            };
            
            return new JwtSecurityToken(
                null, null, // ISSUER and AUDIENCE not needed yet
                claims, 
                null, DateTime.Now.AddHours(2),
                new SigningCredentials(new SymmetricSecurityKey(Config.JWTSecret), SecurityAlgorithms.HmacSha256)
            );
        }

        public static string Serialize(this JwtSecurityToken token) {
            return Handler.WriteToken(token);
        }
    }
}