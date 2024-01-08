using Bunkum.Core.Services;
using NotEnoughLogs;
using Refresh.GameServer.Authentication;
using Refresh.GameServer.Configuration;
using Refresh.GameServer.Resources;

namespace Refresh.GameServer.Services;

public class GuidCheckerService : EndpointService
{
    
    private readonly GameServerConfig _config;
    private readonly HashSet<long> _validMainlineTextureGuids= [];
    private readonly HashSet<long> _validVitaTextureGuids = [];

    internal GuidCheckerService(GameServerConfig config, Logger logger) : base(logger)
    {
        this._config = config;
        //Get the resource streams for the LBP3 and LBPV files
        using Stream lbpStream = ResourceHelper.StreamFromResource("Refresh.GameServer.Resources.lbp3.txt");
        using Stream vitaStream = ResourceHelper.StreamFromResource("Refresh.GameServer.Resources.lbpv.txt");
        
        //Read the files into their respective hash sets
        ReadStream(lbpStream, this._validMainlineTextureGuids);
        ReadStream(vitaStream, this._validVitaTextureGuids);
    }

    // ReSharper disable once SuggestBaseTypeForParameter MOVING TO ISet<long> IS SLOWER AT RUNTIME PLS STOP RIDER
    private static void ReadStream(Stream stream, HashSet<long> set)
    {
        StreamReader reader = new(stream);

        //Read all the lines
        while (reader.ReadLine() is {} line)
        {
            //Parse out and add each line to the set
            set.Add(long.Parse(line));
        }
    }

    /// <summary>
    /// Returns whether or not the GUID is a valid texture GUID for the respective game
    /// </summary>
    /// <param name="game"></param>
    /// <param name="guid"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public bool IsTextureGuid(TokenGame game, long guid)
    {
        // Check if all guids are enabled
        if (this._config.EnableUnknownGuids) return true;
        
        //Allow g0 explicitly
        if (guid == 0) return true;
        
        return game switch
        {
            TokenGame.LittleBigPlanet1 => this._validMainlineTextureGuids.TryGetValue(guid, out _),
            TokenGame.LittleBigPlanet2 => this._validMainlineTextureGuids.TryGetValue(guid, out _),
            TokenGame.LittleBigPlanet3 => this._validMainlineTextureGuids.TryGetValue(guid, out _),
            TokenGame.LittleBigPlanetVita => this._validVitaTextureGuids.TryGetValue(guid, out _),
            TokenGame.LittleBigPlanetPSP => guid is >= 0 and <= 63, //PSP avatar GUIDs can be g0 - g63
            _ => throw new ArgumentOutOfRangeException(nameof(game), game, null),
        };
    }
}