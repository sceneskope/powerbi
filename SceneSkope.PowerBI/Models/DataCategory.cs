using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace SceneSkope.PowerBI.Models
{
    [JsonConverter(typeof(StringEnumConverter), false)]
    public enum DataCategory
    {
        Default,
        Address,
        City,
        Continent,
        Country,
        Image,
        ImageUrl,
        Latitude,
        Longitude,
        Organization,
        Place,
        PostalCode,
        StateOrProvince,
        WebUrl
    }
}