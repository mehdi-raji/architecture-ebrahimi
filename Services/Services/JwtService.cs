using Common;
using Entities;
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
	public class JwtService : IJwtService
	{
		private readonly SiteSettings _siteSettings;

		public JwtService(IOptionsSnapshot<SiteSettings> settings)
        {
			_siteSettings = settings.Value;
		}
        public string Generate(User user)
		{
			var secretKey = Encoding.UTF8.GetBytes(_siteSettings.JwtSettings.SecretKey);
			var signingCredentials = new SigningCredentials(new SymmetricSecurityKey(secretKey), SecurityAlgorithms.HmacSha256Signature);
			var claims = _getClaims(user);
			var descriptor = new SecurityTokenDescriptor
			{
				Issuer = _siteSettings.JwtSettings.Issuer,
				Audience = _siteSettings.JwtSettings.Audience,
				IssuedAt = DateTime.Now,
				NotBefore = DateTime.Now.AddMinutes(_siteSettings.JwtSettings.NotBeforeMinutes),
				Expires = DateTime.Now.AddMinutes(_siteSettings.JwtSettings.ExpirationMinutes),
				SigningCredentials = signingCredentials,
				Subject = new ClaimsIdentity(claims)
			};
			var tokenHandler = new JwtSecurityTokenHandler();
			var securityToken = tokenHandler.CreateToken(descriptor);
			var jwt = tokenHandler.WriteToken(securityToken);

			return jwt;
		}
		private IEnumerable<Claim> _getClaims(User user)
		{
			var list = new List<Claim>()
			{
				new Claim(ClaimTypes.Name,user.UserName),
				new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
				new Claim(ClaimTypes.MobilePhone, "09123456987"),

			};
			var roles = new Role[] {new Role { Name = "Admin"} };
			foreach (var role in roles)
			{
				list.Add(new Claim( ClaimTypes.Role, role.Name));
			}
			return list;
		}
	}
}
