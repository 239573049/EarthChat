using System.Threading.Tasks;

namespace Chat.Data;

public interface IChatDbSchemaMigrator
{
    Task MigrateAsync();
}
