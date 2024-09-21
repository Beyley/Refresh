using NotEnoughLogs;
using Refresh.Common.Extensions;
using Refresh.GameServer.Database;
using Refresh.GameServer.Time;
using Refresh.GameServer.Types.OAuth;

namespace Refresh.GameServer.Services.OAuth;

/// <summary>
/// A minimal implementation of the OAuth2 API (RFC 6749),
/// covering the authorization code and refresh token parts of the specification.
///
/// Also contains an implementation of the token revocation extension of the OAuth2 specification (RFC 7009).
/// </summary>
/// <seealso href="https://datatracker.ietf.org/doc/html/rfc6749"/>
/// <seealso href="https://datatracker.ietf.org/doc/html/rfc7009"/>
public abstract class OAuthClient : IDisposable
{
    protected HttpClient Client = null!;

    protected readonly Logger Logger;
    
    /// <summary>
    /// The provider associated with this OAuth2Service
    /// </summary>
    public abstract OAuthProvider Provider { get; }
    
    protected abstract Uri HttpBaseAddress { get; }

    protected abstract string TokenEndpoint { get; }
    protected abstract string TokenRevocationEndpoint { get; }
    public abstract bool TokenRevocationSupported { get; }
    
    protected abstract string ClientId { get; }
    protected abstract string ClientSecret { get; }
    
    protected abstract string RedirectUri { get; }

    protected OAuthClient(Logger logger)
    {
        this.Logger = logger;
    }

    public virtual void Initialize()
    {
        this.Client = new HttpClient();
        this.Client.BaseAddress = this.HttpBaseAddress;
    }

    /// <summary>
    /// Constructs a URL to send a user to, which they use to authorize Refresh
    /// </summary>
    /// <param name="state">The `state` parameter of the authorization</param>
    /// <returns>The authorization URL to redirect the user to</returns>
    public abstract string GetOAuthAuthorizationUrl(string state);
    
    /// <summary>
    /// Acquires an access and refresh token using the provided authorization code
    /// </summary>
    /// <param name="authCode">The authorization code</param>
    /// <returns>The acquired access and refresh tokens</returns>
    public OAuth2AccessTokenResponse AcquireTokenFromAuthorizationCode(string authCode)
    {
        HttpResponseMessage result = this.Client.PostAsync(this.TokenEndpoint, new FormUrlEncodedContent([
            new KeyValuePair<string, string>("grant_type", "authorization_code"),
            new KeyValuePair<string, string>("code", authCode),
            new KeyValuePair<string, string>("redirect_uri", this.RedirectUri),
            new KeyValuePair<string, string>("client_id", this.ClientId),
            new KeyValuePair<string, string>("client_secret", this.ClientSecret),
        ])).Result;

        // https://datatracker.ietf.org/doc/html/rfc6749#section-5.2
        if (result.StatusCode == BadRequest)
        {
            OAuth2ErrorResponse errorResponse = result.Content.ReadAsJson<OAuth2ErrorResponse>()!;

            throw new Exception($"Unexpected error {errorResponse.Error} when refreshing token! Description: {errorResponse.ErrorDescription}, URI: {errorResponse.ErrorUri}");
        }

        if (!result.IsSuccessStatusCode)
            throw new Exception($"Acquiring token failed, server returned status code {result.StatusCode}");

        if ((result.Content.Headers.ContentLength ?? 0) == 0)
            throw new Exception("Acquiring token failed, request returned no response");
        
        OAuth2AccessTokenResponse response = result.Content.ReadAsJson<OAuth2AccessTokenResponse>()!;

        if (response.RefreshToken == null)
            throw new Exception("OAuth2 response missing refresh token");

        return response;
    }

    /// <summary>
    /// Refreshes the passed OAuthTokenRelation, acquiring a new access token
    /// </summary>
    /// <param name="database">The database context associated with the token</param>
    /// <param name="token">The token to refresh</param>
    /// <param name="timeProvider">The time provider associated with the request</param>
    /// <returns>Whether the refresh succeeded, if failed, assume the token is invalid and authorization has been revoked.</returns>
    public bool RefreshToken(GameDatabaseContext database, OAuthTokenRelation token, IDateTimeProvider timeProvider)
    {
        HttpResponseMessage result = this.Client.PostAsync(this.TokenEndpoint, new FormUrlEncodedContent([
            new KeyValuePair<string, string>("grant_type", "refresh_token"),
            new KeyValuePair<string, string>("refresh_token", token.RefreshToken),
            new KeyValuePair<string, string>("client_id", this.ClientId),
            new KeyValuePair<string, string>("client_secret", this.ClientSecret),
        ])).Result;

        // https://datatracker.ietf.org/doc/html/rfc6749#section-5.2
        if (result.StatusCode == BadRequest)
        {
            OAuth2ErrorResponse errorResponse = result.Content.ReadAsJson<OAuth2ErrorResponse>()!;

            //Special cased error for when the refresh token is invalid
            if (errorResponse.Error == "invalid_grant")
                return false;

            throw new Exception($"Unexpected error {errorResponse.Error} when refreshing token! Description: {errorResponse.ErrorDescription}, URI: {errorResponse.ErrorUri}");
        }

        if (!result.IsSuccessStatusCode)
            throw new Exception($"Refreshing token failed, server returned status code {result.StatusCode}");

        if ((result.Content.Headers.ContentLength ?? 0) == 0)
            throw new Exception("Refreshing token failed, request returned no response");

        OAuth2AccessTokenResponse response = result.Content.ReadAsJson<OAuth2AccessTokenResponse>()!;

        database.UpdateOAuthToken(token, response, timeProvider);

        return true;
    }

    /// <summary>
    /// Revokes the OAuth token
    /// </summary>
    /// <param name="database"></param>
    /// <param name="token"></param>
    /// <exception cref="Exception"></exception>
    /// <exception cref="NotSupportedException"></exception>
    /// <seealso href="https://datatracker.ietf.org/doc/html/rfc7009"/>
    public void RevokeToken(GameDatabaseContext database, OAuthTokenRelation token)
    {
        if (!this.TokenRevocationSupported)
            throw new NotSupportedException("Revocation is not supported by this OAuth client!");
        
        // NOTE: As per https://datatracker.ietf.org/doc/html/rfc7009#autoid-5, revocation of an invalid token returns a 200 OK response, so 
        HttpResponseMessage result = this.Client.PostAsync(this.TokenRevocationEndpoint, new FormUrlEncodedContent([
            new KeyValuePair<string, string>("token", token.RefreshToken),
            new KeyValuePair<string, string>("token_type_hint", "refresh_token"),
            new KeyValuePair<string, string>("client_id", this.ClientId),
            new KeyValuePair<string, string>("client_secret", this.ClientSecret),
        ])).Result;
        
        // https://datatracker.ietf.org/doc/html/rfc6749#section-5.2
        if (result.StatusCode == BadRequest)
        {
            OAuth2ErrorResponse errorResponse = result.Content.ReadAsJson<OAuth2ErrorResponse>()!;

            throw new Exception($"Unexpected error {errorResponse.Error} when revoking token! Description: {errorResponse.ErrorDescription}, URI: {errorResponse.ErrorUri}");
        }

        if (result.StatusCode != OK)
            throw new Exception($"Failed to revoke OAuth token, got status code {result.StatusCode}");

        database.RevokeOAuthToken(token);
    }

    public void Dispose()
    {
        this.Client.Dispose();
    }
}
