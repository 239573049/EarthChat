using EarthChat.Core;
using Microsoft.Extensions.Logging;

namespace EarthChat.Auth.Domains;

public class UserDataSeedContributor : IDataSeedContributor
{
    private readonly IEarthUserRepository _userRepository;
    private readonly ILogger<UserDataSeedContributor> _logger;

    public UserDataSeedContributor(IEarthUserRepository userRepository, ILogger<UserDataSeedContributor> logger)
    {
        _userRepository = userRepository;
        _logger = logger;
    }

    public async Task SeedAsync(DataSeedContext context)
    {
        if (await _userRepository.AnyAsync(x => x.UserName == "admin"))
        {
            _logger.LogInformation("The user has been seeded before.");
            return;
        }

        var user = new EarthUser
        {
            UserName = "admin",
            Email = "239573049@qq.com",
            EmailConfirmed = true,
            PhoneNumber = "13049809673",
            PhoneNumberConfirmed = true,
        };
        user.SetPassword("123QWE!");

        await _userRepository.InsertAsync(user);

        _logger.LogInformation("The user has been seeded.");
    }
}