namespace NewCmExplorer.Data
{
    /// <summary>
    /// Set of constants.
    /// </summary>
    internal static class Constants
    {
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
    }
}
