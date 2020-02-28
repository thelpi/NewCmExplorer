using System.Collections.Generic;
using System.Linq;

namespace NewCmExplorer.Data
{
    /// <summary>
    /// Represents a club.
    /// </summary>
    /// <seealso cref="BaseData"/>
    public class ClubData : BaseData
    {
        /// <summary>
        /// Short name.
        /// </summary>
        public string ShortName { get; private set; }

        /// <summary>
        /// Name.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Country.
        /// </summary>
        public CountryData Country { get; private set; }

        /// <summary>
        /// Facilities.
        /// </summary>
        public int Facilities { get; private set; }

        /// <summary>
        /// Reputation.
        /// </summary>
        public int Reputation { get; private set; }

        /// <summary>
        /// Bank.
        /// </summary>
        public int Bank { get; private set; }

        /// <summary>
        /// Division.
        /// </summary>
        public ClubCompetitionData Division { get; private set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="id"><see cref="BaseData.Id"/></param>
        /// <param name="shortName"><see cref="ShortName"/></param>
        /// <param name="name"><see cref="Name"/></param>
        /// <param name="country"><see cref="Country"/></param>
        /// <param name="reputation"><see cref="Reputation"/></param>
        /// <param name="facilities"><see cref="Facilities"/></param>
        /// <param name="division"><see cref="Division"/></param>
        /// <param name="bank"><see cref="Bank"/></param>
        internal ClubData(int id, string shortName, string name, CountryData country,
            int reputation, int facilities, ClubCompetitionData division, int bank) : base(id)
        {
            ShortName = shortName;
            Name = name;
            Country = country;
            Reputation = reputation;
            Facilities = facilities;
            Division = division;
            Bank = bank;
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return ShortName;
        }

        /// <summary>
        /// Gets (and sets if not create yet) the virtual club for unemployment.
        /// </summary>
        public static ClubData NoClub
        {
            get
            {
                ClubData noClub = Instances.FirstOrDefault(c => c.Id == Constants.NoClubId);
                if (noClub == null)
                {
                    noClub = new ClubData(Constants.NoClubId, Constants.NoClubName,
                        Constants.NoClubName, null, 0, 0, null, 0);
                }
                return noClub;
            }
        }

        /// <summary>
        /// Gets every <see cref="ClubData"/> instances.
        /// </summary>
        public static IReadOnlyCollection<ClubData> Instances
        {
            get
            {
                return GetInstancesOfType<ClubData>();
            }
        }

        /// <summary>
        /// Finds an instance by its identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>The instance; <see cref="NoClub"/> if not found.</returns>
        public static ClubData GetByid(int? id)
        {
            return GetByid<ClubData>(id) ?? NoClub;
        }
    }
}
