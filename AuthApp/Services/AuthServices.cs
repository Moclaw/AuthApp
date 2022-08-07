using AuthApp.Configuration;
using AuthApp.Data;
using AuthApp.Models;
using AuthApp.Models.Interface;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AuthApp.Services
{
    public class AuthServices : IAuthServices
    {
        protected readonly ApplicationDbContext _context;
        protected IConfiguration Configuration { get; }
        protected readonly UserManager<IdentityUser> _userManager;

        public AuthServices(ApplicationDbContext context, IConfiguration configuration, UserManager<IdentityUser> userManager)
        {
            _context = context;
            Configuration = configuration;
            _userManager = userManager;
        }
      
        object IAuthServices.GenerateToken(IdentityUser identityUser)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(Configuration["JwtConfig:SecretKey"]);
            var userRole = _userManager.GetRolesAsync(identityUser);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, identityUser.UserName.ToString()),
                    new Claim(ClaimTypes.Email, identityUser.Email)
                }),

                Expires = DateTime.UtcNow.AddSeconds(double.Parse(Configuration["JwtConfig:ExpiryTimeInSeconds"])),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                Audience = Configuration["JwtConfig:Audience"],
                Issuer = Configuration["JwtConfig:Issuer"]
            };

            foreach (var role in userRole.Result)
            {
                tokenDescriptor.Subject.AddClaim(new Claim(ClaimTypes.Role, role));
            }
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenRe = tokenHandler.WriteToken(token);

            return new AuthResult()
            {
                Token = tokenRe,
                Success = true,
                RefreshToken = AddTokenToUser(tokenRe, identityUser).ToString()
            };
        }

        string AddTokenToUser(string token, IdentityUser user)
        {
            var userToken = new UserToken()
            {
                UserId = user.Id,
                AddedDate = DateTime.UtcNow,
                Token = token.Substring(token.Length - 43, 43)
            };
            _context.UserTokens.Add(userToken);
            _context.SaveChanges();
            return token.Substring(token.Length - 43, 43);
        }
    }
}
