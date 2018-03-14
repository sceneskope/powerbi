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
    /// A Power BI refresh history entry
    /// </summary>
    public partial class Refresh
    {
        /// <summary>
        /// Initializes a new instance of the Refresh class.
        /// </summary>
        public Refresh()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the Refresh class.
        /// </summary>
        public Refresh(string refreshType = default(string), System.DateTime? startTime = default(System.DateTime?), System.DateTime? endTime = default(System.DateTime?), string serviceExceptionJson = default(string), string status = default(string))
        {
            RefreshType = refreshType;
            StartTime = startTime;
            EndTime = endTime;
            ServiceExceptionJson = serviceExceptionJson;
            Status = status;
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "refreshType")]
        public string RefreshType { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "startTime")]
        public System.DateTime? StartTime { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "endTime")]
        public System.DateTime? EndTime { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "serviceExceptionJson")]
        public string ServiceExceptionJson { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "status")]
        public string Status { get; set; }

    }
}
