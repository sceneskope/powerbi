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
    /// Power BI Assign to Capacity Request
    /// </summary>
    public partial class AssignToCapacityRequest
    {
        /// <summary>
        /// Initializes a new instance of the AssignToCapacityRequest class.
        /// </summary>
        public AssignToCapacityRequest()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the AssignToCapacityRequest class.
        /// </summary>
        /// <param name="capacityId">The capacity id</param>
        public AssignToCapacityRequest(string capacityId = default(string))
        {
            CapacityId = capacityId;
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        /// <summary>
        /// Gets or sets the capacity id
        /// </summary>
        [JsonProperty(PropertyName = "capacityId")]
        public string CapacityId { get; set; }

    }
}
