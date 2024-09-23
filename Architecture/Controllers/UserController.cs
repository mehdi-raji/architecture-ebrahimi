using Architecture.Models;
using Common.Exceptions;
using Data.Contracts;
using Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NLog;
using Services.Services;
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
		private readonly IJwtService jwtService;
        private readonly UserManager<User> userManager;
        private readonly RoleManager<Role> roleManager;

        public UserController(IUserRepository userRepository,IJwtService jwtService,UserManager<User> userManager,RoleManager<Role> roleManager)
        {
			this.userRepository = userRepository;
			this.jwtService = jwtService;
            this.userManager = userManager;
            this.roleManager = roleManager;
        }
		[HttpGet("[action]")]
		[AllowAnonymous]
		public async Task<string> Token(string userName,string password,CancellationToken cancellationToken)
		{
            var user = await userManager.FindByNameAsync(userName);
            if (user == null)
				throw new BadRequestException("نام کاربری یا رمز عبور اشتباه است");
            var isPasswordValid = await userManager.CheckPasswordAsync(user, password);
            if (!isPasswordValid)
                throw new BadRequestException("نام کاربری یا رمز عبور اشتباه است");

            var jwt = await jwtService.GenerateAsync(user);
			return jwt;
		}
		[HttpGet("riseError")]
		public async Task<ApiResult> ReturnBadRequest()
		{
			//logger.Error("test");
			throw new BadRequestException("تست");	
		}
		[HttpPost]
		[AllowAnonymous]
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
                UserName = userDto.UserName,
                Email = userDto.Email
            };
            var result = await userManager.CreateAsync(user, userDto.Password);


            //await userRepository.AddAsync(user, userDto.Password, cancellationToken);
            return user;
        }

		[HttpGet]
        public async Task<ApiResult<User>> Get(int id, CancellationToken cancellationToken)
        {
            var user = await userRepository.GetByIdAsync(cancellationToken, id);

            if (user == null)
                return NotFound();

            await userRepository.UpdateSecuirtyStampAsync(user, cancellationToken);

            return user;
        }

    }
}
