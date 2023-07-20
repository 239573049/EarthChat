using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace Chat.Data;

/* This is used if database provider does't define
 * IChatDbSchemaMigrator implementation.
 */
public class NullChatDbSchemaMigrator : IChatDbSchemaMigrator, ITransientDependency
{
    public Task MigrateAsync()
    {
        return Task.CompletedTask;
    }
}
