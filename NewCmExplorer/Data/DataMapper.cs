using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
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

        private const char SEQUENCE_SEPARATOR = ';';

        private List<HashSet<HashSet<int>>> _parallelSequences = new List<HashSet<HashSet<int>>>();

        private DataMapper()
        {
            ParseSequencesFileToHashset();
        }

        /// <summary>
        /// Loads static datas.
        /// </summary>
        internal void LoadStaticDatas()
        {
            SetList("attribute",
                new[] {"ID", "name", "type_ID" },
                (SqlDataReader reader) =>
                {
                    return new AttributeData(reader.Get<int>("ID"),
                        reader.Get<string>("name"),
                        (AttributeTypeData)reader.Get<int>("type_ID"));
                });

            SetList("confederation",
                new[] { "ID", "Name3", "Name", "PeopleName", "FedSigle", "FedName", "Strength" },
                (SqlDataReader reader) =>
                {
                    return new ConfederationData(reader.Get<int>("ID"),
                        reader.Get<string>("Name3"),
                        reader.Get<string>("Name"),
                        reader.Get<string>("PeopleName"),
                        reader.Get<string>("FedSigle"),
                        reader.Get<string>("FedName"),
                        reader.Get<decimal>("Strength"));
                });

            SetList("country",
                new[] { "ID", "Name", "NameShort", "Name3", "ContinentID", "is_EU" },
                (SqlDataReader reader) =>
                {
                    return new CountryData(reader.Get<int>("ID"),
                        reader.Get<string>("Name3"),
                        reader.Get<string>("NameShort"),
                        reader.Get<string>("Name"),
                        ConfederationData.GetByid(reader.Get<int?>("ContinentID")),
                        reader.Get<byte>("is_EU") > 0);
                });

            SetList("club",
                new[] { "ID", "ShortName", "LongName", "NationID", "Reputation", "Facilities", "DivisionID", "Bank" },
                (SqlDataReader reader) =>
                {
                    return new ClubData(reader.Get<int>("ID"),
                        reader.Get<string>("ShortName"),
                        reader.Get<string>("LongName"),
                        CountryData.GetByid(reader.Get<int?>("NationID")),
                        reader.Get<int>("Reputation"),
                        reader.Get<int>("Facilities"),
                        reader.Get<int?>("DivisionID"),
                        reader.Get<int>("Bank"));
                });

            // Forces the "NoClub" instance creation.
            ClubData.GetByid(-1);
        }

        private void SetList<T>(string table, string[] columns, Func<SqlDataReader, T> readerToDataFunc,
            string whereStatement = null, params SqlParameter[] parameters) where T : BaseData
        {
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
                            readerToDataFunc(reader);
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
            PlayerData.ClearInstances();
            SetList("player",
                new[] { "ID", "Firstname", "Lastname", "Commonname", "DateOfBirth", "YearOfBirth", "CurrentAbility",
                    "PotentialAbility", "HomeReputation", "CurrentReputation", "WorldReputation",
                    "RightFoot", "LeftFoot", "NationID1", "NationID2", "ClubContractID", "DateContractStart",
                    "DateContractEnd", "Value", "Wage", "Caps", "IntGoals" },
                (SqlDataReader reader) =>
                {
                    return new PlayerData(reader.Get<int>("ID"),
                        reader.Get<string>("Firstname"),
                        reader.Get<string>("Lastname"),
                        reader.Get<string>("Commonname"),
                        reader.Get<DateTime?>("DateOfBirth"),
                        reader.Get<int?>("YearOfBirth"),
                        reader.Get<int>("CurrentAbility", new Tuple<int, int>(0, 100)),
                        reader.Get<int>("PotentialAbility",
                            new Tuple<int, int>(0, 100),
                            new Tuple<int, int>(-1, 140),
                            new Tuple<int, int>(-2, 180)),
                        reader.Get<int>("HomeReputation", new Tuple<int, int>(0, 100)),
                        reader.Get<int>("CurrentReputation", new Tuple<int, int>(0, 100)),
                        reader.Get<int>("WorldReputation", new Tuple<int, int>(0, 100)),
                        reader.Get<int>("RightFoot"),
                        reader.Get<int>("LeftFoot"),
                        CountryData.GetByid(reader.Get<int?>("NationID1")),
                        CountryData.GetByid(reader.Get<int?>("NationID2")),
                        ClubData.GetByid(reader.Get<int?>("ClubContractID")),
                        reader.Get<DateTime?>("DateContractStart"),
                        reader.Get<DateTime?>("DateContractEnd"),
                        reader.Get<int>("Wage"),
                        reader.Get<int>("Value"),
                        reader.Get<int>("Caps"),
                        reader.Get<int>("IntGoals"));
                },
                "ISNULL([ClubContractID], " + Constants.NoClubId.ToString() + ") = @club",
                new SqlParameter("@club", club.Id));
            
            using (var conn = new SqlConnection(Settings.Default.connectionString))
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT [position_ID], [rate] FROM [dbo].[player_position] WHERE [player_ID] = @player";
                    cmd.Parameters.Add("@player", System.Data.SqlDbType.Int);
                    cmd.Prepare();
                    foreach (PlayerData p in PlayerData.Instances)
                    {
                        cmd.Parameters["@player"].Value = p.Id;
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                p.SetPosition((PositionData)reader.Get<int>("position_ID"),
                                    reader.Get<int>("rate"));
                            }
                        }
                    }
                    
                    cmd.CommandText = "SELECT [side_ID], [rate] FROM [dbo].[player_side] WHERE [player_ID] = @player";
                    cmd.Prepare();
                    foreach (PlayerData p in PlayerData.Instances)
                    {
                        cmd.Parameters["@player"].Value = p.Id;
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                p.SetSide((SideData)reader.Get<int>("side_ID"),
                                    reader.Get<int>("rate"));
                            }
                        }
                    }

                    cmd.CommandText = "SELECT [attribute_ID], [rate] FROM [dbo].[player_attribute] WHERE [player_ID] = @player";
                    cmd.Prepare();
                    foreach (PlayerData p in PlayerData.Instances)
                    {
                        cmd.Parameters["@player"].Value = p.Id;
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                p.SetAttribute(AttributeData.GetByid(reader.Get<int>("attribute_ID")),
                                    reader.Get<int>("rate"));
                            }
                        }
                    }
                }
            }

            foreach (PlayerData p in PlayerData.Instances)
            {
                p.AdjustPositionAndSide();
            }
        }

        internal void ParseSequencesFileToHashset()
        {
            string[] rows = Properties.Resources.Sequences.Split(
                new string[] { "\r\n", "\n", "\r" },
                StringSplitOptions.RemoveEmptyEntries);

            int r = rows.Length / Environment.ProcessorCount;
            int i = 0;
            HashSet<HashSet<int>> currentHashset = null;
            foreach (string row in rows)
            {
                if (i % r == 0)
                {
                    currentHashset = new HashSet<HashSet<int>>();
                    _parallelSequences.Add(currentHashset);
                }

                var tmp = new HashSet<int>();
                foreach (char c in row)
                {
                    if (c != SEQUENCE_SEPARATOR)
                    {
                        tmp.Add(c - '0');
                    }
                }
                currentHashset.Add(tmp);
                i++;
            }
        }

        internal Dictionary<PlayerData, KeyValuePair<PositionData, SideData>> BestLineUpForTactic(TacticData tactic)
        {
            // Assumes the selected goalkeeper can't be a better choice at another position.
            // The side is useless here.
            PlayerData gkPlayer = GetSquadBestPlayerByPositionAndSide(PlayerData.Instances, PositionData.GK, SideData.C);

            List<PlayerData> fullSquadWithoutSelectedGk = new List<PlayerData>(PlayerData.Instances);
            fullSquadWithoutSelectedGk.Remove(gkPlayer);

            var bestPlayersByPosition = new Dictionary<KeyValuePair<PositionData, SideData>, Dictionary<PlayerData, int>>();
            foreach (KeyValuePair<PositionData, SideData> positionAndSide in tactic.Positions)
            {
                if (bestPlayersByPosition.ContainsKey(positionAndSide))
                {
                    continue;
                }
                bestPlayersByPosition.Add(positionAndSide,
                    fullSquadWithoutSelectedGk
                        .Select(p =>
                            new Tuple<PlayerData, int>(p,
                                p.GlobalRate * p.Positions[positionAndSide.Key] * p.Sides[positionAndSide.Value]))
                        .OrderByDescending(p => p.Item2)
                        .ToDictionary(p => p.Item1, p => p.Item2));
            }

            var linesUp = new ConcurrentDictionary<Dictionary<PlayerData, KeyValuePair<PositionData, SideData>>, int>();

            var start = DateTime.Now;
            System.Threading.Tasks.Parallel.For(0, _parallelSequences.Count, (int i) =>
            {
                var subBestLineUp = new Dictionary<PlayerData, KeyValuePair<PositionData, SideData>>();
                // NB : the line-up rate doesn't include the GK (it's not required).
                int subBestLineUpRate = 0;

                foreach (HashSet<int> sequence in _parallelSequences[i])
                {
                    var currentLineUp = new Dictionary<PlayerData, KeyValuePair<PositionData, SideData>>();
                    int currentLineUpRate = 0;
                    foreach (int tacticTupleIndex in sequence)
                    {
                        KeyValuePair<PositionData, SideData> tacticTuple = tactic.PositionAt(tacticTupleIndex);

                        PlayerData pickP = null;
                        int valueP = -1;
                        foreach (PlayerData currentP in bestPlayersByPosition[tacticTuple].Keys)
                        {
                            if (!currentLineUp.ContainsKey(currentP))
                            {
                                pickP = currentP;
                                valueP = bestPlayersByPosition[tacticTuple][currentP];
                                break;
                            }
                        }

                        if (pickP != null)
                        {
                            currentLineUpRate += valueP;
                        }
                        currentLineUp.Add(pickP, tacticTuple);
                    }

                    if (currentLineUpRate > subBestLineUpRate)
                    {
                        subBestLineUp = currentLineUp;
                        subBestLineUpRate = currentLineUpRate;
                    }
                }

                if (!linesUp.TryAdd(subBestLineUp, subBestLineUpRate))
                {
                    throw new NotImplementedException();
                }
            });
            var end = DateTime.Now;

            System.Diagnostics.Debug.WriteLine("Execution time : " + (end - start).TotalMilliseconds);

            var bestLineUp = linesUp.OrderByDescending(kvp => kvp.Value).First().Key;

            bestLineUp.Add(gkPlayer, new KeyValuePair<PositionData, SideData>(PositionData.GK, SideData.C));

            return bestLineUp;
        }

        internal PlayerData GetSquadBestPlayerByPositionAndSide(IEnumerable<PlayerData> squad, PositionData position, SideData side)
        {
            return squad
                .Where(p => p.Positions[position] >= Constants.THRESHOLD_RATE
                    && (position == PositionData.GK || p.Sides[side] >= Constants.THRESHOLD_RATE))
                .OrderByDescending(p => p.GlobalRate * p.Positions[position] * (position == PositionData.GK ? 20 : p.Sides[side]))
                .FirstOrDefault();
        }
    }
}
