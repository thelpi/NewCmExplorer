using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace NewCmExplorer.Data
{
    /// <summary>
    /// Base class for every datas.
    /// </summary>
    public abstract class BaseData
    {
        /// <summary>
        /// Identifier.
        /// </summary>
        public int Id { get; private set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="id">Identifier.</param>
        protected BaseData(int id)
        {
            Id = id;

            _instances[GetType()].Add(this);
        }

        private static readonly Dictionary<Type, List<BaseData>> _instances =
            Assembly
                .GetAssembly(typeof(BaseData))
                .GetTypes()
                .Where(t => t.IsSubclassOf(typeof(BaseData)))
                .ToDictionary(t => t, t => new List<BaseData>());

        /// <summary>
        /// Gets every instances of the targeted type.
        /// </summary>
        /// <typeparam name="T">The targeted type.</typeparam>
        /// <returns>List of instances.</returns>
        protected static List<T> GetInstancesOfType<T>() where T : BaseData
        {
            return _instances[typeof(T)].Cast<T>().ToList();
        }

        /// <summary>
        /// Clears every instances of the targeted type.
        /// </summary>
        /// <typeparam name="T">The targeted type.</typeparam>
        protected static void Clear<T>() where T : BaseData
        {
            _instances[typeof(T)].Clear();
        }

        /// <summary>
        /// Finds the instance of the targeted type for the specified identifier.
        /// </summary>
        /// <typeparam name="T">The targeted type.</typeparam>
        /// <param name="id">Identifier.</param>
        /// <returns>The instance; <c>Null</c> if not found.</returns>
        protected static T GetByid<T>(int? id) where T : BaseData
        {
            return id.HasValue ? (T)_instances[typeof(T)].FirstOrDefault(i => i.Id == id.Value) : null;
        }
    }
}
