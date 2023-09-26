namespace Chat.Service.Application.Users.Queries;

public record GetFriendRequestListQuery(int Page,int PageSize): Query<PaginatedListBase<FriendRequestDto>>
{
    public override PaginatedListBase<FriendRequestDto> Result { get; set; }
}