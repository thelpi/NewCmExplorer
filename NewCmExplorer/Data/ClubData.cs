namespace NewCmExplorer.Data
{
    /// <summary>
    /// Represents a club.
    /// </summary>
    public class ClubData
    {
        /// <summary>
        /// Identifier.
        /// </summary>
        public int Id { get; private set; }

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
        /// Division identifier.
        /// </summary>
        public int? DivisionId { get; private set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="id"><see cref="Id"/></param>
        /// <param name="shortName"><see cref="ShortName"/></param>
        /// <param name="name"><see cref="Name"/></param>
        /// <param name="country"><see cref="Country"/></param>
        /// <param name="reputation"><see cref="Reputation"/></param>
        /// <param name="facilities"><see cref="Facilities"/></param>
        /// <param name="divisionId"><see cref="DivisionId"/></param>
        /// <param name="bank"><see cref="Bank"/></param>
        internal ClubData(int id, string shortName, string name, CountryData country,
            int reputation, int facilities, int? divisionId, int bank)
        {
            Id = id;
            ShortName = shortName;
            Name = name;
            Country = country;
            Reputation = reputation;
            Facilities = facilities;
            DivisionId = divisionId;
            Bank = bank;
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return ShortName;
        }

        private static ClubData _noClub;

        /// <summary>
        /// Represents the virtual club related to unemployment.
        /// </summary>
        public static ClubData NoClub
        {
            get
            {
                if (_noClub == null)
                {
                    _noClub = new ClubData(Constants.NoClubId, Constants.NoClubName,
                        Constants.NoClubName, null, 0, 0, null, 0);
                }
                return _noClub;
            }
        }
    }
}
