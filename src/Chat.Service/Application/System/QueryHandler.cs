using Chat.Service.Application.System.Queries;
using Chat.Service.Domain.System.Repositories;

namespace Chat.Service.Application.System;

public class QueryHandler
{
    private readonly IFileSystemRepository _fileSystemRepository;

    public QueryHandler(IFileSystemRepository fileSystemRepository)
    {
        _fileSystemRepository = fileSystemRepository;
    }

    [EventHandler]
    public async Task DayUploadQuantityAsync(DayUploadQuantityQuery query)
    {
        var start = DateTime.Today;
        var end = start.AddDays(1);
        query.Result =
            await _fileSystemRepository.GetCountAsync(x =>
                start <= x.CreationTime && end >= x.CreationTime && x.Creator == query.UserId);
    }
}