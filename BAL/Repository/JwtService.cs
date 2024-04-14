using BAL.Interface;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;


namespace BAL.Repository
{
    public class JwtService : IJwtService
    {

        private readonly IConfiguration configuration;
       
        public JwtService(IConfiguration configuration)
        {
            this.configuration = configuration;
        }
        public string Generatetoken(string Email, string role)
        {
            // Token Generation 

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(configuration["Jwt:Key"]);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] { new Claim("email", Email), new Claim("Role", role) }),
                Expires = DateTime.UtcNow.AddHours(1), // Token expires in 1 hour
                Issuer = configuration["Jwt:Issuer"],
                //Audience = _audience,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key)

                , SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var jwtToken = tokenHandler.WriteToken(token);

            return jwtToken;
        }

        public bool ValidateToken(string token, out JwtSecurityToken jwtSecurityToken)
        {
            jwtSecurityToken = null;

            if (token == null)
            {
                return false;
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(configuration["Jwt:Key"]);
            try
            {
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key)
    ,
                    ValidateIssuer = true,
                    ValidateLifetime = true,
                    ValidateAudience = false,
                    ValidIssuer = configuration["Jwt:Issuer"],
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                jwtSecurityToken = (JwtSecurityToken)validatedToken;
                if (jwtSecurityToken != null)
                    return true;
                return false;

            }
            catch (Exception ex)
            {
                return (false);
            }
        }
    }
}
