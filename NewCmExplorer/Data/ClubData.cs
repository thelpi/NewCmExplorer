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
        /// Constructor.
        /// </summary>
        /// <param name="id"><see cref="Id"/></param>
        /// <param name="shortName"><see cref="ShortName"/></param>
        internal ClubData(int id, string shortName)
        {
            Id = id;
            ShortName = shortName;
        }

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
                    _noClub = new ClubData(-1, "No club");
                }
                return _noClub;
            }
        }
    }
}
