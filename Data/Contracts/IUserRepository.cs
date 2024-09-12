using Entities;

namespace Data.Contracts
{
    public interface IUserRepository : IRepository<User>
    {
		Task<User> GetByUserAndPass(string userName, string password, CancellationToken cancellationToken);
		Task AddAsync(User user, string password, CancellationToken cancellationToken);

	}
}