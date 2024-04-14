using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAL.Interface
{
    public interface IJwtService
    {

        public string Generatetoken(string Email,string role);

        bool ValidateToken(string token, out JwtSecurityToken jwtSecurityToken);
    }
}
