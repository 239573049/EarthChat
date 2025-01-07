using EarthChat.Domain;

namespace EarthChat.AuthServer.Domain.Users;

public interface IUserRepository<TDbContext> : IRepository<TDbContext, User>
{
    
}