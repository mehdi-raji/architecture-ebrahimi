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
        public Task UpdateSecuirtyStampAsync(User user, CancellationToken cancellationToken)
        {
            //user.SecurityStamp = Guid.NewGuid();
            return UpdateAsync(user, cancellationToken);
        }
        public override Task UpdateAsync(User entity, CancellationToken cancellationToken, bool saveNow = true)
        {
			//entity.SecurityStamp = Guid.NewGuid();
            return base.UpdateAsync(entity, cancellationToken, saveNow);
        }
        public override void Update(User entity, bool saveNow = true)
        {
			//entity.SecurityStamp = Guid.NewGuid();
            base.Update(entity, saveNow);
        }
        public Task UpdateLastLoginDateAsync(User user, CancellationToken cancellationToken)
        {
            user.LastLoginDate = DateTimeOffset.Now;
            return UpdateAsync(user, cancellationToken);
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
