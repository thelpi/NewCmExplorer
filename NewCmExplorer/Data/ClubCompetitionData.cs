using System.Collections.Generic;

namespace NewCmExplorer.Data
{
    /// <summary>
    /// Represents a club competition.
    /// </summary>
    /// <seealso cref="BaseData"/>
    public class ClubCompetitionData : BaseData
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
        /// Reputation (rate from <c>1</c> to <c>20</c>; <c>0</c> if unknown).
        /// </summary>
        public int Reputation { get; private set; }
        /// <summary>
        /// Confederation.
        /// </summary>
        public ConfederationData Confederation { get; private set; }
        /// <summary>
        /// Country.
        /// </summary>
        public CountryData Country { get; private set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="id"><see cref="BaseData.Id"/></param>
        /// <param name="code"><see cref="Code"/></param>
        /// <param name="shortName"><see cref="ShortName"/></param>
        /// <param name="reputation"><see cref="Reputation"/></param>
        /// <param name="confederation"><see cref="Confederation"/></param>
        /// <param name="country"><see cref="Country"/></param>
        internal ClubCompetitionData(int id, string code, string shortName, int reputation,
            ConfederationData confederation, CountryData country) : base(id)
        {
            Code = code;
            ShortName = shortName;
            Reputation = reputation;
            Confederation = confederation;
            Country = country;
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return ShortName;
        }

        /// <summary>
        /// Gets every <see cref="ClubCompetitionData"/> instances.
        /// </summary>
        public static IReadOnlyCollection<ClubCompetitionData> Instances
        {
            get
            {
                return GetInstancesOfType<ClubCompetitionData>();
            }
        }

        /// <summary>
        /// Finds an instance by its identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>The instance; <c>Null</c> if not found.</returns>
        public static ClubCompetitionData GetByid(int? id)
        {
            return GetByid<ClubCompetitionData>(id);
        }
    }
}
