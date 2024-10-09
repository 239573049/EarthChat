namespace EarthChat.Ddd;

public interface IHasTenant
{
    string? TenantId { get; set; }
}