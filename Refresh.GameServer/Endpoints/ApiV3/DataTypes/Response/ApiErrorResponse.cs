using AttribDoc;

namespace Refresh.GameServer.Endpoints.ApiV3.DataTypes.Response;

[JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
public class ApiErrorResponse : IApiResponse, IDataConvertableFrom<ApiErrorResponse, Error>
{
    public required string Name { get; set; }
    public required string OccursWhen { get; set; }
    
    public static ApiErrorResponse? FromOld(Error? old)
    {
        if (old == null) return null;

        return new ApiErrorResponse
        {
            Name = old.Name,
            OccursWhen = old.OccursWhen,
        };
    }

    public static IEnumerable<ApiErrorResponse> FromOldList(IEnumerable<Error> oldList) => oldList.Select(FromOld).ToList()!;
}