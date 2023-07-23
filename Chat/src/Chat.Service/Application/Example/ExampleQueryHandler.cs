namespace Chat.Service.Application.Example;

public class ExampleQueryHandler
{
    /// <summary>
    /// This can use query's DbContext
    /// </summary>
    private readonly ChatDbContext _dbContext;

    public ExampleQueryHandler(ChatDbContext dbContext) => _dbContext = dbContext;

    [EventHandler]
    public Task GetListAsync(ExampleGetListQuery command)
    {
        //TODO:Get logic
        return Task.CompletedTask;
    }
}
