namespace EarthChat.Core;

public interface IDataSeedContributor
{
    Task SeedAsync(DataSeedContext context);
}