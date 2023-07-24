using Chat.Service.Domain.Users.Aggregates;
using Chat.Service.Domain.Users.Repositories;
using Masa.BuildingBlocks.Data.UoW;
using Masa.Contrib.Ddd.Domain.Repository.EFCore;

namespace Chat.Service.Infrastructure.Repositories;

public class UserRepository : Repository<ChatDbContext,User,Guid>, IUserRepository
{
    public UserRepository(ChatDbContext context, IUnitOfWork unitOfWork) : base(context, unitOfWork)
    {
    }
    
}