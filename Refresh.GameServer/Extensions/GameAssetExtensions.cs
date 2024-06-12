using Refresh.GameServer.Database;
using Refresh.GameServer.Types.Assets;

namespace Refresh.GameServer.Extensions;

public static class GameAssetExtensions
{
    public static void TraverseDependenciesRecursively(this GameAsset asset, GameDatabaseContext database, Action<string, GameAsset?> callback)
    {
        callback(asset.AssetHash, asset);
        foreach (AssetDependencyRelation internalAssetHash in database.GetAssetDependencies(asset))
        {
            GameAsset? internalAsset = database.GetAssetFromHash(internalAssetHash.Dependency);
            
            // Only run this if this is null, since the next recursion will trigger its own callback
            if(internalAsset == null)
                callback(internalAssetHash.Dependency, internalAsset);

            internalAsset?.TraverseDependenciesRecursively(database, callback);
        }
    }
}