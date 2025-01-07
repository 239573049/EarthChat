using EarthChat.AuthServer.Domain.Users;
using EarthChat.Domain;
using EarthChat.Domain.Data;
using Gnarly.Data;
using Microsoft.EntityFrameworkCore;

namespace EarthChat.AuthServer.EntityFrameworkCore;

[Registration(typeof(IDbSchemaMigrator))]
public class AuthDbSchemaMigrator(AuthDbContext dbContext) : IDbSchemaMigrator, IScopeDependency
{
    public async Task MigrateAsync()
    {
        
        await MigrateDatabaseSchemaAsync();
    }

    private async Task MigrateDatabaseSchemaAsync()
    {
        // 判断是否存在初始化角色admin
        if (!await dbContext.Users.AnyAsync(x => x.Username == "admin"))
        {
            var user = new User("admin", "239573049@qq.com");
            user.SetPassword("Aa123456.");
            user.Avatar = "https://avatars.githubusercontent.com/u/61819790?v=4";
            user.Signature = "这家伙很懒，什么都没留下";
            user.Roles.Add(RoleConstant.Admin);
            user.Roles.Add(RoleConstant.User);

            await dbContext.Users.AddAsync(user);
        }


        await dbContext.SaveChangesAsync();
    }
}