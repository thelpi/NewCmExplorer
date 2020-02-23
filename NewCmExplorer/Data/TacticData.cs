using System;
using System.Collections.Generic;

namespace NewCmExplorer.Data
{
    /// <summary>
    /// Represents a tactic.
    /// </summary>
    public class TacticData
    {
        private List<Tuple<PositionData, SideData>> _positions;

        /// <summary>
        /// Name.
        /// </summary>
        public string Name { get; private set; }
        /// <summary>
        /// Positions.
        /// </summary>
        public IReadOnlyCollection<Tuple<PositionData, SideData>> Positions { get { return _positions; } }

        private TacticData(string name)
        {
            Name = name;
            _positions = new List<Tuple<PositionData, SideData>>();
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
                    FourThreeThreeFlat
                };
            }
        }

        #region Shortcuts

        private void AddThreeDefenders(bool sweeper = false)
        {
            _positions.Add(new Tuple<PositionData, SideData>(sweeper ? PositionData.SW : PositionData.D, SideData.C));
            _positions.Add(new Tuple<PositionData, SideData>(PositionData.D, SideData.C));
            _positions.Add(new Tuple<PositionData, SideData>(PositionData.D, SideData.C));
        }

        private void AddFourDefenders()
        {
            _positions.Add(new Tuple<PositionData, SideData>(PositionData.D, SideData.L));
            _positions.Add(new Tuple<PositionData, SideData>(PositionData.D, SideData.R));
            _positions.Add(new Tuple<PositionData, SideData>(PositionData.D, SideData.C));
            _positions.Add(new Tuple<PositionData, SideData>(PositionData.D, SideData.C));
        }

        private void AddFiveDefenders(bool sweeper = false)
        {
            _positions.Add(new Tuple<PositionData, SideData>(PositionData.D, SideData.L));
            _positions.Add(new Tuple<PositionData, SideData>(PositionData.D, SideData.R));
            _positions.Add(new Tuple<PositionData, SideData>(sweeper ? PositionData.SW : PositionData.D, SideData.C));
            _positions.Add(new Tuple<PositionData, SideData>(PositionData.D, SideData.C));
            _positions.Add(new Tuple<PositionData, SideData>(PositionData.D, SideData.C));
        }

        private void AddThreeDefendersPlusFullback(bool sweeper = false, bool offensive = false)
        {
            _positions.Add(new Tuple<PositionData, SideData>(offensive ? PositionData.M : PositionData.DM, SideData.L));
            _positions.Add(new Tuple<PositionData, SideData>(offensive ? PositionData.M : PositionData.DM, SideData.R));
            _positions.Add(new Tuple<PositionData, SideData>(sweeper ? PositionData.SW : PositionData.D, SideData.C));
            _positions.Add(new Tuple<PositionData, SideData>(PositionData.D, SideData.C));
            _positions.Add(new Tuple<PositionData, SideData>(PositionData.D, SideData.C));
        }

        private void AddThreeMidfielders(bool? offensive = null)
        {
            if (!offensive.HasValue)
            {
                _positions.Add(new Tuple<PositionData, SideData>(PositionData.M, SideData.C));
            }
            else if (offensive.Value)
            {
                _positions.Add(new Tuple<PositionData, SideData>(PositionData.OM, SideData.C));
            }
            else
            {
                _positions.Add(new Tuple<PositionData, SideData>(PositionData.DM, SideData.C));
            }
            _positions.Add(new Tuple<PositionData, SideData>(PositionData.M, SideData.C));
            _positions.Add(new Tuple<PositionData, SideData>(PositionData.M, SideData.C));
        }

        private void AddFourMidfielders(bool diamond = false, bool offensive = false)
        {
            if (diamond)
            {
                _positions.Add(new Tuple<PositionData, SideData>(offensive ? PositionData.M : PositionData.DM, SideData.C));
                _positions.Add(new Tuple<PositionData, SideData>(PositionData.M, SideData.C));
                _positions.Add(new Tuple<PositionData, SideData>(PositionData.M, SideData.C));
                _positions.Add(new Tuple<PositionData, SideData>(PositionData.OM, SideData.C));
            }
            else
            {
                _positions.Add(new Tuple<PositionData, SideData>(offensive ? PositionData.OM : PositionData.M, SideData.L));
                _positions.Add(new Tuple<PositionData, SideData>(offensive ? PositionData.OM : PositionData.M, SideData.R));
                _positions.Add(new Tuple<PositionData, SideData>(PositionData.M, SideData.C));
                _positions.Add(new Tuple<PositionData, SideData>(PositionData.M, SideData.C));
            }
        }

        private void AddTwoStrikers()
        {
            _positions.Add(new Tuple<PositionData, SideData>(PositionData.S, SideData.C));
            _positions.Add(new Tuple<PositionData, SideData>(PositionData.S, SideData.C));
        }

        private void AddThreeStrikers()
        {
            _positions.Add(new Tuple<PositionData, SideData>(PositionData.S, SideData.L));
            _positions.Add(new Tuple<PositionData, SideData>(PositionData.S, SideData.R));
            _positions.Add(new Tuple<PositionData, SideData>(PositionData.S, SideData.C));
        }

        #endregion
    }
}
