using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace NewCmExplorer.Data
{
    /// <summary>
    /// Extension methods.
    /// </summary>
    internal static class Extensions
    {
        /// <summary>
        /// Gets a typed value for a specified column of a <see cref="SqlDataReader"/>.
        /// </summary>
        /// <typeparam name="T">Targeted type.</typeparam>
        /// <param name="reader">Instance of <see cref="SqlDataReader"/>.</param>
        /// <param name="columnName">Column name.</param>
        /// <param name="substitutions">Specific values substitutions.</param>
        /// <returns>Instance of targeted type.</returns>
        internal static T Get<T>(this SqlDataReader reader, string columnName, params Tuple<T, T>[] substitutions)
        {
            var notNullableT = Nullable.GetUnderlyingType(typeof(T));
            var baseValue = reader[columnName] == DBNull.Value ? default(T) :
                (T)Convert.ChangeType(reader[columnName], notNullableT ?? typeof(T));

            if (substitutions != null)
            {
                foreach (Tuple<T, T> sub in substitutions)
                {
                    if (baseValue.Equals(sub.Item1))
                    {
                        return sub.Item2;
                    }
                }
            }

            return baseValue;
        }
    }
}
