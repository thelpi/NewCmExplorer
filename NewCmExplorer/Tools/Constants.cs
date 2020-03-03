using System;

namespace NewCmExplorer.Tools
{
    /// <summary>
    /// Set of constants.
    /// </summary>
    internal static class Constants
    {
        /// <summary>
        /// Instance of random numbers generator.
        /// </summary>
        internal static readonly Random Randomizer = new Random(DateTime.Now.Millisecond);

        /// <summary>
        /// Date pattern for display.
        /// </summary>
        internal const string DATE_PATTERN = "dd/MM/yyyy";
        /// <summary>
        /// Value to display for unknown date.
        /// </summary>
        internal const string UNKNOWN_DATE = "Inconnue";
        /// <summary>
        /// The minimal rate to be considered at a specific position or side.
        /// </summary>
        internal const int THRESHOLD_RATE = 15;
        /// <summary>
        /// Text to display when there's no player for a specified position on the field.
        /// </summary>
        internal const string DISPLAY_FIELD_NO_PLAYER = "<unavailable>";
        /// <summary>
        /// Identifier of virtual club to represent unemployment.
        /// </summary>
        internal const int NoClubId = -1;
        /// <summary>
        /// Name of virtual club to represent unemployment.
        /// </summary>
        internal const string NoClubName = "No club";
        /// <summary>
        /// Default value for <see cref="Engine.ResultGenerator.HomeAdvantageRate"/>.
        /// </summary>
        internal const double DEFAULT_HOME_ADVANTAGE_RATE = 4 / 3;
        /// <summary>
        /// Default value for <see cref="Engine.ResultGenerator.DrawRate"/>.
        /// </summary>
        internal const double DEFAULT_DRAW_RATE = 0.25;
        /// <summary>
        /// Default value for <see cref="Engine.ResultGenerator.GoalsCountMean"/>.
        /// </summary>
        internal const double DEFAULT_GOALS_COUNT_MEAN = 2.5;
        /// <summary>
        /// Default value for <see cref="Engine.ResultGenerator.GoalsCountDeviation"/>.
        /// </summary>
        internal const double DEFAULT_GOALS_COUNT_DEVIATION = 1.5;
    }
}
