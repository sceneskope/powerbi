// <auto-generated>
// Code generated by Microsoft (R) AutoRest Code Generator.
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.
// </auto-generated>

namespace SceneSkope.PowerBI.Models
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;
    using System.Runtime;
    using System.Runtime.Serialization;

    /// <summary>
    /// Defines values for CrossFilteringBehavior.
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum CrossFilteringBehavior
    {
        /// <summary>
        /// For filtering purposes, filter will be applied on the table where
        /// values are being aggregated
        /// </summary>
        [EnumMember(Value = "OneDirection")]
        OneDirection,
        /// <summary>
        /// For filtering purposes, both tables are treated as if they're a
        /// single table
        /// </summary>
        [EnumMember(Value = "BothDirections")]
        BothDirections,
        /// <summary>
        /// Cross filtering behavior defined automatically
        /// </summary>
        [EnumMember(Value = "Automatic")]
        Automatic
    }
    internal static class CrossFilteringBehaviorEnumExtension
    {
        internal static string ToSerializedValue(this CrossFilteringBehavior? value)
        {
            return value == null ? null : ((CrossFilteringBehavior)value).ToSerializedValue();
        }

        internal static string ToSerializedValue(this CrossFilteringBehavior value)
        {
            switch( value )
            {
                case CrossFilteringBehavior.OneDirection:
                    return "OneDirection";
                case CrossFilteringBehavior.BothDirections:
                    return "BothDirections";
                case CrossFilteringBehavior.Automatic:
                    return "Automatic";
            }
            return null;
        }

        internal static CrossFilteringBehavior? ParseCrossFilteringBehavior(this string value)
        {
            switch( value )
            {
                case "OneDirection":
                    return CrossFilteringBehavior.OneDirection;
                case "BothDirections":
                    return CrossFilteringBehavior.BothDirections;
                case "Automatic":
                    return CrossFilteringBehavior.Automatic;
            }
            return null;
        }
    }
}
