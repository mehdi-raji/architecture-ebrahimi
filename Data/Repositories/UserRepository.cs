using Common.Exceptions;
using Common.Utilities;
using Data.Contracts;
using Entities;
using Microsoft.EntityFrameworkCore;


namespace Data.Repositories
{
    public class UserRepository : Repository<User>, IUserRepository
	{
		public UserRepository(ApplicationDbContext dbContext) : base(dbContext)
		{
		}
		public Task<User> GetByUserAndPass(string userName,string password,CancellationToken cancellationToken)
		{
			var passwordHash = SecurityHelper.GetSha256Hash(password);
			return Table.Where(p => p.UserName == userName && p.PasswordHash == passwordHash).SingleOrDefaultAsync(cancellationToken);
		}
		public async Task AddAsync(User user, string password, CancellationToken cancellationToken)
		{

			var exists = await TableNoTracking.AnyAsync(p => p.UserName == user.UserName);
			if (exists)
				throw new BadRequestException("نام کاربری تکراری است");

			var passwordHash = SecurityHelper.GetSha256Hash(password);
			user.PasswordHash = passwordHash;
			await base.AddAsync(user, cancellationToken);
		}
	}
}
