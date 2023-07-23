
namespace Chat.Service.Services;

public class ExampleService : ServiceBase
{
    private IEventBus _eventBus => GetRequiredService<IEventBus>();

    public async Task<PaginatedListBase<ExampleGetListDto>> GetAsync(string? keyword, string? sort, int pageIndex = 1, int pageDataCount = 10)
    {
        var query = new ExampleGetListQuery(keyword, sort, pageIndex, pageDataCount);
        await _eventBus.PublishAsync(query);
        return query.Result;
    }

    public async Task CreateAsync(ExampleCreateUpdateDto dto)
    {
        var command = new CreateExampleCommand(dto);
        await _eventBus.PublishAsync(command);
    }

    public async Task UpdateAsync([FromQuery] Guid id, [FromBody] ExampleCreateUpdateDto dto)
    {
        var command = new UpdateExampleCommand(id, dto);
        await _eventBus.PublishAsync(command);
    }

    public async Task DeleteAsync([FromQuery] Guid id)
    {
        var command = new DeleteExampleCommand(id);
        await _eventBus.PublishAsync(command);
    }
}
