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
    /// A dataset column
    /// </summary>
    public partial class Column
    {
        /// <summary>
        /// Initializes a new instance of the Column class.
        /// </summary>
        public Column()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the Column class.
        /// </summary>
        /// <param name="name">The column name</param>
        /// <param name="dataType">The column data type</param>
        public Column(string name, string dataType)
        {
            Name = name;
            DataType = dataType;
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        /// <summary>
        /// Gets or sets the column name
        /// </summary>
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the column data type
        /// </summary>
        [JsonProperty(PropertyName = "dataType")]
        public string DataType { get; set; }

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
            if (DataType == null)
            {
                throw new ValidationException(ValidationRules.CannotBeNull, "DataType");
            }
            if (Name != null)
            {
                if (!System.Text.RegularExpressions.Regex.IsMatch(Name, "^[\\x09\\x0A\\x0D\\x20-\\uD7FF\\uE000-\\uFFFD\\u10000-\\u10FFFF]+$"))
                {
                    throw new ValidationException(ValidationRules.Pattern, "Name", "^[\\x09\\x0A\\x0D\\x20-\\uD7FF\\uE000-\\uFFFD\\u10000-\\u10FFFF]+$");
                }
            }
        }
    }
}
