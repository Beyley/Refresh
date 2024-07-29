using Bunkum.Core;
using Refresh.GameServer.Database;
using Refresh.GameServer.Endpoints.Game.Levels.FilterSettings;
using Refresh.GameServer.Services;
using Refresh.GameServer.Types.Data;
using Refresh.GameServer.Types.UserData;

namespace Refresh.GameServer.Types.Levels.Categories;

public class CoolLevelsCategory : LevelCategory
{
    public CoolLevelsCategory() : base("coolLevels", new []{"lbpcool", "lbp2cool", "cool"}, false)
    {
        this.Name = "Cool Levels";
        this.Description = "Levels trending with players like you!";
        this.FontAwesomeIcon = "fire";
        this.IconHash = "g820625";
    }

    public override DatabaseList<GameLevel>? Fetch(RequestContext context, int skip, int count, DataContext dataContext,
        LevelFilterSettings levelFilterSettings, GameUser? _)
    {
        return dataContext.Database.GetCoolLevels(count, skip, dataContext.User, levelFilterSettings);
    }
}