// <auto-generated>
// Code generated by Microsoft (R) AutoRest Code Generator.
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.
// </auto-generated>

namespace SceneSkope.PowerBI.Models
{
    using Newtonsoft.Json;
    using System.Linq;

    /// <summary>
    /// A Power BI dashboard
    /// </summary>
    public partial class Dashboard
    {
        /// <summary>
        /// Initializes a new instance of the Dashboard class.
        /// </summary>
        public Dashboard()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the Dashboard class.
        /// </summary>
        /// <param name="id">The dashboard id</param>
        /// <param name="displayName">The dashboard display name</param>
        /// <param name="isReadOnly">Is ReadOnly dashboard</param>
        /// <param name="embedUrl">The dashboard embed url</param>
        public Dashboard(System.Guid id, string displayName = default(string), bool? isReadOnly = default(bool?), string embedUrl = default(string))
        {
            Id = id;
            DisplayName = displayName;
            IsReadOnly = isReadOnly;
            EmbedUrl = embedUrl;
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        /// <summary>
        /// Gets or sets the dashboard id
        /// </summary>
        [JsonProperty(PropertyName = "id")]
        public System.Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the dashboard display name
        /// </summary>
        [JsonProperty(PropertyName = "displayName")]
        public string DisplayName { get; set; }

        /// <summary>
        /// Gets or sets is ReadOnly dashboard
        /// </summary>
        [JsonProperty(PropertyName = "isReadOnly")]
        public bool? IsReadOnly { get; set; }

        /// <summary>
        /// Gets or sets the dashboard embed url
        /// </summary>
        [JsonProperty(PropertyName = "embedUrl")]
        public string EmbedUrl { get; set; }

        /// <summary>
        /// Validate the object.
        /// </summary>
        /// <exception cref="Microsoft.Rest.ValidationException">
        /// Thrown if validation fails
        /// </exception>
        public virtual void Validate()
        {
            //Nothing to validate
        }
    }
}
