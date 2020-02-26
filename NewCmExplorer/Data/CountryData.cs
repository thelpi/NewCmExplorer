using System.Collections.Generic;

namespace NewCmExplorer.Data
{
    /// <summary>
    /// Represents a country.
    /// </summary>
    /// <seealso cref="BaseData"/>
    public class CountryData : BaseData
    {
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
        /// <param name="id"><see cref="BaseData.Id"/></param>
        /// <param name="code"><see cref="Code"/></param>
        /// <param name="shortName"><see cref="ShortName"/></param>
        /// <param name="name"><see cref="Name"/></param>
        /// <param name="confederation"><see cref="Confederation"/></param>
        /// <param name="isEu"><see cref="IsEuropeanUnion"/></param>
        internal CountryData(int id, string code, string shortName, string name,
            ConfederationData confederation, bool isEu) : base(id)
        {
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

        /// <summary>
        /// Gets every <see cref="CountryData"/> instances.
        /// </summary>
        public static IReadOnlyCollection<CountryData> Instances
        {
            get
            {
                return GetInstancesOfType<CountryData>();
            }
        }

        /// <summary>
        /// Finds an instance by its identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>The instance; <c>Null</c> if not found.</returns>
        public static CountryData GetByid(int? id)
        {
            return GetByid<CountryData>(id);
        }
    }
}
