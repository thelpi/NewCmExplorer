using System;
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
        /// <summary>
        /// Constructor.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();

            BackgroundWorker worker = new BackgroundWorker();
            worker.DoWork += delegate (object sender, DoWorkEventArgs e)
            {
                DataMapper.Instance.LoadStaticDatas();
            };
            worker.RunWorkerCompleted += delegate (object sender, RunWorkerCompletedEventArgs e)
            {
                MainPanel.IsEnabled = true;
                PanelProgress.Visibility = Visibility.Collapsed;
                ComboTactics.ItemsSource = TacticData.DefaultTactics;
                ComboTactics.DisplayMemberPath = nameof(TacticData.Name);
                ComboClubs.ItemsSource = DataMapper.Instance.Clubs;
                ComboClubs.DisplayMemberPath = nameof(ClubData.ShortName);
                FullStaff.ItemsSource = DataMapper.Instance.Players;
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
                SetLineUpGrid(e.Result as List<Tuple<PlayerData, Tuple<PositionData, SideData>>>);
            };
            worker.RunWorkerAsync(new object[] { ComboClubs.SelectedItem, ComboTactics.SelectedItem });
        }

        private void SetLineUpGrid(List<Tuple<PlayerData, Tuple<PositionData, SideData>>> lineUp)
        {
            FieldGrid.Children.Clear();
            foreach (Tuple<PlayerData, Tuple<PositionData, SideData>> tuplePlayerWithPos in lineUp)
            {
                int col = GetGridColumnFromSide(lineUp, tuplePlayerWithPos);
                int row = GetGridRowFromPosition(tuplePlayerWithPos.Item2.Item1);
                AddPlayerToFieldGrid(tuplePlayerWithPos.Item1, row, col);
            }
        }

        private int GetGridColumnFromSide(List<Tuple<PlayerData, Tuple<PositionData, SideData>>> lineUp,
            Tuple<PlayerData, Tuple<PositionData, SideData>> currentPlayerOfLineUp)
        {
            // Shortcuts.
            PositionData position = currentPlayerOfLineUp.Item2.Item1;
            SideData side = currentPlayerOfLineUp.Item2.Item2;

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
            if (lineUp.Count(s => s.Item2.Item1 == position && s.Item2.Item2 == SideData.C) == 1)
            {
                return 2; // Middle column.
            }

            // Count of players already fixed for this position.
            int playersCountFixed = lineUp.Count(s =>
                s.Item2.Item1 == position
                && s.Item2.Item2 == SideData.C
                && lineUp.IndexOf(s) < lineUp.IndexOf(currentPlayerOfLineUp));

            // Two players overall (including the current one) to fix at the current position.
            if (lineUp.Count(s => s.Item2.Item1 == position && s.Item2.Item2 == SideData.C) == 2)
            {
                // Middle-left or middle-right column, depending on players already fixed.
                return playersCountFixed > 0 ? 3 : 1;
            }

            // Three players overall (including the current one) to fix at the current position.
            if (lineUp.Count(s => s.Item2.Item1 == position && s.Item2.Item2 == SideData.C) == 3)
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

        private static List<Tuple<PlayerData, Tuple<PositionData, SideData>>> BestLineUpForTactic(TacticData tactic)
        {
            var bestSquad = new List<Tuple<PlayerData, Tuple<PositionData, SideData>>>();

            PlayerData gkPlayer = GetSquadBestPlayerByPositionAndSide(DataMapper.Instance.Players, PositionData.GK, SideData.C);
            int maxPower = 0;
            int card = tactic.Positions.Count;
            for (int a0 = 0; a0 < card; a0++)
            {
                for (int a1 = 0; a1 < card; a1++)
                {
                    if (a1 == a0) continue;
                    for (int a2 = 0; a2 < card; a2++)
                    {
                        if (a2 == a0 || a2 == a1) continue;
                        for (int a3 = 0; a3 < card; a3++)
                        {
                            if (a3 == a0 || a3 == a1 || a3 == a2) continue;
                            for (int a4 = 0; a4 < card; a4++)
                            {
                                if (a4 == a0 || a4 == a1 || a4 == a2 || a4 == a3) continue;
                                for (int a5 = 0; a5 < card; a5++)
                                {
                                    if (a5 == a0 || a5 == a1 || a5 == a2 || a5 == a3 || a5 == a4) continue;
                                    for (int a6 = 0; a6 < card; a6++)
                                    {
                                        if (a6 == a0 || a6 == a1 || a6 == a2 || a6 == a3 || a6 == a4 || a6 == a5) continue;
                                        for (int a7 = 0; a7 < card; a7++)
                                        {
                                            if (a7 == a0 || a7 == a1 || a7 == a2 || a7 == a3 || a7 == a4 || a7 == a5 || a7 == a6) continue;
                                            for (int a8 = 0; a8 < card; a8++)
                                            {
                                                if (a8 == a0 || a8 == a1 || a8 == a2 || a8 == a3 || a8 == a4 || a8 == a5 || a8 == a6 || a8 == a7) continue;
                                                for (int a9 = 0; a9 < card; a9++)
                                                {
                                                    if (a9 == a0 || a9 == a1 || a9 == a2 || a9 == a3 || a9 == a4 || a9 == a5 || a9 == a6 || a9 == a7 || a9 == a8) continue;
                                                    var allA = new List<int> { a0, a1, a2, a3, a4, a5, a6, a7, a8, a9 };
                                                    List<PlayerData> currentList = new List<PlayerData>(DataMapper.Instance.Players);
                                                    currentList.Remove(gkPlayer);
                                                    var squad = new List<Tuple<PlayerData, Tuple<PositionData, SideData>>>();
                                                    int power = 0;
                                                    foreach (int a in allA)
                                                    {
                                                        Tuple<PositionData, SideData> currentPos = tactic.Positions.ElementAt(a);
                                                        PlayerData currentABest = GetSquadBestPlayerByPositionAndSide(currentList, currentPos.Item1, currentPos.Item2);
                                                        if (currentABest != null)
                                                        {
                                                            currentList.Remove(currentABest);
                                                            power += currentABest.GlobalRate * currentABest.Positions[currentPos.Item1] * currentABest.Sides[currentPos.Item2];
                                                        }
                                                        squad.Add(new Tuple<PlayerData, Tuple<PositionData, SideData>>(currentABest, currentPos));
                                                    }
                                                    if (power > maxPower)
                                                    {
                                                        bestSquad = squad;
                                                        maxPower = power;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            bestSquad.Insert(0, new Tuple<PlayerData, Tuple<PositionData, SideData>>(gkPlayer, new Tuple<PositionData, SideData>(PositionData.GK, SideData.C)));
            return bestSquad;
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
