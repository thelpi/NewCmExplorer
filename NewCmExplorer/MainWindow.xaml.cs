using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using NewCmExplorer.Data;

namespace NewCmExplorer
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const char SEQUENCE_SEPARATOR = ';';

        private static List<HashSet<HashSet<int>>> _parallelSequences;

        /// <summary>
        /// Constructor.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();

            _parallelSequences = new List<HashSet<HashSet<int>>>();

            BackgroundWorker worker = new BackgroundWorker();
            worker.DoWork += delegate (object sender, DoWorkEventArgs e)
            {
                DataMapper.Instance.LoadStaticDatas();
                string[] rows = Properties.Resources.Sequences.Split(
                    new string[] { "\r\n", "\n", "\r" }, StringSplitOptions.RemoveEmptyEntries);

                int i = 0;
                foreach (string row in rows)
                {
                    if (i % (rows.Length / Environment.ProcessorCount) == 0)
                    {
                        _parallelSequences.Add(new HashSet<HashSet<int>>());
                    }

                    string[] sequence = row.Split(new[] { SEQUENCE_SEPARATOR });
                    _parallelSequences.Last().Add(new HashSet<int>(sequence.Select(s => Convert.ToInt32(s))));
                    i++;
                }
            };
            worker.RunWorkerCompleted += delegate (object sender, RunWorkerCompletedEventArgs e)
            {
                MainPanel.IsEnabled = true;
                PanelProgress.Visibility = Visibility.Collapsed;
                ComboTactics.ItemsSource = TacticData.DefaultTactics;
                ComboTactics.DisplayMemberPath = nameof(TacticData.Name);
                ComboClubs.ItemsSource = ClubData.Instances;
                ComboClubs.DisplayMemberPath = nameof(ClubData.ShortName);
                FullStaff.ItemsSource = PlayerData.Instances;
                FullStaff.DisplayMemberPath = nameof(PlayerData.FullName);
            };
            worker.RunWorkerAsync();
        }

        private void ComboTactics_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Refreh();
        }

        private void ComboClubs_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Refreh();
        }

        private void Refreh()
        {
            if (ComboClubs.SelectedIndex < 0 || ComboTactics.SelectedIndex < 0)
            {
                return;
            }

            MainPanel.IsEnabled = false;
            PanelProgress.Visibility = Visibility.Visible;
            BackgroundWorker worker = new BackgroundWorker();
            worker.DoWork += delegate (object sender, DoWorkEventArgs e)
            {
                object[] arguments = e.Argument as object[];
                DataMapper.Instance.LoadPlayers((ClubData)(arguments[0]));
                e.Result = BestLineUpForTactic((TacticData)(arguments[1]));
            };
            worker.RunWorkerCompleted += delegate (object sender, RunWorkerCompletedEventArgs e)
            {
                MainPanel.IsEnabled = true;
                PanelProgress.Visibility = Visibility.Collapsed;
                FullStaff.Items.Refresh();

                if (e.Error != null)
                {
                    MessageBox.Show($"{e.Error.Message}\r\n\r\n{e.Error.StackTrace}");
                }
                else if (e.Result != null)
                {
                    SetLineUpGrid(e.Result as Dictionary<PlayerData, KeyValuePair<PositionData, SideData>>);
                }
            };
            worker.RunWorkerAsync(new object[] { ComboClubs.SelectedItem, ComboTactics.SelectedItem });
        }

        private void SetLineUpGrid(Dictionary<PlayerData, KeyValuePair<PositionData, SideData>> lineUp)
        {
            FieldGrid.Children.Clear();
            foreach (PlayerData player in lineUp.Keys)
            {
                int col = GetGridColumnFromSide(lineUp, player, lineUp[player]);
                int row = GetGridRowFromPosition(lineUp[player].Key);
                AddPlayerToFieldGrid(player, row, col);
            }
        }

        private int GetGridColumnFromSide(Dictionary<PlayerData, KeyValuePair<PositionData, SideData>> lineUp,
            PlayerData player, KeyValuePair<PositionData, SideData> posAndSideTuple)
        {
            // Shortcuts.
            PositionData position = posAndSideTuple.Key;
            SideData side = posAndSideTuple.Value;

            // One goalkeeper.
            if (position == PositionData.GK)
            {
                return 2; // Middle column.
            }
            // There's never more than one player on the left side for a given position.
            else if (side == SideData.L)
            {
                return 0; // First column.
            }
            // There's never more than one player on the right side for a given position.
            else if (side == SideData.R)
            {
                return 4; // Last column.
            }

            // The current player is the only one for this central position
            if (lineUp.Count(s => s.Value.Key == position && s.Value.Value == SideData.C) == 1)
            {
                return 2; // Middle column.
            }

            // Count of players already fixed for this position.
            int playersCountFixed = lineUp.Count(s =>
                s.Value.Key == position
                && s.Value.Value == SideData.C
                && lineUp.Keys.ToList().IndexOf(s.Key) < lineUp.Keys.ToList().IndexOf(player));

            // Two players overall (including the current one) to fix at the current position.
            if (lineUp.Count(s => s.Value.Key == position && s.Value.Value == SideData.C) == 2)
            {
                // Middle-left or middle-right column, depending on players already fixed.
                return playersCountFixed > 0 ? 3 : 1;
            }

            // Three players overall (including the current one) to fix at the current position.
            if (lineUp.Count(s => s.Value.Key == position && s.Value.Value == SideData.C) == 3)
            {
                // Middle-left, middle-right or middle column, depending on players already fixed.
                return playersCountFixed > 1 ? 3 : (playersCountFixed > 0 ? 2 : 1);
            }

            // No tactic with more than 3 players on a central position.
            throw new NotImplementedException();
        }

        private static int GetGridRowFromPosition(PositionData position)
        {
            switch (position)
            {
                case PositionData.SW:
                    return 5;
                case PositionData.D:
                    return 4;
                case PositionData.DM:
                    return 3;
                case PositionData.M:
                    return 2;
                case PositionData.OM:
                    return 1;
                case PositionData.S:
                    return 0;
                case PositionData.GK:
                    return 6;
                default:
                    throw new NotImplementedException();
            }
        }

        private static Dictionary<PlayerData, KeyValuePair<PositionData, SideData>> BestLineUpForTactic(TacticData tactic)
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

        private static PlayerData GetSquadBestPlayerByPositionAndSide(IEnumerable<PlayerData> squad, PositionData position, SideData side)
        {
            return squad
                .Where(p => p.Positions[position] >= Constants.THRESHOLD_RATE
                    && (position == PositionData.GK || p.Sides[side] >= Constants.THRESHOLD_RATE))
                .OrderByDescending(p => p.GlobalRate * p.Positions[position] * (position == PositionData.GK ? 20 : p.Sides[side]))
                .FirstOrDefault();
        }

        private void AddPlayerToFieldGrid(PlayerData p, int row, int col)
        {
            var e = new Ellipse
            {
                Fill = Brushes.Yellow,
                Width = 70,
                Height = 70,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };
            var t = new TextBlock
            {
                Text = p?.FullName ?? Constants.DISPLAY_FIELD_NO_PLAYER,
                TextWrapping = TextWrapping.WrapWithOverflow,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };

            e.SetValue(Grid.RowProperty, row);
            e.SetValue(Grid.ColumnProperty, col);
            e.SetValue(Panel.ZIndexProperty, 1);
            t.SetValue(Grid.RowProperty, row);
            t.SetValue(Grid.ColumnProperty, col);
            t.SetValue(Panel.ZIndexProperty, 2);

            FieldGrid.Children.Add(e);
            FieldGrid.Children.Add(t);
        }
    }
}
