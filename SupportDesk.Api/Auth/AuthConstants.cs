namespace SupportDesk.Api.Auth;

public static class AuthConstants
{
    public const string SupervisorUserPolicyName = "Supervisor";
    public const string SupervisorUserClaimName = "issupervisor";
    
    public const string AdminUserPolicyName = "Admin";
    public const string AdminUserClaimName = "isadmin";

    public const string TrustedMemberPolicyName = "Trusted";
    public const string TrustedMemberClaimName = "trusted_member";
}
