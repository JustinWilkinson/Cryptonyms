using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cryptonyms.Server.Extensions
{
    /// <summary>
    /// Contains useful extensions on IEnumerables and IAsyncEnumerables.
    /// </summary>
    public static class EnumerableExtensions
    {
        /// <summary>
        /// Returns the only element of a sequence, and throws an exception if there is not exactly one element in the sequence.
        /// </summary>
        public static async ValueTask<T> SingleAsync<T>(this IAsyncEnumerable<T> source)
        {
            NullGuards(source);
            
            var enumerator = source.GetAsyncEnumerator();
            if (await enumerator.MoveNextAsync())
            {
                if (await enumerator.MoveNextAsync())
                {
                    throw new InvalidOperationException("The input sequence contains more than one element.");
                }

                return enumerator.Current;
            }

            throw new InvalidOperationException("The input sequence is empty.");
        }

        /// <summary>
        /// Returns the only element of a sequence or the default value if empty, and throws an exception if there is more than one element in the sequence.
        /// </summary>
        public static async ValueTask<T> SingleOrDefaultAsync<T>(this IAsyncEnumerable<T> source)
        {
            NullGuards(source);

            var enumerator = source.GetAsyncEnumerator();
            if (await enumerator.MoveNextAsync())
            {
                if (await enumerator.MoveNextAsync())
                {
                    throw new InvalidOperationException("The input sequence contains more than one element.");
                }

                return enumerator.Current;
            }

            return default;
        }


        /// <summary>
        /// Filters an IAsyncEnumerable.
        /// </summary>
        public static async IAsyncEnumerable<T> WhereAsync<T>(this IAsyncEnumerable<T> source, Func<T, bool> predicate)
        {
            NullGuards(source);

            await foreach (var item in source)
            {
                if (predicate(item))
                {
                    yield return item;
                }
            }
        }

        /// <summary>
        /// Projects an IAsyncEnumerable into another IAsyncEnumerable.
        /// </summary>
        public static async IAsyncEnumerable<TOut> SelectAsync<TIn, TOut>(this IAsyncEnumerable<TIn> source, Func<TIn, TOut> projection)
        {
            NullGuards(source);

            await foreach (var item in source)
            {
                yield return projection(item);
            }
        }

        /// <summary>
        /// Converts an IEnumerablet to an IAsyncEnumerable.
        /// </summary>
        public static async IAsyncEnumerable<T> ToAsyncEnumerable<T>(this IEnumerable<T> source)
        {
            NullGuards(source);

            await Task.CompletedTask;

            foreach (var item in source)
            {
                yield return item;
            }
        }

        /// <summary>
        /// Iterates over the async enumerable and returns an enumerable.
        /// </summary>
        public static async Task<IEnumerable<T>> ToEnumerableAsync<T>(this IAsyncEnumerable<T> source)
        {
            NullGuards(source);

            var list = new List<T>();
            await foreach (var item in source)
            {
                list.Add(item);
            }
            return list;
        }

        private static void NullGuards(object source)
        {
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }
        }
    }
}
