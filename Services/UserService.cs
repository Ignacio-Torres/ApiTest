using APiTest.Entities;
using APiTest.Helpers;
using APiTest.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace APiTest.Services
{
    public interface IUserService
    {
        AuthenticateResponse Authenticate(AuthenticateRequest model);
        IEnumerable<User> GetAll();
        User GetById(Guid id);
    }

    public class UserService : IUserService
    {
        private readonly IConfiguration _configuration;

        private List<User> _users = new List<User>
        {
            new User { Id = Guid.Parse("fb77b72a-972c-4860-b44e-02c5fbdb2ab0"), Username = "user", Password = "user" },
            new User { Id = Guid.Parse("da2cfc25-b539-492b-a9a2-1373f92015c4"), Username = "admin", Password = "admin" },
            new User { Id = Guid.Parse("8c8fda60-79ec-4d38-9680-267ceb0abc9f"), Username = "client", Password = "client" }
        };

        public UserService( IConfiguration configuration)
        {
           
            _configuration = configuration;
        }

        public AuthenticateResponse Authenticate(AuthenticateRequest model)
        {
            var user = _users.SingleOrDefault(x => x.Username == model.Username && x.Password == model.Password);

            
            if (user == null) return null;

            var token = generateJwtToken(user);

            return new AuthenticateResponse(user, token);
        }

        public IEnumerable<User> GetAll()
        {
            return _users;
        }

        public User GetById(Guid id)
        {
            return _users.FirstOrDefault(x => x.Id == id);
        }
        private string generateJwtToken(User user)
        {
            string plainSignKey = _configuration.GetSection("Security:Token:PlainSignKey").Value;
            int expiredLogin = int.Parse(_configuration["Security:Token:ExpiredLogin"]);

            byte[] signKey = Encoding.ASCII.GetBytes(plainSignKey); 

            ClaimsIdentity theClaimsIdentity = new ClaimsIdentity();

            theClaimsIdentity.AddClaim(new Claim("Id", user?.Id.ToString() ?? string.Empty));

            JwtSecurityTokenHandler jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
            SecurityTokenDescriptor securityTokenDescriptor = new SecurityTokenDescriptor()
            {
                Subject = theClaimsIdentity,
                Expires = DateTime.UtcNow.AddMinutes(expiredLogin),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(signKey), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = jwtSecurityTokenHandler.CreateToken(securityTokenDescriptor);
            return jwtSecurityTokenHandler.WriteToken(token); 
        }
        
    }
}
