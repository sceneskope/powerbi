// <auto-generated>
// Code generated by Microsoft (R) AutoRest Code Generator.
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.
// </auto-generated>

namespace SceneSkope.PowerBI.Models
{
    using Microsoft.Rest;
    using Newtonsoft.Json;
    using System.Linq;

    /// <summary>
    /// Connection string wrapper.
    /// </summary>
    public partial class ConnectionDetails
    {
        /// <summary>
        /// Initializes a new instance of the ConnectionDetails class.
        /// </summary>
        public ConnectionDetails()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the ConnectionDetails class.
        /// </summary>
        /// <param name="connectionString">A dataset connection string.</param>
        public ConnectionDetails(string connectionString)
        {
            ConnectionString = connectionString;
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        /// <summary>
        /// Gets or sets a dataset connection string.
        /// </summary>
        [JsonProperty(PropertyName = "connectionString")]
        public string ConnectionString { get; set; }

        /// <summary>
        /// Validate the object.
        /// </summary>
        /// <exception cref="ValidationException">
        /// Thrown if validation fails
        /// </exception>
        public virtual void Validate()
        {
            if (ConnectionString == null)
            {
                throw new ValidationException(ValidationRules.CannotBeNull, "ConnectionString");
            }
        }
    }
}
