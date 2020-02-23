using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using NewCmExplorer.Properties;

namespace NewCmExplorer.Data
{
    /// <summary>
    /// Data mapper.
    /// </summary>
    internal class DataMapper
    {
        private static DataMapper _instance;

        /// <summary>
        /// Unique instance.
        /// </summary>
        internal static DataMapper Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new DataMapper();
                }
                return _instance;
            }
        }

        private DataMapper()
        {
            _clubs = new List<ClubData>();
            _players = new List<PlayerData>();
        }

        private readonly List<ClubData> _clubs;
        private readonly List<PlayerData> _players;

        /// <summary>
        /// Clubs list.
        /// </summary>
        public IReadOnlyCollection<ClubData> Clubs { get { return _clubs; } }
        /// <summary>
        /// Players list.
        /// </summary>
        public IReadOnlyCollection<PlayerData> Players { get { return _players; } }

        /// <summary>
        /// Loads static datas.
        /// </summary>
        internal void LoadStaticDatas()
        {
            _clubs.Clear();
            _clubs.Add(ClubData.NoClub);
            using (var conn = new SqlConnection(Settings.Default.connectionString))
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT [ID], [ShortName] FROM [dbo].[club]";
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            _clubs.Add(new ClubData(reader.Get<int>("ID"),
                                reader.Get<string>("ShortName")));
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Loads players from the specified club.
        /// </summary>
        /// <param name="club">Club.</param>
        internal void LoadPlayers(ClubData club)
        {
            _players.Clear();
            using (var conn = new SqlConnection(Settings.Default.connectionString))
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    string sql = "SELECT [ID], [Firstname], [Lastname], [Commonname], [DateOfBirth], [YearOfBirth]";
                    sql += ", [CurrentAbility], [PotentialAbility], [HomeReputation], [CurrentReputation], [WorldReputation]";
                    sql += ", (SELECT SUM([rate]) FROM [dbo].[player_attribute] WHERE [player_ID] = [ID]) AS [GlobalRate]";
                    sql += " FROM [dbo].[player] WHERE ISNULL([ClubContractID], -1) = @club";

                    cmd.CommandText = sql;
                    cmd.Parameters.Add(new SqlParameter("@club", club.Id));
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var player = new PlayerData(reader.Get<int>("ID"), reader.Get<string>("Firstname"),
                                reader.Get<string>("Lastname"), reader.Get<string>("Commonname"),
                                reader.Get<DateTime?>("DateOfBirth"), reader.Get<int?>("YearOfBirth"),
                                reader.Get<int>("CurrentAbility", new Tuple<int, int>(0, 100)),
                                reader.Get<int>("PotentialAbility",
                                    new Tuple<int, int>(0, 100),
                                    new Tuple<int, int>(-1, 140),
                                    new Tuple<int, int>(-2, 180)),
                                reader.Get<int>("HomeReputation", new Tuple<int, int>(0, 100)),
                                reader.Get<int>("CurrentReputation", new Tuple<int, int>(0, 100)),
                                reader.Get<int>("WorldReputation", new Tuple<int, int>(0, 100)),
                                reader.Get<int>("GlobalRate"));

                            _players.Add(player);
                        }
                    }

                    cmd.Parameters.Clear();
                    cmd.CommandText = "SELECT [position_ID], [rate] FROM [dbo].[player_position] WHERE [player_ID] = @player";
                    cmd.Parameters.Add("@player", System.Data.SqlDbType.Int);
                    cmd.Prepare();
                    foreach (PlayerData p in _players)
                    {
                        cmd.Parameters["@player"].Value = p.Id;
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                p.SetPosition((PositionData)reader.Get<int>("position_ID"), reader.Get<int>("rate"));
                            }
                        }
                    }

                    cmd.Parameters.Clear();
                    cmd.CommandText = "SELECT [side_ID], [rate] FROM [dbo].[player_side] WHERE [player_ID] = @player";
                    cmd.Parameters.Add("@player", System.Data.SqlDbType.Int);
                    cmd.Prepare();
                    foreach (PlayerData p in _players)
                    {
                        cmd.Parameters["@player"].Value = p.Id;
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                p.SetSide((SideData)reader.Get<int>("side_ID"), reader.Get<int>("rate"));
                            }
                        }
                    }
                }
            }

            foreach (PlayerData p in _players)
            {
                p.AdjustPositionAndSide();
            }
        }
    }
}
