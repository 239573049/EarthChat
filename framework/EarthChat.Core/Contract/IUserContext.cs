namespace EarthChat.Core.Contract
{
	public interface IUserContext
	{
		Guid CurrentUserId { get; }

		string CurrentUserName { get; }

		bool IsAuthenticated { get; }

		string[] Roles { get; }
	}
}
