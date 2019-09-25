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
    /// Defines values for GroupUserAccessRight.
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum GroupUserAccessRight
    {
        /// <summary>
        /// Removes permission to content in workspace
        /// </summary>
        [EnumMember(Value = "None")]
        None,
        /// <summary>
        /// Grants read access to content in workspace
        /// </summary>
        [EnumMember(Value = "Member")]
        Member,
        /// <summary>
        /// Grants administrator rights to workspace
        /// </summary>
        [EnumMember(Value = "Admin")]
        Admin,
        /// <summary>
        /// Grants read and write access to content in group
        /// </summary>
        [EnumMember(Value = "Contributor")]
        Contributor
    }
    internal static class GroupUserAccessRightEnumExtension
    {
        internal static string ToSerializedValue(this GroupUserAccessRight? value)
        {
            return value == null ? null : ((GroupUserAccessRight)value).ToSerializedValue();
        }

        internal static string ToSerializedValue(this GroupUserAccessRight value)
        {
            switch( value )
            {
                case GroupUserAccessRight.None:
                    return "None";
                case GroupUserAccessRight.Member:
                    return "Member";
                case GroupUserAccessRight.Admin:
                    return "Admin";
                case GroupUserAccessRight.Contributor:
                    return "Contributor";
            }
            return null;
        }

        internal static GroupUserAccessRight? ParseGroupUserAccessRight(this string value)
        {
            switch( value )
            {
                case "None":
                    return GroupUserAccessRight.None;
                case "Member":
                    return GroupUserAccessRight.Member;
                case "Admin":
                    return GroupUserAccessRight.Admin;
                case "Contributor":
                    return GroupUserAccessRight.Contributor;
            }
            return null;
        }
    }
}
