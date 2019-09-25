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
    /// Power BI rebind report request
    /// </summary>
    public partial class RebindReportRequest
    {
        /// <summary>
        /// Initializes a new instance of the RebindReportRequest class.
        /// </summary>
        public RebindReportRequest()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the RebindReportRequest class.
        /// </summary>
        /// <param name="datasetId">The new dataset of the rebinded
        /// report</param>
        public RebindReportRequest(string datasetId)
        {
            DatasetId = datasetId;
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        /// <summary>
        /// Gets or sets the new dataset of the rebinded report
        /// </summary>
        [JsonProperty(PropertyName = "datasetId")]
        public string DatasetId { get; set; }

        /// <summary>
        /// Validate the object.
        /// </summary>
        /// <exception cref="ValidationException">
        /// Thrown if validation fails
        /// </exception>
        public virtual void Validate()
        {
            if (DatasetId == null)
            {
                throw new ValidationException(ValidationRules.CannotBeNull, "DatasetId");
            }
        }
    }
}
