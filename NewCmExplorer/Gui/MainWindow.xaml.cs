using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using NewCmExplorer.Data;
using NewCmExplorer.Engine;
using NewCmExplorer.Tools;

namespace NewCmExplorer.Gui
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();

            BackgroundWorker worker = new BackgroundWorker
            {
                WorkerReportsProgress = true,
                WorkerSupportsCancellation = false
            };
            worker.DoWork += delegate (object sender, DoWorkEventArgs e)
            {
                DataMapper.Instance.LoadStaticDatas(worker);
            };
            worker.RunWorkerCompleted += delegate (object sender, RunWorkerCompletedEventArgs e)
            {
                MainPanel.IsEnabled = true;
                PanelProgress.Visibility = Visibility.Collapsed;

                ComboConfederations.ItemsSource = ConfederationData.Instances;
                ComboConfederations.DisplayMemberPath = nameof(ConfederationData.ContName);
                ComboCountries.ItemsSource = CountryData.Instances;
                ComboCountries.DisplayMemberPath = nameof(CountryData.ShortName);
                ComboDivision.DisplayMemberPath = nameof(ClubCompetitionData.ShortName);
                ComboDivision.ItemsSource = ClubCompetitionData.Instances;
                ComboClubs.DisplayMemberPath = nameof(ClubData.ShortName);
                ComboClubs.ItemsSource = ClubData.Instances;
                ComboTactics.ItemsSource = TacticData.DefaultTactics;
                ComboTactics.DisplayMemberPath = nameof(TacticData.Name);
                FullStaff.ItemsSource = PlayerData.Instances;
                FullStaff.DisplayMemberPath = nameof(PlayerData.FullName);
            };
            worker.ProgressChanged += delegate (object sender, ProgressChangedEventArgs e)
            {
                LoadingProgressBar.Value = e.ProgressPercentage;
            };
            worker.RunWorkerAsync();
        }

        #region Graphic tools

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

        private void Refreh()
        {
            if (ComboClubs.SelectedIndex < 0 || ComboTactics.SelectedIndex < 0)
            {
                return;
            }

            MainPanel.IsEnabled = false;
            PanelProgress.Visibility = Visibility.Visible;
            BackgroundWorker worker = new BackgroundWorker
            {
                WorkerReportsProgress = true,
                WorkerSupportsCancellation = false
            };
            worker.DoWork += delegate (object sender, DoWorkEventArgs e)
            {
                object[] arguments = e.Argument as object[];
                DataMapper.Instance.LoadPlayers((ClubData)(arguments[0]));
                e.Result = DataMapper.Instance.BestLineUpForTactic((TacticData)(arguments[1]), worker);
            };
            worker.ProgressChanged += delegate (object sender, ProgressChangedEventArgs e)
            {
                LoadingProgressBar.Value = e.ProgressPercentage;
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

        #endregion Graphic tools

        #region Events

        private void ComboTactics_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Refreh();
        }

        private void ComboClubs_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Refreh();
        }

        private void ComboConfederations_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ComboConfederations.SelectedItem != null)
            {
                ComboCountries.ItemsSource = CountryData.Instances.Where(c => c.Confederation == ComboConfederations.SelectedItem);
                ComboDivision.ItemsSource = ClubCompetitionData.Instances.Where(c => c.Confederation == ComboConfederations.SelectedItem);
                ComboClubs.ItemsSource = ClubData.Instances.Where(c => c.Country?.Confederation == ComboConfederations.SelectedItem);
            }
            else
            {
                ComboCountries.ItemsSource = CountryData.Instances;
                ComboDivision.ItemsSource = ClubCompetitionData.Instances;
                ComboClubs.ItemsSource = ClubData.Instances;
            }
        }

        private void ComboCountries_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ComboCountries.SelectedItem != null)
            {
                ComboDivision.ItemsSource = ClubCompetitionData.Instances.Where(c => c.Country == ComboCountries.SelectedItem);
                ComboClubs.ItemsSource = ClubData.Instances.Where(c => c.Country == ComboCountries.SelectedItem);
            }
            else
            {
                ComboDivision.ItemsSource = ClubCompetitionData.Instances;
                ComboClubs.ItemsSource = ClubData.Instances;
            }
        }

        private void ComboDivision_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ComboDivision.SelectedItem != null)
            {
                ComboClubs.ItemsSource = ClubData.Instances.Where(c => c.Division == ComboDivision.SelectedItem);
            }
            else
            {
                ComboClubs.ItemsSource = ClubData.Instances;
            }
        }

        #endregion Events
    }
}
