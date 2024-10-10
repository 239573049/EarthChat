using EarthChat.Auth.Domains;
using EarthChat.EntityFrameworkCore.Repositories;

namespace EarthChat.Auth.EntityFrameworkCore.Users.Repositories;

/// <summary>
/// Repository for EarthUser
/// </summary>
/// <param name="dbContext"></param>
public class EarthUserRepository(AuthDbContext dbContext)
    : Repository<AuthDbContext, EarthUser>(dbContext), IEarthUserRepository
{
    
}