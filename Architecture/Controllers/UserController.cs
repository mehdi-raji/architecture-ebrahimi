using Architecture.Models;
using Data.Contracts;
using Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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

		public UserController(IUserRepository userRepository)
        {
			this.userRepository = userRepository;
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
