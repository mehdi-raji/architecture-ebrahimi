using Common.Utilities;
using Entities;
using Microsoft.EntityFrameworkCore;


namespace Data.Repositories
{
	public class UserRepository : Repository<User>
	{
		public UserRepository(ApplicationDbContext dbContext) : base(dbContext)
		{
		}
		public Task<User> GetByUserAndPass(string userName,string password,CancellationToken cancellationToken)
		{
			var passwordHash = SecurityHelper.GetSha256Hash(password);
			return Table.Where(p => p.UserName == userName && p.PasswordHash == passwordHash).SingleOrDefaultAsync(cancellationToken);
		}
	}
}
