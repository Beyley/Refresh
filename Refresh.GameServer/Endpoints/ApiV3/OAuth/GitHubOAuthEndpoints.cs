using AttribDoc.Attributes;
using Bunkum.Core;
using Bunkum.Core.Endpoints;
using Refresh.GameServer.Database;
using Refresh.GameServer.Endpoints.ApiV3.ApiTypes;
using Refresh.GameServer.Endpoints.ApiV3.ApiTypes.Errors;
using Refresh.GameServer.Services.OAuth;
using Refresh.GameServer.Services.OAuth.Clients;
using Refresh.GameServer.Time;
using Refresh.GameServer.Types.Data;
using Refresh.GameServer.Types.OAuth;
using Refresh.GameServer.Types.OAuth.GitHub;
using Refresh.GameServer.Types.UserData;

namespace Refresh.GameServer.Endpoints.ApiV3.OAuth;

public class GitHubOAuthEndpoints : EndpointGroup
{
    [ApiV3Endpoint("oauth/github/currentUserInformation")]
    [DocSummary("Gets information about the current user's linked GitHub account")]
    [DocError(typeof(ApiNotSupportedError), ApiNotSupportedError.OAuthProviderDisabledErrorWhen)]
    [DocError(typeof(ApiNotFoundError), ApiNotFoundError.OAuthTokenMissingErrorWhen)]
    [DocResponseBody(typeof(GitHubApiUserResponse))]
    public ApiResponse<GitHubApiUserResponse> CurrentUserInformation(
        RequestContext context,
        GameDatabaseContext database,
        OAuthService oAuthService,
        GameUser user,
        IDateTimeProvider timeProvider,
        DataContext dataContext)
    {
        if (!oAuthService.GetOAuthClient<GitHubOAuthClient>(OAuthProvider.GitHub, out GitHubOAuthClient? client))
            return ApiNotSupportedError.OAuthProviderDisabledError;
        
        GitHubApiUserResponse? userInformation = client.GetUserInformation(database, timeProvider, user);
        
        if (userInformation == null)
            return ApiNotFoundError.OAuthTokenMissingError;

        return userInformation;
    }
}