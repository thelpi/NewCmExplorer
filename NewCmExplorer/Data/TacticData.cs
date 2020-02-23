using System.Collections.Generic;

namespace NewCmExplorer.Data
{
    /// <summary>
    /// Represents a tactic.
    /// </summary>
    public class TacticData
    {
        private List<KeyValuePair<PositionData, SideData>> _positions;

        /// <summary>
        /// Name.
        /// </summary>
        public string Name { get; private set; }
        /// <summary>
        /// Positions.
        /// </summary>
        public IReadOnlyCollection<KeyValuePair<PositionData, SideData>> Positions { get { return _positions; } }

        private TacticData(string name)
        {
            Name = name;
            _positions = new List<KeyValuePair<PositionData, SideData>>();
        }

        public override string ToString()
        {
            return Name;
        }

        /// <summary>
        /// Four four two flat.
        /// </summary>
        public static TacticData FourFourTwoFlat
        {
            get
            {
                var t = new TacticData(nameof(FourFourTwoFlat));
                t.AddFourDefenders();
                t.AddFourMidfielders();
                t.AddTwoStrikers();
                return t;
            }
        }

        /// <summary>
        /// Four four two dimaond.
        /// </summary>
        public static TacticData FourFourTwoDiamond
        {
            get
            {
                var t = new TacticData(nameof(FourFourTwoDiamond));
                t.AddFourDefenders();
                t.AddFourMidfielders(diamond: true);
                t.AddTwoStrikers();
                return t;
            }
        }

        /// <summary>
        /// Four three three flat.
        /// </summary>
        public static TacticData FourThreeThreeFlat
        {
            get
            {
                var t = new TacticData(nameof(FourThreeThreeFlat));
                t.AddFourDefenders();
                t.AddThreeMidfielders();
                t.AddThreeStrikers();
                return t;
            }
        }

        /// <summary>
        /// Five three two defensive.
        /// </summary>
        public static TacticData FiveThreeTwoDefensive
        {
            get
            {
                var t = new TacticData(nameof(FiveThreeTwoDefensive));
                t.AddFiveDefenders(sweeper: true);
                t.AddThreeMidfielders(offensive: false);
                t.AddTwoStrikers();
                return t;
            }
        }

        /// <summary>
        /// Five three two offensive.
        /// </summary>
        public static TacticData FiveThreeTwoOffensive
        {
            get
            {
                var t = new TacticData(nameof(FiveThreeTwoOffensive));
                t.AddFiveDefenders(sweeper: true);
                t.AddThreeMidfielders(offensive: true);
                t.AddTwoStrikers();
                return t;
            }
        }

        /// <summary>
        /// Four five one.
        /// </summary>
        public static TacticData FourFiveOne
        {
            get
            {
                var t = new TacticData(nameof(FourFiveOne));
                t.AddFourDefenders();
                t.AddFiveMidfielders(offensive: true);
                t._positions.Add(new KeyValuePair<PositionData, SideData>(PositionData.S, SideData.C));
                return t;
            }
        }

        /// <summary>
        /// Collection of default tactics.
        /// </summary>
        public static IReadOnlyCollection<TacticData> DefaultTactics
        {
            get
            {
                return new List<TacticData>
                {
                    FourFourTwoFlat,
                    FourFourTwoDiamond,
                    FourThreeThreeFlat,
                    FiveThreeTwoDefensive,
                    FiveThreeTwoOffensive,
                    FourFiveOne
                };
            }
        }

        #region Shortcuts

        private void AddThreeDefenders(bool sweeper = false)
        {
            _positions.Add(new KeyValuePair<PositionData, SideData>(sweeper ? PositionData.SW : PositionData.D, SideData.C));
            _positions.Add(new KeyValuePair<PositionData, SideData>(PositionData.D, SideData.C));
            _positions.Add(new KeyValuePair<PositionData, SideData>(PositionData.D, SideData.C));
        }

        private void AddFourDefenders()
        {
            _positions.Add(new KeyValuePair<PositionData, SideData>(PositionData.D, SideData.L));
            _positions.Add(new KeyValuePair<PositionData, SideData>(PositionData.D, SideData.R));
            _positions.Add(new KeyValuePair<PositionData, SideData>(PositionData.D, SideData.C));
            _positions.Add(new KeyValuePair<PositionData, SideData>(PositionData.D, SideData.C));
        }

        private void AddFiveDefenders(bool sweeper = false)
        {
            _positions.Add(new KeyValuePair<PositionData, SideData>(PositionData.D, SideData.L));
            _positions.Add(new KeyValuePair<PositionData, SideData>(PositionData.D, SideData.R));
            _positions.Add(new KeyValuePair<PositionData, SideData>(sweeper ? PositionData.SW : PositionData.D, SideData.C));
            _positions.Add(new KeyValuePair<PositionData, SideData>(PositionData.D, SideData.C));
            _positions.Add(new KeyValuePair<PositionData, SideData>(PositionData.D, SideData.C));
        }

        private void AddThreeDefendersPlusFullback(bool sweeper = false, bool offensive = false)
        {
            _positions.Add(new KeyValuePair<PositionData, SideData>(offensive ? PositionData.M : PositionData.DM, SideData.L));
            _positions.Add(new KeyValuePair<PositionData, SideData>(offensive ? PositionData.M : PositionData.DM, SideData.R));
            _positions.Add(new KeyValuePair<PositionData, SideData>(sweeper ? PositionData.SW : PositionData.D, SideData.C));
            _positions.Add(new KeyValuePair<PositionData, SideData>(PositionData.D, SideData.C));
            _positions.Add(new KeyValuePair<PositionData, SideData>(PositionData.D, SideData.C));
        }

        private void AddThreeMidfielders(bool? offensive = null)
        {
            if (!offensive.HasValue)
            {
                _positions.Add(new KeyValuePair<PositionData, SideData>(PositionData.M, SideData.C));
            }
            else if (offensive.Value)
            {
                _positions.Add(new KeyValuePair<PositionData, SideData>(PositionData.OM, SideData.C));
            }
            else
            {
                _positions.Add(new KeyValuePair<PositionData, SideData>(PositionData.DM, SideData.C));
            }
            _positions.Add(new KeyValuePair<PositionData, SideData>(PositionData.M, SideData.C));
            _positions.Add(new KeyValuePair<PositionData, SideData>(PositionData.M, SideData.C));
        }

        private void AddFourMidfielders(bool diamond = false, bool offensive = false)
        {
            if (diamond)
            {
                _positions.Add(new KeyValuePair<PositionData, SideData>(offensive ? PositionData.M : PositionData.DM, SideData.C));
                _positions.Add(new KeyValuePair<PositionData, SideData>(PositionData.M, SideData.C));
                _positions.Add(new KeyValuePair<PositionData, SideData>(PositionData.M, SideData.C));
                _positions.Add(new KeyValuePair<PositionData, SideData>(PositionData.OM, SideData.C));
            }
            else
            {
                _positions.Add(new KeyValuePair<PositionData, SideData>(offensive ? PositionData.OM : PositionData.M, SideData.L));
                _positions.Add(new KeyValuePair<PositionData, SideData>(offensive ? PositionData.OM : PositionData.M, SideData.R));
                _positions.Add(new KeyValuePair<PositionData, SideData>(PositionData.M, SideData.C));
                _positions.Add(new KeyValuePair<PositionData, SideData>(PositionData.M, SideData.C));
            }
        }

        private void AddFiveMidfielders(bool eighties = false, bool offensive = false)
        {
            if (eighties)
            {
                _positions.Add(new KeyValuePair<PositionData, SideData>(PositionData.DM, SideData.C));
                _positions.Add(new KeyValuePair<PositionData, SideData>(PositionData.M, SideData.C));
                _positions.Add(new KeyValuePair<PositionData, SideData>(PositionData.M, SideData.C));
            }
            else
            {
                _positions.Add(new KeyValuePair<PositionData, SideData>(offensive ? PositionData.M : PositionData.DM, SideData.C));
                _positions.Add(new KeyValuePair<PositionData, SideData>(offensive ? PositionData.M : PositionData.DM, SideData.C));
                _positions.Add(new KeyValuePair<PositionData, SideData>(PositionData.OM, SideData.C));
            }
            _positions.Add(new KeyValuePair<PositionData, SideData>(PositionData.OM, SideData.L));
            _positions.Add(new KeyValuePair<PositionData, SideData>(PositionData.OM, SideData.R));
        }

        private void AddTwoStrikers()
        {
            _positions.Add(new KeyValuePair<PositionData, SideData>(PositionData.S, SideData.C));
            _positions.Add(new KeyValuePair<PositionData, SideData>(PositionData.S, SideData.C));
        }

        private void AddThreeStrikers()
        {
            _positions.Add(new KeyValuePair<PositionData, SideData>(PositionData.S, SideData.L));
            _positions.Add(new KeyValuePair<PositionData, SideData>(PositionData.S, SideData.R));
            _positions.Add(new KeyValuePair<PositionData, SideData>(PositionData.S, SideData.C));
        }

        #endregion
    }
}
