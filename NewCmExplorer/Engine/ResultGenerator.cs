using System;
using NewCmExplorer.Tools;

namespace NewCmExplorer.Engine
{
    /// <summary>
    /// Provides statistic tools to compute the result a of game.
    /// </summary>
    internal class ResultGenerator
    {
        /// <summary>
        /// The advantage rate to play at home (for two teams of the same value).
        /// <c>1</c> is no advantage nor inconvenient.
        /// </summary>
        public double HomeAdvantageRate { get; private set; }
        /// <summary>
        /// The probability rate (on <c>1</c>) to get a draw result (for two teams of the same value).
        /// </summary>
        public double DrawRate { get; private set; }
        /// <summary>
        /// Goals count mean (for two teams with average offense and defense).
        /// </summary>
        public double GoalsCountMean { get; private set; }
        /// <summary>
        /// Goals count standard deviation (for two teams with average offense and defense).
        /// </summary>
        public double GoalsCountDeviation { get; private set; }

        /// <summary>
        /// Default constructor.
        /// </summary>
        internal ResultGenerator() : this(Constants.DEFAULT_HOME_ADVANTAGE_RATE,
            Constants.DEFAULT_DRAW_RATE,
            Constants.DEFAULT_GOALS_COUNT_MEAN,
            Constants.DEFAULT_GOALS_COUNT_DEVIATION) { }

        /// <summary>
        /// Custom constructor.
        /// </summary>
        /// <param name="homeAdvantageRate"><see cref="HomeAdvantageRate"/></param>
        /// <param name="drawRate"><see cref="DrawRate"/></param>
        /// <param name="goalsCountMean"><see cref="GoalsCountMean"/></param>
        /// <param name="goalsCountDeviation"><see cref="GoalsCountDeviation"/></param>
        internal ResultGenerator(double homeAdvantageRate, double drawRate, double goalsCountMean, double goalsCountDeviation)
        {
            HomeAdvantageRate = homeAdvantageRate;
            DrawRate = drawRate;
            GoalsCountMean = goalsCountMean;
            GoalsCountDeviation = goalsCountDeviation;
        }

        /// <summary>
        /// Computes a random number by following a normal distribution.
        /// </summary>
        /// <param name="mean">The mean.</param>
        /// <param name="deviation">The standard deviation.</param>
        /// <returns>The random number.</returns>
        public static double GetNormalDistributionRandomNumber(double mean, double deviation)
        {
            return mean + deviation * (Math.Pow(-2 * Math.Log(Constants.Randomizer.NextDouble()), 0.5) * Math.Cos(2 * Math.PI * Constants.Randomizer.NextDouble()));
        }
    }
}
