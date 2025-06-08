﻿//using System;
//using System.Collections.Generic;
//using System.IdentityModel.Tokens.Jwt;
//using System.Linq;
//using System.Security.Claims;
//using System.Text;
//using System.Threading.Tasks;
//using Microsoft.Extensions.Configuration;
//using Microsoft.IdentityModel.Tokens;
//using MyApp.Application.CQRS.User.Queries.Authenticate;
//using MyApp.Application.Interfaces;
//using MyApp.Core.Entities;
//using MyApp.Infrastructure.Data;

//namespace MyApp.Infrastructure.Repositories
//{
//    public class TokenRepository : ITokenRepository
//    {
//        private readonly IConfiguration configuration;

//        public TokenRepository(IConfiguration configuration)
//        {
//            this.configuration = configuration;
//        }

//        public string CreateJWTToken(User user, List<string> roles)
//        {
//            var claims = new List<Claim>();
//            claims.Add(new Claim(ClaimTypes.UserData, user.Username));
//            foreach (var role in roles)
//            {
//                claims.Add(new Claim(ClaimTypes.Role, role));
//            }
//            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]));
//            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

//            var token = new JwtSecurityToken(
//                configuration["Jwt:Issuer"],
//                configuration["Jwt:Audience"],
//                claims,
//                expires: DateTime.Now.AddMinutes(15),
//                signingCredentials: credentials
//            );
//            return new JwtSecurityTokenHandler().WriteToken(token);
//        }
//    }
//}
