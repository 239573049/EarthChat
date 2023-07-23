namespace Chat.Service.Application.Example;

public class ExampleCommandHandler
{
    /// <summary>
    /// This use business DbContext
    /// </summary>
    private readonly ChatDbContext _dbContext;

    public ExampleCommandHandler(ChatDbContext dbContext) => _dbContext = dbContext;

    [EventHandler]
    public Task CreateAsync(CreateExampleCommand command)
    {
        //TODO:Create logic
        return Task.CompletedTask;
    }

    [EventHandler]
    public Task UpdateAsync(UpdateExampleCommand command)
    {
        //TODO:Update logic
        return Task.CompletedTask;
    }

    [EventHandler]
    public Task DeleteAsync(DeleteExampleCommand command)
    {
        //TODO:Delete logic
        return Task.CompletedTask;
    }
}
