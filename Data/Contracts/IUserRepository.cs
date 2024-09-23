using Entities;

namespace Data.Contracts
{
    public interface IUserRepository : IRepository<User>
    {
		Task<User> GetByUserAndPass(string userName, string password, CancellationToken cancellationToken);
		Task AddAsync(User user, string password, CancellationToken cancellationToken);
        Task UpdateSecuirtyStampAsync(User user, CancellationToken cancellationToken);
        Task UpdateLastLoginDateAsync(User user, CancellationToken cancellationToken);


    }
}