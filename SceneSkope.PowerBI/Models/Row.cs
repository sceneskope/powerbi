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
    /// A Power BI data row
    /// </summary>
    public partial class Row
    {
        /// <summary>
        /// Initializes a new instance of the Row class.
        /// </summary>
        public Row()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the Row class.
        /// </summary>
        /// <param name="id">The unique row id</param>
        public Row(string id = default(string))
        {
            Id = id;
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        /// <summary>
        /// Gets or sets the unique row id
        /// </summary>
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

    }
}
