using Common;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Webframework.Configuration
{
	public static class ServiceCollectionExtensions
	{
		public static void AddJwtAuthentication(this IServiceCollection services, JwtSettings jwtSettings)
		{
			services.AddAuthentication(options =>
			{
				options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
				options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
				options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
			}).AddJwtBearer(options =>
			{
				var secretKey = Encoding.UTF8.GetBytes(jwtSettings.SecretKey);

				var validationParameters = new TokenValidationParameters
				{
					ClockSkew = TimeSpan.Zero, // default: 5 min
					RequireSignedTokens = true,

					ValidateIssuerSigningKey = true,
					IssuerSigningKey = new SymmetricSecurityKey(secretKey),

					RequireExpirationTime = true,
					ValidateLifetime = true,

					ValidateAudience = true, //default : false
					ValidAudience =jwtSettings.Audience,

					ValidateIssuer = true, //default : false
					ValidIssuer = jwtSettings.Issuer,

				};

				options.RequireHttpsMetadata = false;
				options.SaveToken = true;
				options.TokenValidationParameters = validationParameters;
			});
		}
	}
}
