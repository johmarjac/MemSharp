using System;
using System.Collections.Concurrent;

namespace Ether.Network.Extensions
{
    /// <summary>
    /// Contains extensions for <see cref="BlockingCollection{T}"/> objects.
    /// </summary>
    public static class BlockingCollectionExtensions
    {
        /// <summary>
        /// Clears a <see cref="BlockingCollection{T}"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="blockingCollection"></param>
        public static void Clear<T>(this BlockingCollection<T> blockingCollection)
        {
            if (blockingCollection == null)
            {
                throw new ArgumentNullException(nameof(blockingCollection));
            }

            while (blockingCollection.Count > 0)
            {
                blockingCollection.TryTake(out _);
            }
        }
    }
}
