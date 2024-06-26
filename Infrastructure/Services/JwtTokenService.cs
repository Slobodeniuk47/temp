using Core.Constants;
using Core.Entities.Identity;
using Infrastructure.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class JwtTokenService : IJwtTokenService
    {
        private readonly IConfiguration _configuration;
        private readonly UserManager<UserEntity> _userManager;
        public JwtTokenService(IConfiguration configuration, UserManager<UserEntity> userManager)
        {
            _configuration = configuration;
            _userManager = userManager;
        }

        public async Task<string> CreateTokenAsync(UserEntity user)
        {
            var roles = await _userManager.GetRolesAsync(user);
            var phoneNumber = "";
            var image = "";
            if (user.PhoneNumber != null)
            {
                phoneNumber = user.PhoneNumber;
            }
            if (user.Image != null)
            {
                image = user.IsGoogle == false ? $"{DirectoriesInProject.Api}/{DirectoriesInProject.UserImages}/{user.Image}" : user.Image;
            }
            var claims = new List<Claim>()
            {
                //new Claim("name", user.UserName), working!
                new Claim("id", user.Id.ToString()),
                new Claim("email", user.Email),
                new Claim("firstname", user.FirstName),
                new Claim("lastname", user.LastName),
                new Claim("phoneNumber", phoneNumber),
                new Claim("image", image)
            };

            foreach (var role in roles)
            {
                claims.Add(new Claim("roles", role));
            }
            var signinKey =
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSecretKey"]));
            var myCredentials = new SigningCredentials(signinKey, SecurityAlgorithms.HmacSha256);
            var jwt = new JwtSecurityToken(
                signingCredentials: myCredentials,
                expires: DateTime.Now.AddDays(7),
                claims: claims
                );
            return new JwtSecurityTokenHandler().WriteToken(jwt);
        }
    }
}
