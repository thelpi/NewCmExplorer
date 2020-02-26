using System.Collections.Generic;

namespace NewCmExplorer.Data
{
    /// <summary>
    /// Represents an attribute.
    /// </summary>
    /// <seealso cref="BaseData"/>
    public class AttributeData : BaseData
    {
        /// <summary>
        /// Name.
        /// </summary>
        public string Name { get; private set; }
        /// <summary>
        /// Attribute type.
        /// </summary>
        public AttributeTypeData AttributeType { get; private set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="id"><see cref="BaseData.Id"/></param>
        /// <param name="name"><see cref="Name"/></param>
        /// <param name="attributeType"><see cref="AttributeType"/></param>
        internal AttributeData(int id, string name, AttributeTypeData attributeType) : base(id)
        {
            Name = name;
            AttributeType = attributeType;
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return Name;
        }

        /// <summary>
        /// Gets every <see cref="AttributeData"/> instances.
        /// </summary>
        public static IReadOnlyCollection<AttributeData> Instances
        {
            get
            {
                return GetInstancesOfType<AttributeData>();
            }
        }

        /// <summary>
        /// Finds an instance by its identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>The instance; <c>Null</c> if not found.</returns>
        public static AttributeData GetByid(int? id)
        {
            return GetByid<AttributeData>(id);
        }
    }
}
