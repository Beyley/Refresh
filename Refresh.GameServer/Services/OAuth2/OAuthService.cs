using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Bunkum.Core.Services;
using NotEnoughLogs;
using Refresh.GameServer.Configuration;
using Refresh.GameServer.Services.OAuth2.Clients;
using Refresh.GameServer.Types.OAuth2;

namespace Refresh.GameServer.Services.OAuth2;

public class OAuthService : EndpointService
{
    private readonly IntegrationConfig _integrationConfig;

    private Dictionary<OAuthProvider, OAuthClient> _clients;
    
    public OAuthService(Logger logger, IntegrationConfig integrationConfig) : base(logger)
    {
        this._integrationConfig = integrationConfig;

        this._clients = new Dictionary<OAuthProvider, OAuthClient>();
        if (integrationConfig.DiscordOAuthEnabled)
            this._clients[OAuthProvider.Discord] = new DiscordOAuthClient(logger, integrationConfig);
        
        // Initialize all the OAuth clients
        foreach ((OAuthProvider provider, OAuthClient? client) in this._clients)
        {
            logger.LogInfo(RefreshContext.Startup, "Initializing OAuth client {0}", provider);
            client.Initialize();
        }
    }

    public bool GetOAuthClient(OAuthProvider provider, [MaybeNullWhen(false)] out OAuthClient client) => this._clients.TryGetValue(provider, out client);
    public bool GetOAuthClient<T>(OAuthProvider provider, [MaybeNullWhen(false)] out T client) where T : class
    {
        bool ret = this._clients.TryGetValue(provider, out OAuthClient? rawClient);

        if(rawClient != null) 
            Debug.Assert(rawClient.GetType().IsAssignableTo(typeof(T)), "Acquired client must be assignable to type parameter");
        
        client = rawClient as T;
        
        return ret;
    }

    public T? GetOAuthClient<T>(OAuthProvider provider) where T : class 
        => this._clients.GetValueOrDefault(provider) as T;
}