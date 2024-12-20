﻿using Common;
using Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Services.Services
{
	public class JwtService : IJwtService, IScopedDependency
    {
		private readonly SiteSettings _siteSettings;
        private readonly SignInManager<User> signInManager;

        public JwtService(IOptionsSnapshot<SiteSettings> settings,SignInManager<User> signInManager)
        {
			_siteSettings = settings.Value;
            this.signInManager = signInManager;
        }
        public async Task<string>  GenerateAsync(User user)
		{
			var secretKey = Encoding.UTF8.GetBytes(_siteSettings.JwtSettings.SecretKey);
			var signingCredentials = new SigningCredentials(new SymmetricSecurityKey(secretKey), SecurityAlgorithms.HmacSha256Signature);

            var encryptionkey = Encoding.UTF8.GetBytes(_siteSettings.JwtSettings.EncryptKey); 
            var encryptingCredentials = new EncryptingCredentials(new SymmetricSecurityKey(encryptionkey), SecurityAlgorithms.Aes128KW, SecurityAlgorithms.Aes128CbcHmacSha256);

            var claims = await _getClaims(user);

            var descriptor = new SecurityTokenDescriptor
			{
				Issuer = _siteSettings.JwtSettings.Issuer,
				Audience = _siteSettings.JwtSettings.Audience,
				IssuedAt = DateTime.Now,
				NotBefore = DateTime.Now.AddMinutes(_siteSettings.JwtSettings.NotBeforeMinutes),
				Expires = DateTime.Now.AddMinutes(_siteSettings.JwtSettings.ExpirationMinutes),
				SigningCredentials = signingCredentials,
				EncryptingCredentials = encryptingCredentials,
                Subject = new ClaimsIdentity(claims)
			};
			var tokenHandler = new JwtSecurityTokenHandler();
			var securityToken = tokenHandler.CreateToken(descriptor);
			var jwt = tokenHandler.WriteToken(securityToken);

			return jwt;
		}
		private async Task<IEnumerable<Claim>> _getClaims(User user)
		{
            var userClaims = await signInManager.ClaimsFactory.CreateAsync(user);
			var list = new List<Claim>(userClaims.Claims) ;
			list.Add(new Claim(ClaimTypes.MobilePhone, "1023457"));
			return list;

            //var securityStampClaimType = new ClaimsIdentityOptions().SecurityStampClaimType;
			//var list = new List<Claim>()
			//{
			//	new Claim(ClaimTypes.Name,user.UserName),
			//	new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
			//	new Claim(ClaimTypes.MobilePhone, "09123456987"),
			//	new Claim(securityStampClaimType,user.SecurityStamp.ToString())

			//};
			//var roles = new Role[] {new Role { Name = "Admin"} };
			//foreach (var role in roles)
			//{
			//	list.Add(new Claim( ClaimTypes.Role, role.Name));
			//}
			//return list;
		}
	}
}
