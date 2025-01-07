using EarthChat.AuthServer.Domain.LoginLogs;
using EarthChat.EntityFrameworkCore;
using Gnarly.Data;

namespace EarthChat.AuthServer.EntityFrameworkCore.LoginLogs;

/// <summary>
/// 登录日志仓储
/// </summary>
/// <param name="dbContext"></param>
public class LoginLogRepository(AuthDbContext dbContext)
    : Repository<AuthDbContext, LoginLog>(dbContext), ILoginLogRepository, IScopeDependency
{
}