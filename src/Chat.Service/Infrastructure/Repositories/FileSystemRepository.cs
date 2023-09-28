using Chat.Service.Domain.System.Aggregates;
using Chat.Service.Domain.System.Repositories;
using Masa.Contrib.Ddd.Domain.Repository.EFCore;

namespace Chat.Service.Infrastructure.Repositories;

public class FileSystemRepository : BaseRepository<ChatDbContext, FileSystem, Guid>, IFileSystemRepository
{
    public FileSystemRepository(ChatDbContext context, IUnitOfWork unitOfWork) : base(context, unitOfWork)
    {
    }
}