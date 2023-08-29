using Chat.Service.Application.System.Commands;
using Chat.Service.Domain.System.Aggregates;
using Chat.Service.Domain.System.Repositories;

namespace Chat.Service.Application.System;

public class CommandHandler
{
    private readonly IFileSystemRepository _fileSystemRepository;
    private readonly IUserContext _userContext;


    public CommandHandler(IFileSystemRepository fileSystemRepository, IUserContext userContext)
    {
        _fileSystemRepository = fileSystemRepository;
        _userContext = userContext;
    }

    [EventHandler]
    public async Task CreateFileSystemAsync(CreateFileSystemCommand command)
    {
        var fileSystem = new FileSystem
        {
            FileName = command.FileName,
            Uri = command.uri,
            FullName = command.FullName,
            Size = command.Size
        };

        await _fileSystemRepository.AddAsync(fileSystem);
    }

    [EventHandler]
    public async Task DeleteFileSystemAsync(DeleteFileSystemCommand command)
    {
        var fileSystem =
            await _fileSystemRepository.FindAsync(x =>
                x.Uri == command.Uri && x.Creator == _userContext.GetUserId<Guid>());

        if (fileSystem != null)
            await _fileSystemRepository.RemoveAsync(fileSystem);
        else
            throw new UserFriendlyException("文件不存在");
    }
}