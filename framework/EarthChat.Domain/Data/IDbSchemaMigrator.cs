namespace EarthChat.Domain.Data;

public interface IDbSchemaMigrator
{
    Task MigrateAsync();
}