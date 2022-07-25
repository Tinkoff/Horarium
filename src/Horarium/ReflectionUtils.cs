using System;

namespace Horarium
{
    /// <summary>
    /// Contains useful methods for working with reflection.
    /// </summary>
    internal static class ReflectionUtils
    {
        /// <summary>
        /// Checks if a type implements a specific interface.
        /// </summary>
        /// <typeparam name="TInterface"> The interface to look for in the type. </typeparam>
        /// <param name="type"> The type in which to look for the interface. </param>
        /// <returns> Whether specified type implements <typeparamref name="TInterface"/> interface. </returns>
        public static bool Implements<TInterface>(this Type type)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));

            return Implements(type, typeof(TInterface));
        }

        /// <summary>
        /// Checks if a type implements a specific interface.
        /// </summary>
        /// <param name="type"> The type in which to look for the interface </param>
        /// <param name="interface"> The interface to look for in the type. </param>
        /// <returns> Whether specified type implements <paramref name="interface"/> interface. </returns>
        public static bool Implements(this Type type, Type @interface)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));

            if (@interface is null)
                throw new ArgumentNullException(nameof(@interface));

            return Array.IndexOf(type.GetInterfaces(), @interface) != -1;
        }
    }
}
