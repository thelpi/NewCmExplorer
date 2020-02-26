namespace NewCmExplorer.Data
{
    /// <summary>
    /// Represents a confederation.
    /// </summary>
    public class ConfederationData
    {
        /// <summary>
        /// Identifier.
        /// </summary>
        public int Id { get; private set; }
        /// <summary>
        /// Continent code.
        /// </summary>
        public string ContCode { get; private set; }
        /// <summary>
        /// Continent name.
        /// </summary>
        public string ContName { get; private set; }
        /// <summary>
        /// Continent demonym.
        /// </summary>
        public string ContDemonym { get; private set; }
        /// <summary>
        /// Code / sigle of the federation.
        /// </summary>
        public string FedCode { get; private set; }
        /// <summary>
        /// Name of the federation.
        /// </summary>
        public string FedName { get; private set; }
        /// <summary>
        /// Football strength (from <c>0</c> the lowest, to <c>1</c> the highest).
        /// </summary>
        public decimal Strength { get; private set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="id"><see cref="Id"/></param>
        /// <param name="contCode"><see cref="ContCode"/></param>
        /// <param name="contName"><see cref="ContName"/></param>
        /// <param name="contDemonym"><see cref="ContDemonym"/></param>
        /// <param name="fedCode"><see cref="FedCode"/></param>
        /// <param name="fedName"><see cref="FedName"/></param>
        /// <param name="strength"><see cref="Strength"/></param>
        internal ConfederationData(int id, string contCode, string contName, string contDemonym,
            string fedCode, string fedName, decimal strength)
        {
            Id = id;
            ContCode = contCode;
            ContName = contName;
            ContDemonym = contDemonym;
            FedCode = fedCode;
            FedName = fedName;
            Strength = strength;
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return ContName;
        }
    }
}
