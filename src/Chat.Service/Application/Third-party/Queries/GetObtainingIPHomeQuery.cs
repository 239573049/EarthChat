using Chat.Contracts.third_party;

namespace Chat.Service.Application.Third_party.Queries;

/// <summary>
/// 
/// </summary>
public record GetObtainingIPHomeQuery(string ip) : Query<GetObtainingIPHomeDto>
{
    public override GetObtainingIPHomeDto? Result { get; set; }
}