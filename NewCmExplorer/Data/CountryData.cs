namespace NewCmExplorer.Data
{
    /// <summary>
    /// Represents a country.
    /// </summary>
    public class CountryData
    {
        /// <summary>
        /// Identifier.
        /// </summary>
        public int Id { get; private set; }
        /// <summary>
        /// Code.
        /// </summary>
        public string Code { get; private set; }
        /// <summary>
        /// Short name.
        /// </summary>
        public string ShortName { get; private set; }
        /// <summary>
        /// Name.
        /// </summary>
        public string Name { get; private set; }
        /// <summary>
        /// Confederation.
        /// </summary>
        public ConfederationData Confederation { get; private set; }
        /// <summary>
        /// Is european union y/n.
        /// </summary>
        public bool IsEuropeanUnion { get; private set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="id"><see cref="Id"/></param>
        /// <param name="code"><see cref="Code"/></param>
        /// <param name="shortName"><see cref="ShortName"/></param>
        /// <param name="name"><see cref="Name"/></param>
        /// <param name="confederation"><see cref="Confederation"/></param>
        /// <param name="isEu"><see cref="IsEuropeanUnion"/></param>
        internal CountryData(int id, string code, string shortName, string name, ConfederationData confederation, bool isEu)
        {
            Id = id;
            Code = code;
            ShortName = shortName;
            Name = name;
            Confederation = confederation;
            IsEuropeanUnion = isEu;
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return ShortName;
        }
    }
}
