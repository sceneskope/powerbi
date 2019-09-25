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
    /// Defines values for CredentialType.
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum CredentialType
    {
        [EnumMember(Value = "Basic")]
        Basic,
        [EnumMember(Value = "Windows")]
        Windows,
        [EnumMember(Value = "Anonymous")]
        Anonymous,
        [EnumMember(Value = "OAuth2")]
        OAuth2,
        [EnumMember(Value = "Key")]
        Key
    }
    internal static class CredentialTypeEnumExtension
    {
        internal static string ToSerializedValue(this CredentialType? value)
        {
            return value == null ? null : ((CredentialType)value).ToSerializedValue();
        }

        internal static string ToSerializedValue(this CredentialType value)
        {
            switch( value )
            {
                case CredentialType.Basic:
                    return "Basic";
                case CredentialType.Windows:
                    return "Windows";
                case CredentialType.Anonymous:
                    return "Anonymous";
                case CredentialType.OAuth2:
                    return "OAuth2";
                case CredentialType.Key:
                    return "Key";
            }
            return null;
        }

        internal static CredentialType? ParseCredentialType(this string value)
        {
            switch( value )
            {
                case "Basic":
                    return CredentialType.Basic;
                case "Windows":
                    return CredentialType.Windows;
                case "Anonymous":
                    return CredentialType.Anonymous;
                case "OAuth2":
                    return CredentialType.OAuth2;
                case "Key":
                    return CredentialType.Key;
            }
            return null;
        }
    }
}
