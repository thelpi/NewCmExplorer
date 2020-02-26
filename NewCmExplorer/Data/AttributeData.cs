namespace NewCmExplorer.Data
{
    /// <summary>
    /// Represents an attribute.
    /// </summary>
    public class AttributeData
    {
        /// <summary>
        /// Identifier.
        /// </summary>
        public int Id { get; private set; }
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
        /// <param name="id"><see cref="Id"/></param>
        /// <param name="name"><see cref="Name"/></param>
        /// <param name="attributeType"><see cref="AttributeType"/></param>
        internal AttributeData(int id, string name, AttributeTypeData attributeType)
        {
            Id = id;
            Name = name;
            AttributeType = attributeType;
        }
    }
}
