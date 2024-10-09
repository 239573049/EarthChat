using System.Security.Claims;

namespace EarthChat.Identity.Options;

public class EarthIdentityOptions
{
    public string IdentityUserId { get; set; } = ClaimTypes.NameIdentifier;

    public string IdentityRoleId { get; set; } = ClaimTypes.Role + "s";

    public string IdentityUserClaimId { get; set; } = ClaimTypes.UserData;
    
    public string TenantId { get; set; } = "tenant_id";
}