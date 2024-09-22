using Architecture.Models;
using Common.Exceptions;
using Data.Contracts;
using Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NLog;
using Webframework.API;
using Webframework.Filters;

namespace Architecture.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	[ApiResultFilter]
	public class UserController : ControllerBase
	{
		private readonly IUserRepository userRepository;
		private readonly ILogger<UserController> logger;


		public UserController(IUserRepository userRepository,ILogger<UserController> logger)
        {
			this.userRepository = userRepository;
			this.logger = logger;
		}

		[HttpGet]
		public async Task<ApiResult> ReturnBadRequest()
		{
			//logger.Error("test");
			throw new BadRequestException("تست");	
		}
		[HttpPost]
		public async Task<ApiResult<User>> Create(UserDto userDto, CancellationToken cancellationToken)
		{
			//var exists = await userRepository.TableNoTracking.AnyAsync(p => p.UserName == userDto.UserName);
			//if (exists)
			//    return BadRequest("نام کاربری تکراری است");

			var user = new User
			{
				Age = userDto.Age,
				FullName = userDto.FullName,
				Gender = userDto.Gender,
				UserName = userDto.UserName
			};
			await userRepository.AddAsync(user, userDto.Password, cancellationToken);
			return user;
		}
	}
}
