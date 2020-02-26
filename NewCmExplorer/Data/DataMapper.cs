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
            _attributes = new List<AttributeData>();
            _confederations = new List<ConfederationData>();
            _countries = new List<CountryData>();
        }

        private readonly List<ClubData> _clubs;
        private readonly List<PlayerData> _players;
        private readonly List<AttributeData> _attributes;
        private readonly List<ConfederationData> _confederations;
        private readonly List<CountryData> _countries;

        /// <summary>
        /// Clubs list.
        /// </summary>
        public IReadOnlyCollection<ClubData> Clubs { get { return _clubs; } }
        /// <summary>
        /// Players list.
        /// </summary>
        public IReadOnlyCollection<PlayerData> Players { get { return _players; } }
        /// <summary>
        /// Attributes list.
        /// </summary>
        public IReadOnlyCollection<AttributeData> Attributes { get { return _attributes; } }
        /// <summary>
        /// Confederations list.
        /// </summary>
        public IReadOnlyCollection<ConfederationData> Confederations { get { return _confederations; } }
        /// <summary>
        /// Countries list.
        /// </summary>
        public IReadOnlyCollection<CountryData> Countries { get { return _countries; } }

        /// <summary>
        /// Loads static datas.
        /// </summary>
        internal void LoadStaticDatas()
        {
            SetList(_clubs,
                "club",
                new[] { "ID", "ShortName" },
                (SqlDataReader reader) =>
                {
                    return new ClubData(reader.Get<int>("ID"), reader.Get<string>("ShortName"));
                });
            _clubs.Insert(0, ClubData.NoClub);

            SetList(_attributes,
                "attribute",
                new[] {"ID", "name", "type_ID" },
                (SqlDataReader reader) =>
                {
                    return new AttributeData(reader.Get<int>("ID"), reader.Get<string>("name"),
                        (AttributeTypeData)reader.Get<int>("type_ID"));
                });

            SetList(_confederations,
                "confederation",
                new[] { "ID", "Name3", "Name", "PeopleName", "FedSigle", "FedName", "Strength" },
                (SqlDataReader reader) =>
                {
                    return new ConfederationData(reader.Get<int>("ID"), reader.Get<string>("Name3"),
                        reader.Get<string>("Name"), reader.Get<string>("PeopleName"), reader.Get<string>("FedSigle"),
                        reader.Get<string>("FedName"), reader.Get<decimal>("Strength"));
                });

            SetList(_countries,
                "country",
                new[] { "ID", "Name", "NameShort", "Name3", "ContinentID", "is_EU" },
                (SqlDataReader reader) =>
                {
                    return new CountryData(reader.Get<int>("ID"), reader.Get<string>("Name3"),
                        reader.Get<string>("NameShort"), reader.Get<string>("Name"),
                        GetConfederationById(reader.Get<int?>("ContinentID")), reader.Get<byte>("is_EU") > 0);
                });
        }

        private ConfederationData GetConfederationById(int? confederationId)
        {
            return confederationId.HasValue ? _confederations.Find(c => c.Id == confederationId) : null;
        }

        private void SetList<T>(List<T> instances, string table, string[] columns,
            Func<SqlDataReader, T> readerToDataFunc, string whereStatement = null, params SqlParameter[] parameters)
        {
            instances.Clear();
            using (var conn = new SqlConnection(Settings.Default.connectionString))
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = string.Format("SELECT [{0}] FROM [dbo].[{1}] WHERE {2}", string.Join("], [", columns), table, string.IsNullOrWhiteSpace(whereStatement) ? "1=1" : whereStatement);
                    if (parameters != null)
                    {
                        foreach (SqlParameter param in parameters)
                        {
                            cmd.Parameters.Add(param);
                        }
                    }
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            instances.Add(readerToDataFunc(reader));
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
            SetList(_players,
                "player",
                new[] { "ID", "Firstname", "Lastname", "Commonname", "DateOfBirth", "YearOfBirth", "CurrentAbility",
                    "PotentialAbility", "HomeReputation", "CurrentReputation", "WorldReputation" },
                (SqlDataReader reader) =>
                {
                    return new PlayerData(reader.Get<int>("ID"), reader.Get<string>("Firstname"),
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
                        0);
                },
                "ISNULL([ClubContractID], -1) = @club",
                new SqlParameter("@club", club.Id));
            
            using (var conn = new SqlConnection(Settings.Default.connectionString))
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
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
                    
                    cmd.CommandText = "SELECT [side_ID], [rate] FROM [dbo].[player_side] WHERE [player_ID] = @player";
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
