using System;
using System.Collections.Generic;
using System.Linq;

namespace NewCmExplorer.Data
{
    /// <summary>
    /// Represents a player.
    /// </summary>
    public class PlayerData
    {
        // Source date of birth.
        private readonly DateTime? _dateOfBirth;
        // Source year of birth.
        private readonly int? _yearOfBirth;
        // Positions.
        private readonly Dictionary<PositionData, int> _positions;
        // Sides.
        private readonly Dictionary<SideData, int> _sides;

        /// <summary>
        /// Identifier.
        /// </summary>
        public int Id { get; private set; }
        /// <summary>
        /// First name.
        /// </summary>
        public string FirstName { get; private set; }
        /// <summary>
        /// Last name.
        /// </summary>
        public string LastName { get; private set; }
        /// <summary>
        /// Common name.
        /// </summary>
        public string CommonName { get; private set; }
        /// <summary>
        /// Current ability.
        /// </summary>
        public int CurrentAbility { get; private set; }
        /// <summary>
        /// Potential ability.
        /// </summary>
        public int PotentialAbility { get; private set; }
        /// <summary>
        /// Home reputation.
        /// </summary>
        public int HomeReputation { get; private set; }
        /// <summary>
        /// Current reputation.
        /// </summary>
        public int CurrentReputation { get; private set; }
        /// <summary>
        /// World reputation.
        /// </summary>
        public int WorldReputation { get; private set; }

        /// <summary>
        /// Inferred; full name.
        /// </summary>
        public string FullName
        {
            get
            {
                return string.IsNullOrWhiteSpace(CommonName) ? string.Concat(LastName, ", ", FirstName) : CommonName;
            }
        }
        /// <summary>
        /// Inferred; displayable date of birth.
        /// </summary>
        public string DateOfBirth
        {
            get
            {
                if (_dateOfBirth.HasValue)
                {
                    return _dateOfBirth.Value.ToString(Constants.DATE_PATTERN);
                }

                if (_yearOfBirth.HasValue)
                {
                    return _yearOfBirth.Value.ToString();
                }

                return Constants.UNKNOWN_DATE;
            }
        }
        /// <summary>
        /// Inferred (at some point); sum of rates for each attribute.
        /// </summary>
        public int GlobalRate { get; private set; }

        /// <summary>
        /// Positions with rate collection.
        /// </summary>
        public IReadOnlyDictionary<PositionData, int> Positions { get { return _positions; } }
        /// <summary>
        /// Sides with rate collection.
        /// </summary>
        public IReadOnlyDictionary<SideData, int> Sides { get { return _sides; } }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="id"><see cref="Id"/></param>
        /// <param name="firstName"><see cref="FirstName"/></param>
        /// <param name="lastName"><see cref="LastName"/></param>
        /// <param name="commonName"><see cref="CommonName"/></param>
        /// <param name="dateOfBirth"><see cref="_dateOfBirth"/></param>
        /// <param name="yearOfBirth"><see cref="_yearOfBirth"/></param>
        /// <param name="currentAbility"><see cref="CurrentAbility"/></param>
        /// <param name="currentReputation"><see cref="CurrentReputation"/></param>
        /// <param name="globalRate"><see cref="GlobalRate"/> (temporarly).</param>
        /// <param name="homeReputation"><see cref="HomeReputation"/></param>
        /// <param name="potentialAbility"><see cref="PotentialAbility"/></param>
        /// <param name="worldReputation"><see cref="WorldReputation"/></param>
        internal PlayerData(int id, string firstName, string lastName, string commonName, DateTime? dateOfBirth, int? yearOfBirth,
            int currentAbility, int potentialAbility, int homeReputation, int currentReputation, int worldReputation, int globalRate)
        {
            Id = id;
            FirstName = firstName;
            LastName = lastName;
            CommonName = commonName;
            _dateOfBirth = dateOfBirth;
            _yearOfBirth = yearOfBirth;
            CurrentAbility = currentAbility;
            PotentialAbility = potentialAbility;
            HomeReputation = homeReputation;
            CurrentReputation = currentReputation;
            WorldReputation = worldReputation;

            _positions = Enum.GetValues(typeof(PositionData)).OfType<PositionData>().ToDictionary(p => p, p => 1);
            _sides = Enum.GetValues(typeof(SideData)).OfType<SideData>().ToDictionary(s => s, p => 1);

            // Temporary.
            GlobalRate = globalRate;
        }

        /// <summary>
        /// Sets a <see cref="PositionData"/>.
        /// </summary>
        /// <param name="position">The position.</param>
        /// <param name="rate">The rate.</param>
        internal void SetPosition(PositionData position, int rate)
        {
            _positions[position] = rate;
        }

        /// <summary>
        /// Sets a <see cref="SideData"/>.
        /// </summary>
        /// <param name="side">The side.</param>
        /// <param name="rate">The rate.</param>
        internal void SetSide(SideData side, int rate)
        {
            _sides[side] = rate;
        }

        /// <summary>
        /// Adjusts position and side rates.
        /// </summary>
        internal void AdjustPositionAndSide()
        {
            if (_positions[PositionData.M] == 0)
            {
                if (_positions[PositionData.OM] != 0 && _positions[PositionData.DM] != 0)
                {
                    _positions[PositionData.M] = _positions[PositionData.OM] > _positions[PositionData.DM] ?
                        _positions[PositionData.OM] : _positions[PositionData.DM];
                }
                else if (_positions[PositionData.OM] != 0)
                {
                    _positions[PositionData.M] = _positions[PositionData.OM];
                }
                else if (_positions[PositionData.DM] != 0)
                {
                    _positions[PositionData.M] = _positions[PositionData.DM];
                }
            }

            for (int i = 0; i < _positions.Count; i++)
            {
                if (_positions[_positions.Keys.ElementAt(i)] == 0)
                {
                    _positions[_positions.Keys.ElementAt(i)] = 1;
                }
            }

            for (int i = 0; i < _sides.Count; i++)
            {
                if (_sides[_sides.Keys.ElementAt(i)] == 0)
                {
                    _sides[_sides.Keys.ElementAt(i)] = 1;
                }
            }
        }

        public override string ToString()
        {
            return FullName;
        }
    }
}
