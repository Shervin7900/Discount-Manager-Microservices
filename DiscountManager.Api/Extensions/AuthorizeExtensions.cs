using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace DiscountManager.Api.Extensions;

public static class AuthorizeExtensions
{
    /// <summary>
    /// Checks if the user has a specific scope.
    /// </summary>
    public static bool HasScope(this ClaimsPrincipal user, string scope)
    {
        return user.HasClaim(c => c.Type == "scope" && c.Value == scope);
    }

    /// <summary>
    /// Checks if the user has all of the specified scopes.
    /// </summary>
    public static bool HasAllScopes(this ClaimsPrincipal user, params string[] scopes)
    {
        return scopes.All(scope => user.HasScope(scope));
    }

    /// <summary>
    /// Checks if the user has any of the specified scopes.
    /// </summary>
    public static bool HasAnyScope(this ClaimsPrincipal user, params string[] scopes)
    {
        return scopes.Any(scope => user.HasScope(scope));
    }

    /// <summary>
    /// Checks if the user has a specific role.
    /// </summary>
    public static bool IsInRoleCustom(this ClaimsPrincipal user, string role)
    {
        return user.IsInRole(role);
    }

    /// <summary>
    /// Fluent authorization requirement for specific scopes.
    /// </summary>
    public static AuthorizationPolicyBuilder RequireScopes(this AuthorizationPolicyBuilder builder, params string[] scopes)
    {
        return builder.RequireClaim("scope", scopes);
    }
}
