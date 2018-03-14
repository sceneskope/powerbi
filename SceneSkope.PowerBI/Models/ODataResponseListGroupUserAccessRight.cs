// <auto-generated>
// Code generated by Microsoft (R) AutoRest Code Generator.
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.
// </auto-generated>

namespace SceneSkope.PowerBI.Models
{
    using Newtonsoft.Json;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Odata response wrapper for a Power BI user Access Right for group List
    /// </summary>
    public partial class ODataResponseListGroupUserAccessRight
    {
        /// <summary>
        /// Initializes a new instance of the
        /// ODataResponseListGroupUserAccessRight class.
        /// </summary>
        public ODataResponseListGroupUserAccessRight()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the
        /// ODataResponseListGroupUserAccessRight class.
        /// </summary>
        /// <param name="value">The user Access Right for group List</param>
        public ODataResponseListGroupUserAccessRight(string odatacontext = default(string), IList<GroupUserAccessRight> value = default(IList<GroupUserAccessRight>))
        {
            Odatacontext = odatacontext;
            Value = value;
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "odata.context")]
        public string Odatacontext { get; set; }

        /// <summary>
        /// Gets or sets the user Access Right for group List
        /// </summary>
        [JsonProperty(PropertyName = "value")]
        public IList<GroupUserAccessRight> Value { get; set; }

    }
}
