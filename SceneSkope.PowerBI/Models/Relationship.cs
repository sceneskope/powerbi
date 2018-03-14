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

    public partial class Relationship
    {
        /// <summary>
        /// Initializes a new instance of the Relationship class.
        /// </summary>
        public Relationship()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the Relationship class.
        /// </summary>
        /// <param name="name">Name of the measure</param>
        /// <param name="fromTable">Name of the foreign key table</param>
        /// <param name="fromColumn">Name of the foreign key column</param>
        /// <param name="toTable">Name of the primary key table</param>
        /// <param name="toColumn">Name of the primary key column</param>
        /// <param name="crossFilteringBehavior">The filter direction. Possible
        /// values include: 'OneDirection', 'BothDirections',
        /// 'Automatic'</param>
        public Relationship(string name, string fromTable, string fromColumn, string toTable, string toColumn, string crossFilteringBehavior = default(string))
        {
            Name = name;
            CrossFilteringBehavior = crossFilteringBehavior;
            FromTable = fromTable;
            FromColumn = fromColumn;
            ToTable = toTable;
            ToColumn = toColumn;
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        /// <summary>
        /// Gets or sets name of the measure
        /// </summary>
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the filter direction. Possible values include:
        /// 'OneDirection', 'BothDirections', 'Automatic'
        /// </summary>
        [JsonProperty(PropertyName = "crossFilteringBehavior")]
        public string CrossFilteringBehavior { get; set; }

        /// <summary>
        /// Gets or sets name of the foreign key table
        /// </summary>
        [JsonProperty(PropertyName = "fromTable")]
        public string FromTable { get; set; }

        /// <summary>
        /// Gets or sets name of the foreign key column
        /// </summary>
        [JsonProperty(PropertyName = "fromColumn")]
        public string FromColumn { get; set; }

        /// <summary>
        /// Gets or sets name of the primary key table
        /// </summary>
        [JsonProperty(PropertyName = "toTable")]
        public string ToTable { get; set; }

        /// <summary>
        /// Gets or sets name of the primary key column
        /// </summary>
        [JsonProperty(PropertyName = "toColumn")]
        public string ToColumn { get; set; }

        /// <summary>
        /// Validate the object.
        /// </summary>
        /// <exception cref="ValidationException">
        /// Thrown if validation fails
        /// </exception>
        public virtual void Validate()
        {
            if (Name == null)
            {
                throw new ValidationException(ValidationRules.CannotBeNull, "Name");
            }
            if (FromTable == null)
            {
                throw new ValidationException(ValidationRules.CannotBeNull, "FromTable");
            }
            if (FromColumn == null)
            {
                throw new ValidationException(ValidationRules.CannotBeNull, "FromColumn");
            }
            if (ToTable == null)
            {
                throw new ValidationException(ValidationRules.CannotBeNull, "ToTable");
            }
            if (ToColumn == null)
            {
                throw new ValidationException(ValidationRules.CannotBeNull, "ToColumn");
            }
        }
    }
}
