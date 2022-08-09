using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using Sparcpoint.Models;
using Microsoft.Extensions.Configuration;

namespace Sparcpoint.BusinessLayer.User
{
    public class UserService : IUserService
    {
        protected IConfiguration _configuration;
        public UserService(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        private List<Sparcpoint.Models.User> _users = new List<Sparcpoint.Models.User>
        {
            new Sparcpoint.Models.User { Id = 1, UserName = "komal", Password = "pass@1234", Role = "admin"},
            new Sparcpoint.Models.User { Id = 2, UserName = "user2", Password = "password2", Role = "guest"}
        };

        public string Login(string userName, string password)
        {
            var user = _users.SingleOrDefault(x => x.UserName == userName && x.Password == password);

            // return null if user not found
            if (user == null)
            {
                return string.Empty;
            }

            // authentication successful so generate jwt token
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["ApiKey"]);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(ClaimTypes.Role, user.Role)
                }),

                Expires = DateTime.UtcNow.AddMinutes(5),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            user.Token = tokenHandler.WriteToken(token);

            return user.Token;
        }
    }
}

